﻿using Marketplace.Paths;

namespace Marketplace.Modules.Quests;

[UsedImplicitly]
[Market_Autoload(Market_Autoload.Type.Server, Market_Autoload.Priority.Normal, "OnInit",
    new[] { "QP", "QD", "QE" },
    new[] { "OnQuestsProfilesFileChange", "OnQuestDatabaseFileChange", "OnQuestsEventFileChange" })]
public static class Quests_Main_Server
{
    private static void OnInit()
    {
        ReadQuestProfiles();
        ReadQuestDatabase();
        ReadEventDatabase();
    }

    private static void OnQuestsProfilesFileChange()
    {
        ReadQuestProfiles();
        Utils.print("Quests Profiles Changed. Sending new info to all clients");
    }

    private static IEnumerator DelayMore()
    {
        yield return new WaitForSeconds(3);
        ReadQuestDatabase();
        Utils.print("Quests Database Changed. Sending new info to all clients");
    }

    private static void OnQuestDatabaseFileChange()
    {
        Marketplace._thistype.StartCoroutine(DelayMore());
    }

    private static void OnQuestsEventFileChange()
    {
        ReadEventDatabase();
        Utils.print("Quests Events Changed. Sending new info to all clients");
    }

    private static void ProcessQuestProfiles(IReadOnlyList<string> profiles)
    {
        string splitProfile = "default";
        for (int i = 0; i < profiles.Count; ++i)
        {
            if (string.IsNullOrWhiteSpace(profiles[i]) || profiles[i].StartsWith("#")) continue;
            if (profiles[i].StartsWith("["))
            {
                splitProfile = profiles[i].Replace("[", "").Replace("]", "").Replace(" ", "").ToLower();
            }
            else
            {
                string[] split = profiles[i].Replace(" ", "").Split(',').Distinct().ToArray();
                if (!Quests_DataTypes.SyncedQuestProfiles.Value.ContainsKey(splitProfile))
                {
                    Quests_DataTypes.SyncedQuestProfiles.Value.Add(splitProfile, new List<int>());
                }

                foreach (string quest in split)
                {
                    int questToHashcode = quest.ToLower().GetStableHashCode();
                    Quests_DataTypes.SyncedQuestProfiles.Value[splitProfile].Add(questToHashcode);
                }
            }
        }
    }

    private static void ReadQuestProfiles()
    {
        Quests_DataTypes.SyncedQuestProfiles.Value.Clear();
        string folder = Market_Paths.QuestsProfilesFolder;
        string[] files = Directory.GetFiles(folder, "*.cfg", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            IReadOnlyList<string> profiles = File.ReadAllLines(file).ToList();
            ProcessQuestProfiles(profiles);
        }
        Quests_DataTypes.SyncedQuestProfiles.Update();
    }


