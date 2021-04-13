using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Factories.Config;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Globals;

namespace Factories
{
    public enum ImageType
    {
        Small,
        Large
    }
    
    public static class CardFactory
    {
        public const int numberOfGenerations = 8;
        public static int[] numberOfCardsInGeneration = new int[numberOfGenerations + 1] { 0, 151, 100, 135, 107, 156, 72, 88, 89 };
        public static int[] startNPNOfGeneration = new int[numberOfGenerations + 1] { 0, 1, 152, 252, 387, 494, 650, 722, 810 };

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
            /* debugging, give me all 97 normal or 17 special Pikachu cards
            foreach (PossibleCard pc in pcl.possibleCard)
            {
                if (pc.nationalPokedexNumber == 25)
                {
                    GameManager.AddCardToCollection(pc);
                }
            }
            */
            int index = UnityEngine.Random.Range(0, pcl.possibleCard.Length);
            PossibleCard chosenCard = pcl.possibleCard[index];
            Debug.Log("NPN " + chosenCard.nationalPokedexNumber);
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

        public static IEnumerator FillImage(ImageType type, string url, Image image)
        {
            Sprite sprite = CacheManager.GetSprite(type, url);
            if (sprite == null)
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    DownloadHandler handle = request.downloadHandler;
                    Texture2D texture = new Texture2D(5, 5);
                    if (texture.LoadImage(handle.data))
                    {
                        // sprite will be scaled by spriteRenderer.Drawmode = sliced
                        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    }
                    image.sprite = sprite;
                    CacheManager.PutSprite(sprite, type, url);
                }
            } else
            {
                image.sprite = sprite;
            }
        }


    }
}