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
    public GameObject pageHolder;
    public GameObject cardSpace1;
    public GameObject cardSpace2;

    private Generation generation;
    private int numberOfPokemonInGeneration = 151;
    private int fromNationalPokedexNumber = 1;
    private int pageSize = 9;

    // Start is called before the first frame update
    void Start()
    {
        pageHolder.GetComponent<PageSwiper>().totalPages = (numberOfPokemonInGeneration + pageSize - 1) / pageSize;
        Debug.Log("we have " + pageHolder.GetComponent<PageSwiper>().totalPages + " pages");
        generation = GameManager.playerStats.generations[selectedGeneration];
        for (int pageNumber = 0; pageNumber <= 1; pageNumber++)
        {
            GameObject cardSpace;
            if (pageNumber == 0)
            {
                cardSpace = cardSpace1;
            } else
            {
                cardSpace = cardSpace2;
            }
            for (int i = fromNationalPokedexNumber; i < fromNationalPokedexNumber + pageSize; i++)
            {
                int nationalPokedexNumber = i + pageNumber * pageSize;
                if (generation.cards.ContainsKey(nationalPokedexNumber))
                { // we own cards of this NationalPokedexNumber
                    Debug.Log("You DO own cards with NationalPokedexNumber: " + nationalPokedexNumber);
                    Dictionary<string, PossibleCard> cardsOfNumber = generation.cards[nationalPokedexNumber];
                    foreach (PossibleCard ownedCard in cardsOfNumber.Values)
                    {
                        PlaceCard(ownedCard, cardSpace);
                        break;
                    }
                }
                else
                { // leave empty placeholder? show greyed-out back?
                    Debug.Log("You DONT own cards with NationalPokedexNumber: " + nationalPokedexNumber);
                    PlaceEmptyPanel(nationalPokedexNumber, cardSpace);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)cardSpace.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceEmptyPanel(int nationalPokedexNumber, GameObject cardSpace)
    {

        GameObject panel = new GameObject("noneOwned" + nationalPokedexNumber);
        panel.transform.SetParent(cardSpace.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;
        image.color = color;
    }

    public void PlaceCard(PossibleCard ownedCard, GameObject cardSpace)
    {

        GameObject panel = new GameObject(ownedCard.id);
        panel.SetActive(false);
        panel.transform.SetParent(cardSpace.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;
        panel.SetActive(true);

        StartCoroutine(CardFactory.CreateCard(ownedCard,
            (GameObject newCardInstance) =>
            {
                image.sprite = newCardInstance.GetComponent<Card>().front;
            }));
    }

}
