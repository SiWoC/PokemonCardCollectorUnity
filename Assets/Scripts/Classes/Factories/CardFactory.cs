using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Factories.Config;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Globals;
using UnityEngine.Events;

namespace Factories
{
    public enum ImageType
    {
        Small,
        Large
    }
    
    public static class CardFactory
    {
        public static event Action packReadyEvent;

        public const int numberOfGenerations = 8;
        public static int[] numberOfCardsInGeneration = new int[numberOfGenerations + 1] { 0, 151, 100, 135, 107, 156, 72, 88, 89 };
        public static int[] startNPNOfGeneration = new int[numberOfGenerations + 1] { 0, 1, 152, 252, 387, 494, 650, 722, 810 };

        private const string SQUARE_SETS = "^base|^dp|^ecard|^ex|^gym|^neo|^np|^pl|^pop|^si";

        public static GameObject packPrefab;
        public static GameObject cardPrefab;
        public static Sprite roundedBack;
        public static Sprite squareBack;

        private static int cardsInPackStillLoading = 10;

        private static Dictionary<string, PossibleCardList> cardSets = new Dictionary<string, PossibleCardList>();
        private static Dictionary<int, Dictionary<int, int>> availableNPNs = new Dictionary<int, Dictionary<int,int>>();

        static CardFactory()
        {
            for (int generation = 1; generation <= numberOfGenerations; generation++)
            {
                availableNPNs.Add(generation, new Dictionary<int, int>());
                AddCardsFromFile("gen" + generation + "-special", generation);
                AddCardsFromFile("gen" + generation + "-normal", generation);
            }
        }

        public static void AddCardsFromFile(string cardResourceName, int generation) {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("Factories/Config/" + cardResourceName);
            PossibleCardList pcl = JsonUtility.FromJson<PossibleCardList>(jsonTextFile.text);
            foreach (PossibleCard pc in pcl.possibleCard)
            {
                if (!availableNPNs[generation].ContainsKey(pc.nationalPokedexNumber))
                {
                    availableNPNs[generation].Add(pc.nationalPokedexNumber,0);
                }
                availableNPNs[generation][pc.nationalPokedexNumber] += 1;
            }
            cardSets.Add(cardResourceName, pcl);
        }

        public static GameObject GetPack(int generation)
        {
            cardsInPackStillLoading = 10;
            GameObject pack = GameObject.Instantiate(packPrefab);
            GameObject wrapper = pack.transform.Find("PackWrapper").gameObject;
            wrapper.GetComponent<BoxCollider2D>().enabled = false;
            Transform normalCardsHolder = pack.transform.Find("PackContent").Find("NormalCards");
            for (int i = 1; i < 9; i++)
            {
                GameObject cardInstance = CreateCard(generation, normalCardsHolder, "normal");
                cardInstance.transform.localPosition = new Vector3(0f, 0f, (10.0f + i) / 50.0f);
                SpriteRenderer sr = cardInstance.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 10 + i;

            }


            Transform specialCardsHolder = pack.transform.Find("PackContent").Find("SpecialCards");
            for (int i = 1; i < 3; i++)
            {
                GameObject cardInstance = CreateCard(generation, specialCardsHolder, "special");
                cardInstance.transform.localPosition = new Vector3(0f, 0f, (10.0f + i) / 50.0f);
                SpriteRenderer sr = cardInstance.GetComponent<SpriteRenderer>();
                sr.sortingOrder = i;

            }
            return pack;
        }

        private static GameObject CreateCard(int generation, Transform normalCardsHolder, string rarity)
        {
            string cardResourceName = ("gen" + generation + "-" + rarity).Replace(" ", "").ToLower();
            PossibleCardList pcl = cardSets[cardResourceName];
            int index = UnityEngine.Random.Range(0, pcl.possibleCard.Length);
            PossibleCard chosenCard = pcl.possibleCard[index];
            Debug.Log("NPN " + chosenCard.nationalPokedexNumber);
            GameObject cardInstance = GameObject.Instantiate(cardPrefab);
            Card card = cardInstance.GetComponent<Card>();
            card.CreatedFrom = chosenCard; // this will start downloading the image maybe set unknown rounded/square to front before starting the download
                                           //cardInstance.SetActive(false);
            cardInstance.transform.SetParent(normalCardsHolder);
            cardInstance.transform.localScale = new Vector3(1f, 1f, 1f);
            Match matcher = Regex.Match(chosenCard.setCode, SQUARE_SETS, RegexOptions.IgnoreCase);
            if (matcher.Success)
            {
                card.back = squareBack;
            }
            else
            {
                card.back = roundedBack;
            }

            return cardInstance;
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
            Card1 card = cardInstance.GetComponent<Card1>();
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

        internal static int GetNumberOfAvailableCards(int generation, int nationalPokedexNumber)
        {
            if (nationalPokedexNumber >= CardFactory.startNPNOfGeneration[generation] + CardFactory.numberOfCardsInGeneration[generation])
            {
                return 0;
            }
            if (!availableNPNs[generation].ContainsKey(nationalPokedexNumber))
            {
                return 0;
            }
            return availableNPNs[generation][nationalPokedexNumber];
        }

        public static IEnumerator CreateCard111(PossibleCard someCard, Action<GameObject> returnToCaller)
        {
            GameObject cardInstance = GameObject.Instantiate(cardPrefab);
            cardInstance.SetActive(false);
            Card card = cardInstance.GetComponent<Card>();
            card.CreatedFrom = someCard;
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
                Card1 card = cardInstance.GetComponent<Card1>();
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

        public static IEnumerator FillSprite(ImageType type, string url, Card card)
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
                    card.front = sprite;
                    cardsInPackStillLoading--;
                    Debug.Log("WWW front filled of " + card.CreatedFrom.id + " still loading " + cardsInPackStillLoading);
                    CacheManager.PutSprite(sprite, type, url);
                }
            }
            else
            {
                cardsInPackStillLoading--;
                Debug.Log("Cache front filled of " + card.CreatedFrom.id + " still loading " + cardsInPackStillLoading);
                card.front = sprite;
            }
            if (cardsInPackStillLoading == 0)
            {
                packReadyEvent?.Invoke();
            }
        }

    }
}