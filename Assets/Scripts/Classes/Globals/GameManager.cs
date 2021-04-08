﻿using Factories.Config;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{
    public static class GameManager
    {
        public readonly static PlayerStats playerStats = PlayerStats.GetInstance();
        private static string[] sceneNamesStack = new string[10];
        private static int currentStackIndex = 0;

        public static int GetCoins()
        {
            return playerStats.Coins;
        }

        public static void AddCoins(int coins)
        {
            playerStats.Coins = playerStats.Coins + coins;
        }

        public static void AddCardToCollection(PossibleCard possibleCard)
        {
            playerStats.AddCardToCollection(possibleCard);
        }

        public static void Back()
        {

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