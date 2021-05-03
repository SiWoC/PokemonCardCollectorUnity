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

        internal static void AddRandomPack()
        {
            // TODO add random pack
            PlayerStats.SaveData();
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

        public static void ChoreClick()
        {
            if (earnType == EarnType.Coins)
            {
                PlayerStats.AddCoins(1);
            } else
            {
                PlayerStats.AddRandomPackPercentage(1);
            }
        }

        public static void BuyPacks(int generation)
        {
            if (PlayerStats.GetCoins() < (selectedMultiplier * price[generation])) return; // not enough money?? Bug? Hack?
            PlayerStats.AddCoins(-1 * selectedMultiplier * price[generation]);  // Coins setter saves timed
            PlayerStats.SetPacks(generation, selectedMultiplier); // SetPacks saves always
        }

        public static void OpenedPack(int generation)
        {
            PlayerStats.SetPacks(generation, -1); // SetPacks saves always
        }

        public static void AddCardToCollection(PossibleCard possibleCard)
        {
            PlayerStats.AddCardToCollection(possibleCard);
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