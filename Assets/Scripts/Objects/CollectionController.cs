using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments.V1;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayerStats = Globals.PlayerStats;

public class CollectionController : MonoBehaviour
{
    public Dropdown generationDropdown;
    public GameObject pageHolder;
    public GameObject singleNPNPageHolder;
    public GameObject singleCard;
    public GameObject pagePrefab;
    public GameObject unknownCardPrefab;
    public GameObject unavailableCardPrefab;

    private Generation generation;
    private int pageSize = 9;

    PageSwiper pageSwiper;
    private Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int maxPageFilled = 0;

    PageSwiper singleNPNPageSwiper;
    private Image singleCardImage;
    private Dictionary<int, GameObject> singleNPNPages = new Dictionary<int, GameObject>();
    Dictionary<string, PossibleCard> cardsOfNumber;
    private int maxSingleNPNPageFilled = 0;

    private void Awake()
    {
        GameManager.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        pageSwiper = pageHolder.GetComponent<PageSwiper>();
        singleNPNPageSwiper = singleNPNPageHolder.GetComponent<PageSwiper>();
        singleCardImage = singleCard.GetComponent<Image>();
        generationDropdown.ClearOptions();
        for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
        {
            generationDropdown.options.Add(new Dropdown.OptionData("gen" + i));
        }
        generationDropdown.value = GameManager.SelectedGeneration - 1;
        OnGenerationChanged(generationDropdown);
    }

