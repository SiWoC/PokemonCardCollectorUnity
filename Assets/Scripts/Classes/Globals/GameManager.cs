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

        public static void Initialize()
        {
            if (!initialized)
            {
                UnityEngine.Object initializerPrefab = Resources.Load("Initializer");
                GameObject.Instantiate(initializerPrefab);
                selectedGeneration = PlayerPrefs.GetInt("SelectedGeneration");
                initialized = true;
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