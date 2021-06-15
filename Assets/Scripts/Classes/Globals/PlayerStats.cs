using Factories;
using Factories.Config;
using Globals.PlayerStatsSegments.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Globals
{
    [Serializable]
    public class PlayerStats
    {

        private static PlayerStatsSegments.V1.PlayerStats theInstance;

        public static void AddCoins(int coins)
        {
            theInstance.Coins += coins;
        }

        public static void AddClick()
        {
            theInstance.Coins += theInstance.clickPower;
        }

        public static void AddRandomPackPercentage(int percentage)
        {
            theInstance.RandomPackagePercentage += percentage;
        }

        public static int GetCoins()
        {
            return theInstance.Coins; 
        }

        internal static string GetNextPack(int generation)
        {
            return theInstance.GetNextPack(generation);
        }

        public int RandomPackagePercentage
        {
            get => theInstance.RandomPackagePercentage;
            set
            {
                theInstance.RandomPackagePercentage = value;
            }
        }

        public static int GetRandomPackPercentage()
        {
            return theInstance.RandomPackagePercentage;
        }

        static PlayerStats()
        {
            // try load current version
            try
            {
                theInstance = PlayerStatsSegments.V1.PlayerStats.LoadData();
            }
            catch (System.Exception)
            {
            }
            if (theInstance == null)
            {
                theInstance = new PlayerStatsSegments.V1.PlayerStats();
            }
        }

        public static void AddCardToCollection(PossibleCard nowOwnedCard)
        {
            Generation generation = theInstance.generations[nowOwnedCard.generation];
            // anything for this nationalPokedexNumber yet?
            if (!generation.cards.ContainsKey(nowOwnedCard.nationalPokedexNumber))
            {
                generation.cards.Add(nowOwnedCard.nationalPokedexNumber, new Dictionary<string, PossibleCard>());
            }

            // do we have this card already?
            if (!generation.cards[nowOwnedCard.nationalPokedexNumber].ContainsKey(nowOwnedCard.id))
            {
                generation.cards[nowOwnedCard.nationalPokedexNumber].Add(nowOwnedCard.id, nowOwnedCard);
            }
            generation.cards[nowOwnedCard.nationalPokedexNumber][nowOwnedCard.id].numberOwned += 1;
            //SaveData();

        }

        internal static Generation GetGeneration(int generation)
        {
            return theInstance.generations[generation];
        }

        public static void SetPacks(int generation, int numberOfPacks)
        {
            theInstance.generations[generation].numberOfPacks += numberOfPacks;
            for (int i = 0; i < Math.Abs(numberOfPacks); i++)
            {
                if (numberOfPacks > 0)
                {
                    theInstance.PushPack(generation);
                } else
                {
                    theInstance.PopPack(generation);
                }
            }
            SaveData();
        }

        public static void SaveData()
        {
            PlayerStatsSegments.V1.PlayerStats.SaveData(theInstance);
        }

        public static int GetAvailablePacks(int generation)
        {
            return theInstance.generations[generation].numberOfPacks;
        }

        public static bool IsGenerationUnlocked(int generation)
        {
            return theInstance.generations[generation].unlocked;
        }

        public static int GetHighestUnlockedGeneration()
        {
            return theInstance.highestUnlockedGeneration;
        }

        internal static void UnlockNextGeneration()
        {
            theInstance.highestUnlockedGeneration++;
            theInstance.generations[theInstance.highestUnlockedGeneration].unlocked = true;
        }
    }
}