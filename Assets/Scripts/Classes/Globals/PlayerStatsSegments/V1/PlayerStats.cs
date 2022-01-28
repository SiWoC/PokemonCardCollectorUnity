using Assets.Scripts.Classes.Globals;
using Factories;
using Factories.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Globals.PlayerStatsSegments.V1
{
    [Serializable]
    public class PlayerStats
    {

        const int CURRENT_VERSION = 1;
        private const string SAVE_FILE = "/save.v1.binary";

        public int version;
        private int coins = 0;
        private int randomPackagePercentage = 0;
        private static DateTime lastSave = DateTime.UtcNow;
        public Dictionary<int, Generation> generations = new Dictionary<int, Generation>();
        public int highestUnlockedGeneration = 1;
        public int clickPower = GameManager.coinFactor;
        public Dictionary<int, string[]> packStacks = new Dictionary<int, string[]>();
        [OptionalField(VersionAdded = 2)]
        public Dictionary<int, string> favorites = new Dictionary<int, string>();
        public Dictionary<TutorialStep, bool> tutorialStepsCompleted = new Dictionary<TutorialStep, bool>();

        private int[] currentStackIndex = new int[CardFactory.numberOfGenerations + 1];

        static BinaryFormatter formatter = new BinaryFormatter();

        public int Coins
        {
            get => coins;
            set
            {
                coins = value;
                SaveDataTimed(this);
            }
        }

        public int RandomPackagePercentage
        {
            get => randomPackagePercentage;
            set
            {
                randomPackagePercentage = value;
                SaveDataTimed(this);
            }
        }

        public void PushPack(int generation)
        {
            packStacks[generation][currentStackIndex[generation]] = CardFactory.GeneratePackArtName(generation);
            currentStackIndex[generation]++;
        }

        public string GetNextPack(int generation)
        {
            CheckPackStack(generation);
            return packStacks[generation][currentStackIndex[generation] - 1].Replace("small", "basic").Replace("big", "stage2");
        }

        public void PopPack(int generation)
        {
            CheckPackStack(generation);
            currentStackIndex[generation]--;
            packStacks[generation][currentStackIndex[generation]] = null;
        }

        private void CheckPackStack(int generation)
        {
            if (currentStackIndex[generation] == 0)
            {
                PushPack(generation);
            }
            else if (packStacks[generation][currentStackIndex[generation] - 1] == null)
            {
                packStacks[generation][currentStackIndex[generation] - 1] = CardFactory.GeneratePackArtName(generation);
            }
        }

        public PlayerStats()
        {
            version = CURRENT_VERSION;
            Initialize();
        }

        public static void SaveDataTimed(PlayerStats playerStats)
        {
            if (lastSave.AddSeconds(5) < DateTime.UtcNow)
            {
                SaveData(playerStats);
                lastSave = DateTime.UtcNow;
            }
        }

        public static void SaveData(PlayerStats playerStats)
        {
            FileStream saveFile = File.Create(Application.persistentDataPath + SAVE_FILE);
            formatter.Serialize(saveFile, playerStats);
            saveFile.Close();

        }

        public static PlayerStats LoadData()
        {
            
            FileStream saveFile = File.Open(Application.persistentDataPath + SAVE_FILE, FileMode.Open);
            PlayerStats playerStats = (PlayerStats)formatter.Deserialize(saveFile);
            saveFile.Close();
            playerStats.Initialize();
            return playerStats;
        }

        private void Initialize()
        {

            for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
            {
                if (!generations.ContainsKey(i))
                {
                    generations.Add(i, new Generation(i));
                }
                if (i > highestUnlockedGeneration && generations[i].unlocked)
                {
                    highestUnlockedGeneration = i;
                }
                if (!generations[i].unlocked)
                {
                    generations[i].numberOfPacks = 0;
                }

                currentStackIndex[i] = 0;
                if (packStacks.ContainsKey(i))
                {
                    bool nullFound = false;
                    for (int j = 0; j < 100; j++)
                    {
                        // already found null, make sure rest of stack is null too
                        if (nullFound)
                        {
                            packStacks[i][j] = null;
                        }
                        else
                        {
                            if (packStacks[i][j] == null)
                            {
                                nullFound = true;
                            } else
                            {
                                currentStackIndex[i] = j + 1;
                            }
                        }
                    }
                } else
                {
                    packStacks.Add(i, new string[100]);
                }
            }
            generations[1].unlocked = true;

            if (clickPower < GameManager.coinFactor)
            {
                clickPower = GameManager.coinFactor;
            }
            if (favorites == null)
            {
                favorites = new Dictionary<int, string>();
            }
            if (tutorialStepsCompleted == null || true) // debug
            {
                tutorialStepsCompleted = new Dictionary<TutorialStep, bool>();
            }

        }

    } // playerStats
} // namespace
