﻿namespace Marketplace.Paths;

[UsedImplicitly]
[Market_Autoload(Market_Autoload.Type.Both, Market_Autoload.Priority.Init)]
public static class Market_Paths
{
    public static string MainPath => Path.Combine(BepInEx.Paths.ConfigPath, "MarketplaceKG");
    public static string NPC_SoundsPath => Path.Combine(BepInEx.Paths.ConfigPath, "MarketplaceNPC_Sounds");
    public static string CachedImagesFolder => Path.Combine(BepInEx.Paths.ConfigPath, "MarketplaceCachedImages");
    public static string DiscordStuffFolder => Path.Combine(MainPath, "DiscordWebhooks");

    public static string QuestProfilesPath => Path.Combine(MainPath, "QuestsProfiles.cfg");
    public static string QuestDatabasePath => Path.Combine(MainPath, "QuestsDatabase.cfg");
    public static string QuestEventsPath => Path.Combine(MainPath, "QuestsEvents.cfg");

    private static string DataFolder => Path.Combine(MainPath, "DO NOT TOUCH");

    public static string BankerDataJSONFile => Path.Combine(DataFolder, "BankerData.json");
    public static string BankerFile => Path.Combine(MainPath, "BankerProfiles.cfg");

    public static string LoggerPath => Path.Combine(MainPath, "Logger.log");

    public static string ServerMarketDataJSON => Path.Combine(DataFolder, "MarketplaceData.json");
    public static string MarketPlayersIncomeJSON => Path.Combine(DataFolder, "PlayersIncome.json");
    public static string MarketPlayerMessagesJSON => Path.Combine(DataFolder, "PlayerMessages.json");
    public static string MarketLeaderboardJSON => Path.Combine(DataFolder, "LeaderboardV2.json");

    public static string TeleporterPinsFolder => Path.Combine(MainPath, "MapPinsIcons");
    public static string TeleporterConfig => Path.Combine(MainPath, "TeleportHubProfiles.cfg");

    public static string TraderConfig => Path.Combine(MainPath, "TraderProfiles.cfg");

    public static string ServerInfoConfig => Path.Combine(MainPath, "ServerInfoProfiles.cfg");

    public static string TerritoriesConfigPath => Path.Combine(MainPath, "TerritoryDatabase.cfg");

    private static string DistancedUIFolder => Path.Combine(MainPath, "DistancedUI");
    public static string DistancedUIConfig => Path.Combine(DistancedUIFolder, "DistancedUI.cfg");

    public static string GamblerConfig => Path.Combine(MainPath, "GamblerProfiles.cfg");

    public static string BufferProfilesConfig => Path.Combine(MainPath, "BufferProfiles.cfg");
    public static string BufferDatabaseConfig => Path.Combine(MainPath, "BufferDatabase.cfg");

    public static string NpcDialoguesConfig => Path.Combine(MainPath, "NpcDialogues.cfg");

    public static string MainConfig => Path.Combine(MainPath, "MarketPlace.cfg");

    private static string AdditionalConfigsFolder => Path.Combine(MainPath, "AdditionalConfigs");
    public static string AdditionalConfigsQuestsDatabaseConfig => Path.Combine(AdditionalConfigsFolder, "Quests");
    public static string AdditionalConfigsDialoguesFolder => Path.Combine(AdditionalConfigsFolder, "Dialogues");
    public static string AdditionalCondfigsTerritoriesFolder => Path.Combine(AdditionalConfigsFolder, "Territories");
    public static string AdditionalConfigsQuestsProfilesConfig => Path.Combine(AdditionalConfigsFolder, "QuestProfiles");
    public static string AdditionalConfigsQuestsEventsConfig => Path.Combine(AdditionalConfigsFolder, "QuestEvents");
    public static string AdditionalConfigsBankerProfilesConfig => Path.Combine(AdditionalConfigsFolder, "Bankers");
    public static string AdditionalConfigsTeleportHubProfilesConfig => Path.Combine(AdditionalConfigsFolder, "Teleporters");
    public static string AdditionalConfigsTraderProfilesConfig => Path.Combine(AdditionalConfigsFolder, "Traders");
    public static string AdditionalConfigsServerInfoProfilesConfig => Path.Combine(AdditionalConfigsFolder, "ServerInfos");
    public static string AdditionalConfigsGamblerProfilesConfig => Path.Combine(AdditionalConfigsFolder, "Gamblers");
    public static string AdditionalConfigsBufferDatabaseConfig => Path.Combine(AdditionalConfigsFolder, "Buffers");
    public static string AdditionalConfigsBufferProfilesConfig => Path.Combine(AdditionalConfigsFolder, "BufferProfiles");
    public static string AdditionalConfigsTransmogrificationConfig => Path.Combine(AdditionalConfigsFolder, "Transmogrifications");

    public static string PlayerTagsConfig => Path.Combine(MainPath, "PlayerTags.cfg");

