using Factories;
using Factories.Config;
using System;
using System.Collections.Generic;
using System.IO;
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

        public PlayerStats()
        {
            version = CURRENT_VERSION;
            InitGenerations();
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
            return playerStats;
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