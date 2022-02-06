using Factories;
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
       
        public static event Action GenerationUnlockedEvent;
        public delegate void RandomPackEarnedHandler(int generation);
        public static event RandomPackEarnedHandler RandomPackEarnedEvent;

        private static int selectedGeneration = 1;
        public static int selectedNPN = 1;
        public static EarnType earnType = EarnType.Coins;

        private static int selectedMultiplier = 1;
        public static readonly float basePrice = 370f;
        public static readonly int percentageFactor = 100000; // 100.000
        public static readonly int coinFactor = 100; // 1.00
        private static readonly float priceIncreaseFactor = 1.8f;

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

        public static int GetPriceInCents(int generation)
        {
            // rounded down to whole Pokedollar then to cents
            return (int)(basePrice * Math.Pow(priceIncreaseFactor, generation - 1)) * coinFactor;
        }

        private static int AddRandomPack()
        {
            int generation = UnityEngine.Random.Range(1,PlayerStats.GetHighestUnlockedGeneration() + 1);
            PlayerStats.SetPacks(generation, 1); // SetPacks saves always
            // FOR TESTMODUS COMMENT RESETTING
            PlayerStats.ResetRandomPackPercentage();
            return generation;
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
                PlayerStats.AddClick();
            } else
            {
                PlayerStats.AddRandomPackPercentage(CalculateRandomPackclick());
                if (PlayerStats.GetRandomPackPercentage() >= percentageFactor)
                {
                    int generation = AddRandomPack();
                    RandomPackEarnedEvent?.Invoke(generation);
                }
            }
        }

        private static int CalculateRandomPackclick()
        {
            // for a random package you need the same clicks as for the average of the unlocked packages
            // gen1 unlocked > 370 clicks for 100% > randomPackClick = 270 = 0.270%
            // gen2 unlocked > (370 + 665) / 2 = 517 clicks for 100% > randomPackClick = 193 = 0.193%
            int totalPrice = 0;
            for (int generation = 1; generation <= PlayerStats.GetHighestUnlockedGeneration(); generation++)
            {
                totalPrice += GetPriceInCents(generation) / coinFactor;
            }
            int average = totalPrice / PlayerStats.GetHighestUnlockedGeneration();
            /*
             *  click = 1 /370
             *  click = click * percentageFactor
             *  same as
             *  click = percentageFactor / 370
             */
            //Debug.Log("randomPackClick: " + percentageFactor / average);
            return percentageFactor / average;
        }

        public static void BuyPacks(int generation)
        {
            if (PlayerStats.GetCoins() < (selectedMultiplier * GetPriceInCents(generation))) return; // not enough money?? Bug? Hack?
            PlayerStats.AddCoins(-1 * selectedMultiplier * GetPriceInCents(generation));  // Coins setter saves timed
            PlayerStats.SetPacks(generation, selectedMultiplier); // SetPacks saves always
        }

        public static void OpenedPack(int generation, System.Collections.Generic.List<PossibleCard> cardInThisPack)
        {
            PlayerStats.SetPacks(generation, -1); // SetPacks saves always
            foreach (PossibleCard card in cardInThisPack)
            {
                AddCardToCollection(card);
            }
            if (generation == PlayerStats.GetHighestUnlockedGeneration())
            {
                if (PlayerStats.GetGeneration(generation).cards.Count > (CardFactory.numberOfNPNsInGeneration[generation] * 2/3))
                {
                    PlayerStats.UnlockNextGeneration();
                    GenerationUnlockedEvent?.Invoke();
                }
            }
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