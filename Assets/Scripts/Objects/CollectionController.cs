using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
    private const string DESTROYABLE_TAG = "Collection_Destroy_OnGenerationChanged";
    public Dropdown generationDropdown;
    public GameObject pageHolder;
    public GameObject pagePrefab;

    PageSwiper pageSwiper;
    private Generation generation;
    private int pageSize = 9;
    private Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int maxPageFilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        pageSwiper = pageHolder.GetComponent<PageSwiper>();
        generationDropdown.ClearOptions();
        for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
        {
            generationDropdown.options.Add(new Dropdown.OptionData("gen" + i));
        }
        OnGenerationChanged(generationDropdown);
    }

    public void OnGenerationChanged(Dropdown dd)
    {
        // Destroy stuff of old generation
        GameObject[] destroyables = GameObject.FindGameObjectsWithTag(DESTROYABLE_TAG);
        foreach (GameObject destroyable in destroyables)
        {
            GameObject.Destroy(destroyable);
        }
        pages.Clear();
        maxPageFilled = 0;

        // set up pages for new generation
        GameManager.selectedGeneration = generationDropdown.value + 1;
        int totalPages = (CardFactory.numberOfCardsInGeneration[GameManager.selectedGeneration] + pageSize - 1) / pageSize;
        pageSwiper.totalPages = totalPages;
        RectTransform phRectTransform = pageHolder.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(900 * totalPages, phRectTransform.sizeDelta.y);
        generation = GameManager.playerStats.generations[GameManager.selectedGeneration];
        for (int pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            GameObject page = GameObject.Instantiate(pagePrefab);
            pages.Add(pageNumber, page);
            page.tag = DESTROYABLE_TAG;
            page.transform.SetParent(pageHolder.transform);
            page.transform.localPosition = new Vector3(900 * pageNumber, 0f, 0f);
            page.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // pageSwiper will publish event pageChanged which will fill some pages
        pageSwiper.BackToPage1();
    }

    private void FillPage(int pageNumber, GameObject page)
    {
        for (int i = 0; i < pageSize; i++)
        {
            int nationalPokedexNumber = CardFactory.startNPNOfGeneration[GameManager.selectedGeneration] + i + pageNumber * pageSize;
            if (nationalPokedexNumber < CardFactory.startNPNOfGeneration[GameManager.selectedGeneration] + CardFactory.numberOfCardsInGeneration[GameManager.selectedGeneration])
            {
                if (generation.cards.ContainsKey(nationalPokedexNumber))
                { // we own cards of this NationalPokedexNumber
                    Dictionary<string, PossibleCard> cardsOfNumber = generation.cards[nationalPokedexNumber];
                    PossibleCard lastFound = cardsOfNumber.Values.OrderBy(p => p.foundOn).Last();
                    PlaceCard(lastFound, page);
                }
                else
                { // leave empty placeholder? show greyed-out back?
                    PlaceEmptyPanel(nationalPokedexNumber, page);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)page.transform);
            }
        }
    }

    public void PlaceEmptyPanel(int nationalPokedexNumber, GameObject cardSpace)
    {

        GameObject panel = new GameObject("noneOwned" + nationalPokedexNumber);
        panel.tag = DESTROYABLE_TAG;
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
        panel.tag = DESTROYABLE_TAG;
        panel.SetActive(false);
        panel.transform.SetParent(cardSpace.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;
        panel.SetActive(true);
        StartCoroutine(CardFactory.FillImage(ImageType.Small, ownedCard.imageUrlSmall, image));
    }

    public void OnPageChanged()
    {
        // pageSwiper pages start at 1, we start at 0
        int currentPage = pageSwiper.currentPage - 1;
        int newMax = Mathf.Min(pageSwiper.totalPages, currentPage + 2);
        for (int pageNumber = maxPageFilled; pageNumber < newMax; pageNumber++)
        {
            FillPage(pageNumber, pages[pageNumber]);
        }
        maxPageFilled = Mathf.Max(maxPageFilled, newMax);
    }
}
