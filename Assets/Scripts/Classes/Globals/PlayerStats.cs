using Factories;
using Factories.Config;
using Globals.PlayerStatsSegments;
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

        const int CURRENT_VERSION = 1;
        private const string SAVE_FILE = "/save.binary";

        public int version;
        private int coins = 0;
        private int randomPackagePercentage = 0;
        private static DateTime lastSave = DateTime.UtcNow;
        public Dictionary<int, Generation> generations = new Dictionary<int, Generation>();

        private static PlayerStats theInstance;

        static BinaryFormatter formatter = new BinaryFormatter();

        public int Coins
        {
            get => coins; 
            set
            {
                coins = value;
                SaveDataTimed();
            }
        }

        public int RandomPackagePercentage
        {
            get => randomPackagePercentage;
            set
            {
                randomPackagePercentage = value;
                SaveDataTimed();
            }
        }

        private PlayerStats()
        {
            version = CURRENT_VERSION;
        }

        public static PlayerStats GetInstance()
        {

            try
            {
                LoadDataV1();
            }
            catch (System.Exception)
            {
            }
            if (theInstance == null)
            {
                theInstance = new PlayerStats();
            }
            theInstance.InitGenerations();
            return theInstance;
        }

        public static void SaveDataTimed()
        {
            if (lastSave.AddSeconds(5) < DateTime.UtcNow)
            {
                SaveData();
                lastSave = DateTime.UtcNow;
            }
        }

        public static void SaveData()
        {
            FileStream saveFile = File.Create(Application.persistentDataPath + SAVE_FILE);
            formatter.Serialize(saveFile, theInstance);
            saveFile.Close();

        }

        public static void LoadDataV1()
        {
            FileStream saveFile = File.Open(Application.persistentDataPath + SAVE_FILE, FileMode.Open);
            theInstance = (PlayerStats)formatter.Deserialize(saveFile);
            saveFile.Close();
        }

        internal void AddCardToCollection(PossibleCard nowOwnedCard)
        {
            Generation generation = generations[nowOwnedCard.generation];
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

        private void InitGenerations()
        {
            for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
            {
                if (!generations.ContainsKey(i))
                {
                    generations.Add(i, new Generation(i));
                }
            }
        }

    }
}