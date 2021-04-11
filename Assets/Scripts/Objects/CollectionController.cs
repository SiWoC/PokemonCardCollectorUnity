using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
    public Dropdown generationDropdown;
    public GameObject pageHolder;
    public GameObject singleNPNPageHolder;
    public GameObject pagePrefab;

    private Generation generation;
    private int pageSize = 9;

    PageSwiper pageSwiper;
    private Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int maxPageFilled = 0;

    PageSwiper singleNPNPageSwiper;
    private Dictionary<int, GameObject> singleNPNPages = new Dictionary<int, GameObject>();
    Dictionary<string, PossibleCard> cardsOfNumber;
    private int maxSingleNPNPageFilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        pageSwiper = pageHolder.GetComponent<PageSwiper>();
        singleNPNPageSwiper = singleNPNPageHolder.GetComponent<PageSwiper>();
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
        foreach (GameObject page in pages.Values)
        {
            GameObject.Destroy(page);
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
                    PlaceSmallCard(lastFound, page);
                }
                else
                { // leave empty placeholder? show greyed-out back?
                    PlaceEmptyPanel(nationalPokedexNumber, page);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)page.transform);
            }
        }
    }

    public void PlaceEmptyPanel(int nationalPokedexNumber, GameObject page)
    {

        GameObject panel = new GameObject("noneOwned" + nationalPokedexNumber);
        //panel.tag = DESTROYABLE_TAG;
        panel.transform.SetParent(page.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Image image = panel.AddComponent<Image>();
        Color color = image.color;
        color.a = 0.1f;
        image.color = color;
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
        GameObject panel = new GameObject(ownedCard.id);
        // child of page will be destroyed with page otherwise 2 versions of tag normal/singleNPN
        //panel.tag = DESTROYABLE_TAG;
        panel.SetActive(false);
        panel.transform.SetParent(page.transform);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        Button button = panel.AddComponent<Button>();
        button.image = panel.AddComponent<Image>();
        button.onClick.AddListener(() => { OnPointerClick(ownedCard); });
        panel.SetActive(true);
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
        generation = GameManager.playerStats.generations[GameManager.selectedGeneration];
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
        IEnumerable<PossibleCard> cards = cardsOfNumber.Values.OrderBy(p => p.foundOn).Skip(pageNumber * pageSize);
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
        Debug.Log("SingleNPNpage changed " + currentPage + "-" + newMax);
        for (int pageNumber = maxSingleNPNPageFilled; pageNumber < newMax; pageNumber++)
        {
            FillSingleNPNPage(pageNumber, singleNPNPages[pageNumber]);
        }
        maxSingleNPNPageFilled = Mathf.Max(maxSingleNPNPageFilled, newMax);
    }

    public void OnPointerClick(PossibleCard ownedCard)
    {
        if (singleNPNPageHolder.activeSelf)
        {
            Debug.Log("already single NPN, so zoom large card");
        }
        else
        {
            GameManager.selectedNPN = ownedCard.nationalPokedexNumber;
            SingleNPNStart();
        }
    }

    public void OnBack()
    {
        if (singleNPNPageHolder.activeSelf)
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
}
