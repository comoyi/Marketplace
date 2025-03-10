﻿using Marketplace.Modules.Global_Options;
using Object = UnityEngine.Object;

namespace Marketplace.Modules.MainMarketplace;

[UsedImplicitly]
[Market_Autoload(Market_Autoload.Type.Client, Market_Autoload.Priority.Normal)]
public static class Marketplace_Main_Client
{
    public static int IncomeValue;
    public static Action OnUpdateCurrency;

    [UsedImplicitly]
    private static void OnInit()
    {
        Marketplace_UI.Init();
        Marketplace.Global_Updator += Update;
        Marketplace.Global_OnGUI_Updator += Marketplace_Messages.OnGUI;
        Marketplace_DataTypes.SyncedMarketplaceData.ValueChanged += OnMarketplaceUpdate;
        Global_Configs.SyncedGlobalOptions.ValueChanged += () => OnUpdateCurrency?.Invoke();
    }

    private static void OnMarketplaceUpdate()
    {
        if (Marketplace_UI.IsPanelVisible()) Marketplace_UI.ResetBUYPage();
    }

    private static void Update(float dt)
    {
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (Marketplace_UI.IsPanelVisible() || Marketplace_Messages._showMessageBox))
        {
            Marketplace_UI.Hide();
            Menu.instance.OnClose();
            Marketplace_Messages._showMessageBox = false;
        }
    }

    [HarmonyPatch(typeof(TextInput), nameof(TextInput.IsVisible))]
    [ClientOnlyPatch]
    private static class MarketplaceUIFix
    {
        [UsedImplicitly]
        private static void Postfix(ref bool __result)
        {
            if (Marketplace_UI.IsPanelVisible()) __result = true;
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [ClientOnlyPatch]
    private static class ZrouteMethodsClient
    {
        [UsedImplicitly]
        private static void Postfix()
        {
            ZRoutedRpc.instance.Register("KGmarket BuyItemAnswer",
                new Action<long, string>(InstantiateItemFromServer));
            ZRoutedRpc.instance.Register("KGmarket ReceiveIncome",
                new Action<long, int>(ReceiveIncomeFromServer));
            ZRoutedRpc.instance.Register("KGmarket GetLocalMessages",
                new Action<long, ZPackage>(Marketplace_Messages.Messenger.GetMessages));
        }
    }

    private static void ReceiveIncomeFromServer(long sender, int value)
    {
        IncomeValue = value;
        Marketplace_UI.ResetIncome();
    }

    private static void InstantiateItemFromServer(long sender, string data)
    {
        if (sender == ZRoutedRpc.instance.GetServerPeerID() && data.Length > 0)
        {
            Player p = Player.m_localPlayer;
            Marketplace_DataTypes.ServerMarketSendData shopItem =
                JSON.ToObject<Marketplace_DataTypes.ServerMarketSendData>(data);
            GameObject main = ZNetScene.instance.GetPrefab(shopItem.ItemPrefab);
            if (!main) return;
            string text = Localization.instance.Localize("$mpasn_added", shopItem.Count.ToString(),
                Localization.instance.Localize(main.GetComponent<ItemDrop>().m_itemData.m_shared.m_name));
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, text);
            ItemDrop item = main.GetComponent<ItemDrop>();
            shopItem.Quality = Mathf.Max(1, shopItem.Quality);
            int stack = shopItem.Count;
            Dictionary<string, string> NewCustomData = JSON.ToObject<Dictionary<string, string>>(shopItem.CUSTOMdata);

            while (stack > 0)
            {
                if (p.m_inventory.FindEmptySlot(false) is { x: >= 0 } pos)
                {
                    int addStack = Math.Min(stack, item.m_itemData.m_shared.m_maxStackSize);
                    stack -= addStack;
                    float durability = item.m_itemData.GetMaxDurability(shopItem.Quality) * Mathf.Clamp01(shopItem.DurabilityPercent / 100f);
                    p.m_inventory.AddItem(shopItem.ItemPrefab, addStack, durability
                        , pos,
                        false, shopItem.Quality, shopItem.Variant, shopItem.CrafterID, shopItem.CrafterName,
                        NewCustomData, Game.m_worldLevel, true);
                }
                else
                {
                    break;
                }
            }

            if (stack <= 0) return;
            while (stack > 0)
            {
                int addStack = Math.Min(stack, item.m_itemData.m_shared.m_maxStackSize);
                stack -= addStack;
                Transform transform = p.transform;
                Vector3 position = transform.position;
                ItemDrop itemDrop = Object.Instantiate(main, position + Vector3.up, transform.rotation)
                    .GetComponent<ItemDrop>();
                itemDrop.m_itemData.m_customData = NewCustomData;
                itemDrop.m_itemData.m_stack = addStack;
                itemDrop.m_itemData.m_crafterName = shopItem.CrafterName;
                itemDrop.m_itemData.m_crafterID = shopItem.CrafterID;
                
                float durability = item.m_itemData.GetMaxDurability(shopItem.Quality) * Mathf.Clamp01(shopItem.DurabilityPercent / 100f);
                itemDrop.m_itemData.m_durability = durability;
                
                itemDrop.Save();
                itemDrop.OnPlayerDrop();
                itemDrop.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up);
                p.m_dropEffects.Create(position, Quaternion.identity);
            }
        }
    }
}