    public void OnGenerationChanged(Dropdown dd)
    {
        // Destroy stuff of old generation
        foreach (GameObject page in pages.Values)
        {
            GameObject.Destroy(page);
        }
        pages.Clear();
        maxPageFilled = 0;

        // set up pages for new generation
        GameManager.SelectedGeneration = generationDropdown.value + 1;
        int totalPages = (CardFactory.numberOfCardsInGeneration[GameManager.SelectedGeneration] + pageSize - 1) / pageSize;
        pageSwiper.totalPages = totalPages;
        RectTransform phRectTransform = pageHolder.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(900 * totalPages, phRectTransform.sizeDelta.y);
        generation = PlayerStats.GetGeneration(GameManager.SelectedGeneration);
        for (int pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            GameObject page = GameObject.Instantiate(pagePrefab);
            pages.Add(pageNumber, page);
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
            int nationalPokedexNumber = CardFactory.startNPNOfGeneration[GameManager.SelectedGeneration] + i + pageNumber * pageSize;
            if (nationalPokedexNumber < CardFactory.startNPNOfGeneration[GameManager.SelectedGeneration] + CardFactory.numberOfCardsInGeneration[GameManager.SelectedGeneration])
            {
                if (generation.cards.ContainsKey(nationalPokedexNumber))
                { // we own cards of this NationalPokedexNumber
                    Dictionary<string, PossibleCard> cardsOfNumber = generation.cards[nationalPokedexNumber];
                    PossibleCard lastFound = cardsOfNumber.Values.OrderBy(p => p.foundOn).Last();
                    PlaceSmallCard(lastFound, page);
                }
                else if (CardFactory.GetNumberOfAvailableCards(GameManager.SelectedGeneration, nationalPokedexNumber) > 0)
                { // leave empty placeholder? show greyed-out back?
                    PlaceEmptyPanel(nationalPokedexNumber, page);
                } else
                {
                    PlaceUnavailablePanel(nationalPokedexNumber, page);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)page.transform);
            }
        }
    }

    public void PlaceEmptyPanel(int nationalPokedexNumber, GameObject page)
    {

        GameObject panel = GameObject.Instantiate(unknownCardPrefab);
        panel.name = "noneOwned" + nationalPokedexNumber;
        /*
        Image image = panel.GetComponent<Image>();
        Color color = image.color;
        color.a = 0.5f;
        image.color = color;
        */
        panel.transform.SetParent(page.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void PlaceUnavailablePanel(int nationalPokedexNumber, GameObject page)
    {

        GameObject panel = GameObject.Instantiate(unavailableCardPrefab);
        panel.GetComponentInChildren<Text>().text += nationalPokedexNumber;
        panel.name = "unavailable" + nationalPokedexNumber;
        /*
        Image image = panel.GetComponent<Image>();
        Color color = image.color;
        color.a = 0.5f;
        image.color = color;
        */
        panel.transform.SetParent(page.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void PlaceSmallCard(PossibleCard ownedCard, GameObject page)
    {
        PlaceCard(ownedCard, ImageType.Small, page);
    }

    public void PlaceLargeCard(PossibleCard ownedCard, GameObject page)
    {
        PlaceCard(ownedCard, ImageType.Large, page);
    }

    public void PlaceCard(PossibleCard ownedCard, ImageType imageType, GameObject page)
    {
        GameObject panel = GameObject.Instantiate(unknownCardPrefab);
        panel.name = ownedCard.id;

        Image image = panel.GetComponent<Image>();
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;

        panel.transform.SetParent(page.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);

        Button button = panel.AddComponent<Button>();
        button.image = panel.GetComponent<Image>();
        button.onClick.AddListener(() => { OnPointerClick(ownedCard, button.image); });
        string url = null;
        switch (imageType)
        {
            case ImageType.Small:
                url = ownedCard.imageUrlSmall;
                break;
            case ImageType.Large:
                url = ownedCard.imageUrlLarge;
                break;
        }
        StartCoroutine(CardFactory.FillImage(imageType, url, button.image));
    }

    private void SingleNPNStart()
    {
        generation = PlayerStats.GetGeneration(GameManager.SelectedGeneration);
        cardsOfNumber = generation.cards[GameManager.selectedNPN];
        Debug.Log("count: " + cardsOfNumber.Count);
        // set up pages for new NPN
        int totalPages = (cardsOfNumber.Count + pageSize - 1) / pageSize;
        singleNPNPageSwiper = singleNPNPageHolder.GetComponent<PageSwiper>();
        singleNPNPageSwiper.totalPages = totalPages;
        RectTransform phRectTransform = singleNPNPageHolder.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(900 * totalPages, phRectTransform.sizeDelta.y);
        for (int pageNumber = 0; pageNumber < totalPages; pageNumber++)
        {
            GameObject page = GameObject.Instantiate(pagePrefab);
            singleNPNPages.Add(pageNumber, page);
            page.transform.SetParent(singleNPNPageHolder.transform);
            page.transform.localPosition = new Vector3(900 * pageNumber, 0f, 0f);
            page.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // pageSwiper will publish event pageChanged which will fill some pages
        pageHolder.SetActive(false);
        generationDropdown.gameObject.SetActive(false);
        singleNPNPageHolder.SetActive(true);
        singleNPNPageSwiper.BackToPage1();
    }

    private void FillSingleNPNPage(int pageNumber, GameObject page)
    {
        IEnumerable<PossibleCard> cards = cardsOfNumber.Values.OrderBy(p => p.foundOn).Skip(pageNumber * pageSize).Take(9);
        foreach (PossibleCard card in cards)
        {
            PlaceLargeCard(card, page);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)page.transform);
        }
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

    public void OnSingleNPNPageChanged()
    {
        // pageSwiper pages start at 1, we start at 0
        int currentPage = singleNPNPageSwiper.currentPage - 1;
        int newMax = Mathf.Min(singleNPNPageSwiper.totalPages, currentPage + 2);
        for (int pageNumber = maxSingleNPNPageFilled; pageNumber < newMax; pageNumber++)
        {
            FillSingleNPNPage(pageNumber, singleNPNPages[pageNumber]);
        }
        maxSingleNPNPageFilled = Mathf.Max(maxSingleNPNPageFilled, newMax);
    }

    public void OnPointerClick(PossibleCard ownedCard, Image image)
    {
        if (singleNPNPageHolder.activeSelf)
        {
            //Debug.Log("already single NPN, so zoom large card");
            singleCardImage.sprite = image.sprite;
            singleCard.SetActive(true);
            singleNPNPageHolder.SetActive(false);
        }
        else
        {
            GameManager.selectedNPN = ownedCard.nationalPokedexNumber;
            SingleNPNStart();
        }
    }

    public void OnBack()
    {
        if (singleCard.activeSelf)
        {
            singleCard.SetActive(false);
            singleNPNPageHolder.SetActive(true);
        }
        else if (singleNPNPageHolder.activeSelf)
        {
            // Destroy stuff of this NPN
            foreach (GameObject page in singleNPNPages.Values)
            {
                GameObject.Destroy(page);
            }
            singleNPNPages.Clear();
            maxSingleNPNPageFilled = 0;

            singleNPNPageHolder.SetActive(false);
            pageHolder.SetActive(true);
            generationDropdown.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Back();
        }
    }

    public void OnSingleCardClick()
    {
        singleCard.SetActive(false);
        singleNPNPageHolder.SetActive(true);
    }
}