    public static string TransmogrificationConfig => Path.Combine(MainPath, "TransmogrificationProfiles.cfg");
    
    
    public static string AchievementsProfiles => Path.Combine(MainPath, "LeaderboardAchievements.cfg");

    private static void OnInit()
    {
        if (Marketplace.WorkingAsType is Marketplace.WorkingAs.Server or Marketplace.WorkingAs.Both)
        {
            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
            if (!Directory.Exists(DiscordStuffFolder))
                Directory.CreateDirectory(DiscordStuffFolder);
            if (!Directory.Exists(TeleporterPinsFolder))
                Directory.CreateDirectory(TeleporterPinsFolder);
            if (!Directory.Exists(DistancedUIFolder))
                Directory.CreateDirectory(DistancedUIFolder);
            if (!Directory.Exists(AdditionalConfigsFolder))
                Directory.CreateDirectory(AdditionalConfigsFolder);
            if (!Directory.Exists(AdditionalConfigsQuestsDatabaseConfig))
                Directory.CreateDirectory(AdditionalConfigsQuestsDatabaseConfig);
            if (!Directory.Exists(AdditionalConfigsDialoguesFolder))
                Directory.CreateDirectory(AdditionalConfigsDialoguesFolder);
            if (!Directory.Exists(AdditionalCondfigsTerritoriesFolder))
                Directory.CreateDirectory(AdditionalCondfigsTerritoriesFolder);
            if (!Directory.Exists(AdditionalConfigsQuestsProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsQuestsProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsQuestsEventsConfig))
                Directory.CreateDirectory(AdditionalConfigsQuestsEventsConfig);
            if (!Directory.Exists(AdditionalConfigsBankerProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsBankerProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsTeleportHubProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsTeleportHubProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsTraderProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsTraderProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsServerInfoProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsServerInfoProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsGamblerProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsGamblerProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsBufferDatabaseConfig))
                Directory.CreateDirectory(AdditionalConfigsBufferDatabaseConfig);
            if (!Directory.Exists(AdditionalConfigsBufferProfilesConfig))
                Directory.CreateDirectory(AdditionalConfigsBufferProfilesConfig);
            if (!Directory.Exists(AdditionalConfigsTransmogrificationConfig))
                Directory.CreateDirectory(AdditionalConfigsTransmogrificationConfig);

            if (!File.Exists(QuestProfilesPath)) File.Create(QuestProfilesPath).Dispose();
            if (!File.Exists(QuestEventsPath)) File.Create(QuestEventsPath).Dispose();
            if (!File.Exists(QuestDatabasePath))
            {
                if (File.Exists(Path.Combine(MainPath, "QuestsDATABASE.cfg")))
                {
                    File.Move(Path.Combine(MainPath, "QuestsDATABASE.cfg"), QuestDatabasePath);
                }
                else
                {
                    File.Create(QuestDatabasePath).Dispose();
                }
            }
            
            if (!File.Exists(BankerFile)) File.Create(BankerFile).Dispose();
            if (!File.Exists(BankerDataJSONFile)) File.Create(BankerDataJSONFile).Dispose();
            if (!File.Exists(TeleporterConfig)) File.Create(TeleporterConfig).Dispose();
            if (!File.Exists(MarketLeaderboardJSON)) File.Create(MarketLeaderboardJSON).Dispose();
            if (!File.Exists(TraderConfig)) File.Create(TraderConfig).Dispose();
            if (!File.Exists(ServerInfoConfig)) File.Create(ServerInfoConfig).Dispose();
            if (!File.Exists(TerritoriesConfigPath)) File.Create(TerritoriesConfigPath).Dispose();
            if (!File.Exists(BufferProfilesConfig)) File.Create(BufferProfilesConfig).Dispose();
            if (!File.Exists(BufferDatabaseConfig))
            {
                if (File.Exists(Path.Combine(MainPath, "BufferDATABASE.cfg")))
                {
                    File.Move(Path.Combine(MainPath, "BufferDATABASE.cfg"), BufferDatabaseConfig);
                }
                else
                {
                    File.Create(BufferDatabaseConfig).Dispose();
                }
            }

            if (!File.Exists(GamblerConfig)) File.Create(GamblerConfig).Dispose();
            if (!File.Exists(NpcDialoguesConfig)) File.Create(NpcDialoguesConfig).Dispose();

            if (!File.Exists(PlayerTagsConfig)) File.Create(PlayerTagsConfig).Dispose();
            if (!File.Exists(TransmogrificationConfig)) File.Create(TransmogrificationConfig).Dispose();
            if (!File.Exists(AchievementsProfiles)) File.Create(AchievementsProfiles).Dispose();
        }

        if (Marketplace.WorkingAsType is Marketplace.WorkingAs.Client or Marketplace.WorkingAs.Both)
        {
            if (!Directory.Exists(NPC_SoundsPath))
                Directory.CreateDirectory(NPC_SoundsPath);
            if (!Directory.Exists(CachedImagesFolder))
                Directory.CreateDirectory(CachedImagesFolder);
        }
    }
}