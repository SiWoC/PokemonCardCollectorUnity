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
        private int randomPackPercentage = 0;
        private DateTime lastSave = DateTime.UtcNow;
        public Dictionary<int, Generation> generations = new Dictionary<int, Generation>();
        private int highestUnlockedGeneration = 1;
        public int clickPower = GameManager.coinFactor;
        public Dictionary<int, string[]> packStacks = new Dictionary<int, string[]>();

        [OptionalField(VersionAdded = 2)]
        private Dictionary<int, string> favorites = new Dictionary<int, string>();
        private bool showTutorial = true;
        private Dictionary<TutorialStep, bool> tutorialStepsCompleted;
        // Statistics
        private int statsTotalCoins = 0;
        private int statsRandomPacks = 0;
        private int statsTotalClicks = 0;
        private int statsPacksOpenend = 0;
        private int statsDoublesTradedIn = 0;


        private int[] currentStackIndex = new int[CardFactory.numberOfGenerations + 1];

        static BinaryFormatter formatter = new BinaryFormatter();

        public int GetCoins()
        {
            return this.coins;
        }

        public void AddCoins(int coins)
        {
            this.coins += coins;
            SaveDataTimed(this);
        }

        public void AddCoinClick()
        {
            this.coins += this.clickPower;
            this.statsTotalCoins += this.clickPower;
            this.statsTotalClicks += 1;
            SaveDataTimed(this);
        }

        public int GetRandomPackPercentage()
        {
            return this.randomPackPercentage;
        }

        public void AddRandomPackPercentage(int percentage)
        {
            this.randomPackPercentage += percentage;
            this.statsTotalClicks += 1;
            SaveDataTimed(this);
        }

        public void ResetRandomPackPercentage()
        {
            this.randomPackPercentage = 0;
        }

        public void AddRandomPack(int generation)
        {
            SetPacks(generation, 1);
            this.statsRandomPacks += 1;
            // FOR TESTMODUS COMMENT RESETTING
            ResetRandomPackPercentage();
        }

        public void SetPacks(int generation, int numberOfPacks)
        {
            this.generations[generation].numberOfPacks += numberOfPacks;
            for (int i = 0; i < Math.Abs(numberOfPacks); i++)
            {
                if (numberOfPacks > 0)
                {
                    PushPack(generation);
                }
                else
                {
                    PopPack(generation);
                }
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

        public void UnlockNextGeneration()
        {
            this.highestUnlockedGeneration++;
            this.generations[this.highestUnlockedGeneration].unlocked = true;
        }


        internal int GetHighestUnlockedGeneration()
        {
            return this.highestUnlockedGeneration;
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

        public void SaveDataTimed(PlayerStats playerStats)
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

        public void TradeInDoubles(int generation)
        {
            int numberOfDoubles = 0;
            Dictionary<int, Dictionary<string, PossibleCard>> ownedNPNs = this.generations[generation].cards;
            foreach (Dictionary<string, PossibleCard> ownedCardsOfNpn in ownedNPNs.Values)
            {
                foreach (PossibleCard ownedCard in ownedCardsOfNpn.Values)
                {
                    if (ownedCard.numberOwned > 1)
                    {
                        numberOfDoubles += (ownedCard.numberOwned - 1);
                        ownedCard.numberOwned = 1;
                    }
                }
            }
            this.clickPower += numberOfDoubles;
            this.statsDoublesTradedIn += numberOfDoubles;
        }

        public static PlayerStats LoadData()
        {
            
            FileStream saveFile = File.Open(Application.persistentDataPath + SAVE_FILE, FileMode.Open);
            PlayerStats playerStats = (PlayerStats)formatter.Deserialize(saveFile);
            saveFile.Close();
            playerStats.Initialize();
            return playerStats;
        }

        public void OpenPack(int generation)
        {
            SetPacks(generation, -1);
            this.statsPacksOpenend += 1;
        }

        public int GetAvailablePacks()
        {
            int numberOfPacks = 0;
            foreach (Generation generation in generations.Values)
            {
                numberOfPacks += generation.numberOfPacks;
            }
            return numberOfPacks;
        }

        public int GetAvailablePacks(int generation)
        {
            return generations[generation].numberOfPacks;
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
            if (tutorialStepsCompleted == null)
            {
                showTutorial = true;
                tutorialStepsCompleted = new Dictionary<TutorialStep, bool>();
                // First free pack
                generations[1].numberOfPacks += 1;
                PushPack(1);
            }
            // estimating starting stats
            if (statsTotalCoins < coins)
            {
                statsTotalCoins = coins;
                statsTotalClicks = coins / clickPower;
                statsDoublesTradedIn = clickPower - 100;
                int totalNumberOwned = 0;
                for (int generation = 1; generation <= GetHighestUnlockedGeneration(); generation++)
                {
                    totalNumberOwned += GetNumberOfOwnedCards(generation); 
                }
                statsPacksOpenend = (totalNumberOwned + 9) / 10;
            }

        }

        public string GetFavorite(int nationalPokedexNumber)
        {
            favorites.TryGetValue(nationalPokedexNumber, out string returnValue);
            return returnValue;
        }

        public bool ToggleFavorite(PossibleCard newFavorite)
        {
            favorites.TryGetValue(newFavorite.nationalPokedexNumber, out string favoriteId);
            if (newFavorite.id == favoriteId) // un-favorite
            {
                favorites.Remove(newFavorite.nationalPokedexNumber);
                return false;
            }
            else
            {
                favorites[newFavorite.nationalPokedexNumber] = newFavorite.id;
                return true;
            }
        }

        internal int GetNumberOfOwnedCards(int generation)
        {
            int numberOfOwnedCards = 0;
            foreach (Dictionary<string, PossibleCard> npnCards in generations[generation].cards.Values)
            {
                numberOfOwnedCards += npnCards.Count;
            }
            return numberOfOwnedCards;
        }

        public bool GetTutorialStepCompleted(TutorialStep step)
        {
            tutorialStepsCompleted.TryGetValue(step, out bool completed);
            return completed;
        }

        public void SetTutorialStepCompleted(TutorialStep step)
        {
            tutorialStepsCompleted[step] = true;
        }

        public void SetShowTutorial(bool value)
        {
            this.showTutorial = value;
        }

        public bool GetShowTutorial()
        {
            return this.showTutorial;
        }

        public void RestartTutorial()
        {
            showTutorial = true;
            tutorialStepsCompleted = new Dictionary<TutorialStep, bool>();
        }

        // Statistics
        public int GetStatsTotalCoins()
        {
            return this.statsTotalCoins;
        }

        public int GetStatsRandomPacks()
        {
            return this.statsRandomPacks;
        }

        public int GetStatsTotalClicks()
        {
            return this.statsTotalClicks;
        }

        public int GetStatsPacksOpened()
        {
            return this.statsPacksOpenend;
        }

        public int GetStatsDoublesTradedIn()
        {
            return this.statsDoublesTradedIn;
        }

    } // playerStats
} // namespace
