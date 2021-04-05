using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
    public int selectedGeneration = 1;
    public int selectedPage = 1;
    public GameObject cardSpace;

    private Generation generation;
    private int fromNationalPokedexNumber = 1;
    private int pageSize = 9;
    private GameObject cardInstance;

    // Start is called before the first frame update
    void Start()
    {
        generation = GameManager.playerStats.generations[selectedGeneration];
        for (int i = 1; i < 152; i++)
        {
            if (generation.cards.ContainsKey(i))
            {
                Debug.Log("All You DO own cards with NationalPokedexNumber: " + i);
            }
        }
        for (int i = fromNationalPokedexNumber; i < fromNationalPokedexNumber + pageSize; i++)
        {
            if (generation.cards.ContainsKey(i))
            { // we own cards of this NationalPokedexNumber
                Debug.Log("You DO own cards with NationalPokedexNumber: " + i);
                Dictionary<string, PossibleCard> cardsOfNumber = generation.cards[i];
                foreach (PossibleCard ownedCard in cardsOfNumber.Values)
                {
                    PlaceCard(ownedCard);
                    break;
                }
            }
            else { // leave empty placeholder? show greyed-out back?
                Debug.Log("You DONT own cards with NationalPokedexNumber: " + i);
                PlaceEmptyPanel(i);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)cardSpace.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceEmptyPanel(int nationalPokedexNumber)
    {

        GameObject panel = new GameObject("noneOwned" + nationalPokedexNumber);
        panel.transform.SetParent(cardSpace.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;
        image.color = color;
    }

    public void PlaceCard(PossibleCard ownedCard)
    {

        GameObject panel = new GameObject(ownedCard.id);
        panel.SetActive(false);
        panel.transform.SetParent(cardSpace.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;

        StartCoroutine(CardFactory.CreateCard(ownedCard,
            (GameObject newCardInstance) =>
            {
                image.sprite = newCardInstance.GetComponent<Card>().front;
                panel.SetActive(true);
            }));
    }

}