    private static void ProcessQuestDatabaseProfiles(string fPath, IReadOnlyList<string> profiles)
    {
        if (profiles.Count == 0) return;
        string dbProfile = null;
        Quests_DataTypes.SpecialQuestTag specialQuestTag = Quests_DataTypes.SpecialQuestTag.None;
        for (int i = 0; i < profiles.Count; ++i)
        {
            if (string.IsNullOrWhiteSpace(profiles[i]) || profiles[i].StartsWith("#")) continue;
            if (profiles[i].StartsWith("["))
            {
                dbProfile = profiles[i].Replace("[", "").Replace("]", "").Replace(" ", "").ToLower();
                string[] checkTags = dbProfile.Split('=');
                if (checkTags.Length == 2)
                {
                    if (!Enum.TryParse(checkTags[1], true, out Quests_DataTypes.SpecialQuestTag tag)) continue;
                    specialQuestTag |= tag;
                    dbProfile = checkTags[0];
                }
                else
                {
                    specialQuestTag = Quests_DataTypes.SpecialQuestTag.None;
                }
            }
            else
            {
                if (dbProfile == null) continue;
                try
                {
                    int UID = dbProfile.GetStableHashCode();
                    string typeString = profiles[i];
                    string name = profiles[i + 1];
                    string image = "";
                    int imageIndex = name.IndexOf("<image=", StringComparison.Ordinal);
                    if (imageIndex != -1)
                    {
                        int imageEndIndex = name.IndexOf(">", imageIndex, StringComparison.Ordinal);
                        if (imageEndIndex != -1)
                        {
                            image = name.Substring(imageIndex + 7, imageEndIndex - imageIndex - 7);
                            name = name.Remove(imageIndex, imageEndIndex - imageIndex + 1);
                        }
                    }

                    string description = profiles[i + 2];
                    string target = profiles[i + 3];
                    string reward = profiles[i + 4];
                    
                    string[] cooldownsString = profiles[i + 5].Replace(" ","").Split(',');
                    string cooldown = cooldownsString[0];
                    string timeLimit = cooldownsString.Length > 1 ? cooldownsString[1] : "0";
                    string restrictions = profiles[i + 6];
                    if (!(Enum.TryParse(typeString, true, out Quests_DataTypes.QuestType type) &&
                          Enum.IsDefined(typeof(Quests_DataTypes.QuestType), type)))
                    {
                        dbProfile = null;
                        continue;
                    }
                    string[] rewardsArray = reward.Replace(" ", "").Split('|');
                    int _RewardsAMOUNT = Mathf.Max(1, rewardsArray.Length);
                    Quests_DataTypes.QuestRewardType[] rewardTypes =
                        new Quests_DataTypes.QuestRewardType[_RewardsAMOUNT];
                    string[] RewardPrefabs = new string[_RewardsAMOUNT];
                    int[] RewardLevels = new int[_RewardsAMOUNT];
                    int[] RewardCounts = new int[_RewardsAMOUNT];
                    for (int r = 0; r < _RewardsAMOUNT; ++r)
                    {
                        string[] rwdTypeCheck = rewardsArray[r].Split(':');
                        if (!(Enum.TryParse(rwdTypeCheck[0], true,
                                out rewardTypes[r])))
                        {
                            Utils.print($"Failed to parse reward type {rewardsArray[r]} in quest {name} (File: {fPath}). Skipping quest");
                            continue;
                        }

                        string[] RewardSplit = rwdTypeCheck[1].Split(',');

                        if (rewardTypes[r] 
                            is Quests_DataTypes.QuestRewardType.EpicMMO_EXP
                            or Quests_DataTypes.QuestRewardType.MH_EXP 
                            or Quests_DataTypes.QuestRewardType.Cozyheim_EXP
                            or Quests_DataTypes.QuestRewardType.GuildAddLevel)
                        {
                            RewardPrefabs[r] = "NONE";
                            RewardCounts[r] = int.Parse(RewardSplit[0]);
                            RewardLevels[r] = 1;
                        }
                        else
                        {
                            RewardPrefabs[r] = RewardSplit[0];
                            RewardCounts[r] = int.Parse(RewardSplit[1]);
                            RewardLevels[r] = 1;
                        }

                        if (RewardSplit.Length >= 3)
                        {
                            RewardLevels[r] = Convert.ToInt32(RewardSplit[2]);
                            if (rewardTypes[r] == Quests_DataTypes.QuestRewardType.Pet) ++RewardLevels[r];
                        }
                    }


                    if (type is not Quests_DataTypes.QuestType.Talk)
                    {
                        target = target.Replace(" ", "");
                    }

                    string[] targetsArray = target.Split('|');
                    int _targetsCount = Mathf.Max(1, targetsArray.Length);
                    string[] TargetPrefabs = new string[_targetsCount];
                    int[] TargetLevels = new int[_targetsCount];
                    int[] TargetCounts = new int[_targetsCount];
                    for (int t = 0; t < _targetsCount; ++t)
                    {
                        string[] TargetSplit = targetsArray[t].Split(',');
                        TargetPrefabs[t] = TargetSplit[0];
                        TargetLevels[t] = 1;
                        TargetCounts[t] = 1;

                        if (type is Quests_DataTypes.QuestType.Kill && TargetSplit.Length >= 3)
                        {
                            TargetLevels[t] = Mathf.Max(1, Convert.ToInt32(TargetSplit[2]) + 1);
                        }

                        if (type is Quests_DataTypes.QuestType.Craft or Quests_DataTypes.QuestType.Collect &&
                            TargetSplit.Length >= 3)
                        {
                            TargetLevels[t] = Mathf.Max(1, Convert.ToInt32(TargetSplit[2]));
                        }

                        if (type != Quests_DataTypes.QuestType.Talk)
                        {
                            TargetCounts[t] = Mathf.Max(1, int.Parse(TargetSplit[1]));
                        }
                    }
                    string[] Conditions = restrictions.ReplaceSpacesOutsideQuotes().Split('|');

                    Quests_DataTypes.Quest quest = new()
                    {
                        RewardsAmount = _RewardsAMOUNT,
                        Type = type,
                        RewardType = rewardTypes,
                        Name = name,
                        Description = description,
                        TargetAmount = _targetsCount,
                        TargetPrefab = TargetPrefabs,
                        TargetCount = TargetCounts,
                        TargetLevel = TargetLevels,
                        RewardPrefab = RewardPrefabs,
                        RewardCount = RewardCounts,
                        Cooldown = int.Parse(cooldown),
                        RewardLevel = RewardLevels,
                        SpecialTag = specialQuestTag,
                        PreviewImage = image,
                        TimeLimit = int.Parse(timeLimit),
                        Conditions = Conditions
                    };
                    if (!Quests_DataTypes.SyncedQuestData.Value.ContainsKey(UID))
                        Quests_DataTypes.SyncedQuestData.Value.Add(UID, quest);
                }
                catch (Exception ex)
                {
                    Utils.print($"Error in Quests {fPath} {dbProfile}\n{ex}", ConsoleColor.Red);
                }
                dbProfile = null;
            }
        }
    }
    

