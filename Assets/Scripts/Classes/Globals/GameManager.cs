using Factories.Config;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{

    public enum EarnType
    {
        Coins,
        Packs
    }

    public static class GameManager
    {
        private static int selectedGeneration = 1;
        public static int selectedNPN = 1;
        public static EarnType earnType = EarnType.Coins;

        public readonly static PlayerStats playerStats = PlayerStats.GetInstance();

        private static int selectedMultiplier = 1;
        public readonly static int[] price = { 0, 370, 740, 1110, 1480, 1850, 2220, 2590, 2960 };

        private static string[] sceneNamesStack = new string[10];
        private static int currentStackIndex = 0;
        private static bool initialized = false;

        public static int SelectedGeneration
        {
            get => selectedGeneration;
            set
            {
                selectedGeneration = value;
                PlayerPrefs.SetInt("SelectedGeneration", value);
                PlayerPrefs.Save();
            }
        }

        public static int SelectedMultiplier
        {
            get => selectedMultiplier;
            set
            {
                selectedMultiplier = value;
                PlayerPrefs.SetInt("SelectedMultiplier", value);
                PlayerPrefs.Save();
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
                UnityEngine.Object initializerPrefab = Resources.Load("Initializer");
                //GameObject initializer = (GameObject)
                    GameObject.Instantiate(initializerPrefab);
                selectedGeneration = Math.Max(1,PlayerPrefs.GetInt("SelectedGeneration"));
                selectedMultiplier = Math.Max(1, PlayerPrefs.GetInt("SelectedMultiplier"));
                initialized = true;
                //GameObject.Destroy(initializer);
            }
        }
        public static int GetCoins()
        {
            return playerStats.Coins;
        }

        public static float GetRandomPackPercentage()
        {
            return playerStats.RandomPackagePercentage;
        }

        internal static void AddRandomPack()
        {
            // TODO add randomPack
            playerStats.RandomPackagePercentage = 0;
        }

        public static void ChoreClick()
        {
            if (earnType == EarnType.Coins)
            {
                AddCoins(1);
            } else
            {
                AddRandomPackPercentage(1);
            }
        }

        public static void AddCoins(int coins)
        {
            playerStats.Coins += coins;
        }

        public static void AddRandomPackPercentage(int percentage)
        {
            playerStats.RandomPackagePercentage += percentage;
        }

        public static void BuyPack(int generation)
        {
            if (playerStats.Coins < (selectedMultiplier * price[generation])) return; // not enough money?? Bug? Hack?
            playerStats.Coins -= (selectedMultiplier * price[generation]);  // Coins setter saves timed
            playerStats.SetPacks(generation, selectedMultiplier); // SetPacks saves always
        }

        public static void AddCardToCollection(PossibleCard possibleCard)
        {
            playerStats.AddCardToCollection(possibleCard);
        }

        public static void Back()
        {

            PlayerStats.SaveData();
            if (currentStackIndex == 0)
            {
                Debug.Log("Quitting");
                Application.Quit();
            } else
            {
                currentStackIndex--;
                SceneManager.LoadScene(sceneNamesStack[currentStackIndex]);
                //SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneNamesStack[currentStackIndex]));
            }
        }

        public static void Forward(string sceneName)
        {
            sceneNamesStack[currentStackIndex] = SceneManager.GetActiveScene().path;
            currentStackIndex++;
            SceneManager.LoadScene(sceneName);
        }

    }
}