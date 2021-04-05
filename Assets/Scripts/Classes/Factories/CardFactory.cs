using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Factories.Config;
using System.Text.RegularExpressions;

namespace Factories
{
    public static class CardFactory
    {
        public const int numberOfGenerations = 8;

        private const string SQUARE_SETS = "^base|^dp|^ecard|^ex|^gym|^neo|^np|^pl|^pop|^si";

        public static GameObject cardPrefab;
        public static Sprite roundedBack;
        public static Sprite squareBack;

        private static Dictionary<string, PossibleCardList> cardSets = new Dictionary<string, PossibleCardList>();

        static CardFactory()
        {
            for (int i = 1; i <= numberOfGenerations; i++)
            {
                AddCardsFromFile("gen" + i + "-special");
                AddCardsFromFile("gen" + i + "-normal");
            }
        }

        public static void AddCardsFromFile(string cardResourceName) {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("Factories/Config/" + cardResourceName);
            PossibleCardList pcl = JsonUtility.FromJson<PossibleCardList>(jsonTextFile.text);
            cardSets.Add(cardResourceName, pcl);
        }

        public static IEnumerator GetCard(string generation, string rarity, Action<GameObject> returnToCaller)
        {
            string cardResourceName = (generation + "-" + rarity).Replace(" ", "").ToLower();
            PossibleCardList pcl = cardSets[cardResourceName];
            int index = UnityEngine.Random.Range(0, pcl.possibleCard.Length);
            PossibleCard chosenCard = pcl.possibleCard[index];
            GameObject cardInstance = GameObject.Instantiate(cardPrefab);
            cardInstance.SetActive(false);
            Card card = cardInstance.GetComponent<Card>();
            card.createdFrom = chosenCard;
            Match matcher = Regex.Match(chosenCard.setCode, SQUARE_SETS, RegexOptions.IgnoreCase);

            if (matcher.Success)
            {
                card.back = squareBack;
            }
            else
            {
                card.back = roundedBack;
            }
            return DownloadImage(chosenCard.imageUrlLarge, cardInstance, returnToCaller);
        }

        public static IEnumerator CreateCard(PossibleCard someCard, Action<GameObject> returnToCaller)
        {
            GameObject cardInstance = GameObject.Instantiate(cardPrefab);
            cardInstance.SetActive(false);
            Card card = cardInstance.GetComponent<Card>();
            card.createdFrom = someCard;
            Match matcher = Regex.Match(someCard.setCode, SQUARE_SETS, RegexOptions.IgnoreCase);
            if (matcher.Success)
            {
                card.back = squareBack;
            }
            else
            {
                card.back = roundedBack;
            }
            return DownloadImage(someCard.imageUrlLarge, cardInstance, returnToCaller);
        }

        static IEnumerator DownloadImage(string mediaUrl, GameObject cardInstance, Action<GameObject> returnToCaller)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Card card = cardInstance.GetComponent<Card>();
                DownloadHandler handle = request.downloadHandler;
                Texture2D texure = new Texture2D(5, 5);
                Sprite sprite = null;
                if (texure.LoadImage(handle.data))
                {
                    // sprite will be scaled by spriteRenderer.Drawmode = sliced
                    sprite = Sprite.Create(texure, new Rect(0, 0, texure.width, texure.height), new Vector2(0.5f, 0.5f));
                }
                card.front = sprite;
                returnToCaller(cardInstance);
            }
        }
    }
}