    private static void ReadQuestDatabase()
    {
        Quests_DataTypes.SyncedQuestRevision.Value = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Quests_DataTypes.SyncedQuestData.Value.Clear();
        string folder = Market_Paths.QuestsDatabaseFolder;
        string[] files = Directory.GetFiles(folder, "*.cfg", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            IReadOnlyList<string> profiles = File.ReadAllLines(file).ToList();
            ProcessQuestDatabaseProfiles(file, profiles);
        }

        Quests_DataTypes.SyncedQuestData.Update();
        
    }

    private static void ProcessEventDatabase(IReadOnlyList<string> profiles)
    {
        bool ValidateEventArguments(Quests_DataTypes.QuestEventAction action, string args, string QuestID)
        {
            string[] split = args.Split(',');
            int count = split.Length;
            bool result = action switch
            {
                Quests_DataTypes.QuestEventAction.GiveItem => count == 3,
                Quests_DataTypes.QuestEventAction.GiveQuest => count == 1,
                Quests_DataTypes.QuestEventAction.Spawn => count == 3,
                Quests_DataTypes.QuestEventAction.Teleport => count == 3,
                Quests_DataTypes.QuestEventAction.Damage => count == 1,
                Quests_DataTypes.QuestEventAction.Heal => count == 1,
                Quests_DataTypes.QuestEventAction.RemoveQuest => count == 1,
                Quests_DataTypes.QuestEventAction.PlaySound => count == 1,
                Quests_DataTypes.QuestEventAction.NpcText => count >= 1,
                _ => false
            };
            if (!result)
            {
                Utils.print($"Arguments for Quest Event: {QuestID} => action {action} are invalid: {args}",
                    ConsoleColor.Red);
            }

            return result;
        }
        
        string splitProfile = "default";
        for (int i = 0; i < profiles.Count; ++i)
        {
            if (string.IsNullOrWhiteSpace(profiles[i]) || profiles[i].StartsWith("#")) continue;
            if (profiles[i].StartsWith("["))
            {
                splitProfile = profiles[i].Replace("[", "").Replace("]", "").ToLower();
            }
            else
            {
                string[] split = profiles[i].Split(':');
                if (split.Length < 2) continue;
                string key = split[0];
                string value = split[1];
                if (!Enum.TryParse(key, out Quests_DataTypes.QuestEventCondition cond) ||
                    !Enum.IsDefined(typeof(Quests_DataTypes.QuestEventCondition), cond)) continue;

                string[] actionSplit = value.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (!Enum.TryParse(actionSplit[0], true, out Quests_DataTypes.QuestEventAction action) ||
                    !Enum.IsDefined(typeof(Quests_DataTypes.QuestEventAction), action)) continue;

                string args = actionSplit.Length > 1 ? actionSplit[1] : string.Empty;
                if (action is not Quests_DataTypes.QuestEventAction.NpcText) args = args.Replace(" ", "");

                if (!ValidateEventArguments(action, args, splitProfile)) continue;
                if (Quests_DataTypes.SyncedQuestsEvents.Value.ContainsKey(splitProfile.GetStableHashCode()))
                {
                    Quests_DataTypes.SyncedQuestsEvents.Value[splitProfile.GetStableHashCode()]
                        .Add(new Quests_DataTypes.QuestEvent(cond, action, args));
                }
                else
                {
                    Quests_DataTypes.SyncedQuestsEvents.Value.Add(splitProfile.GetStableHashCode(),
                        new List<Quests_DataTypes.QuestEvent> { new(cond, action, args) });
                }
            }
        }
    }
    
    private static void ReadEventDatabase()
    {
        Quests_DataTypes.SyncedQuestsEvents.Value.Clear();
        string folder = Market_Paths.QuestsEventsFolder;
        string[] files = Directory.GetFiles(folder, "*.cfg", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            IReadOnlyList<string> events = File.ReadAllLines(file).ToList();
            ProcessEventDatabase(events);
        }
        Quests_DataTypes.SyncedQuestsEvents.Update();
        
    }
}