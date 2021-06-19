using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments.V1;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public Text generationProgressText;
    public Text npnProgressText;
    public GameObject doublesButton;
    public GameObject turnInDoublesPanel;
    public TextMeshProUGUI doublesText;
    public TextMeshProUGUI finalClickPowerText;
    public GameObject favoriteButton;
    private Image favoriteButtonImage;

    private Generation generation;
    private int pageSize = 9;
    private int numberOfDoubles = 0;

    PageSwiper pageSwiper;
    private Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int maxPageFilled = 0;
    private Dictionary<int, Image> smallImages = new Dictionary<int, Image>();

    PageSwiper singleNPNPageSwiper;
    private Image singleCardImage;
    private PossibleCard currentSingle;
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
        favoriteButtonImage = favoriteButton.GetComponent<Image>();
        generationDropdown.ClearOptions();
        for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
        {
            if (PlayerStats.IsGenerationUnlocked(i))
            {
                generationDropdown.options.Add(new Dropdown.OptionData("gen" + i));
            }
        }
        generationDropdown.value = GameManager.SelectedGeneration - 1;
        // if GameManager.SelectedGeneration != 1, then line above will have triggered OnGenerationChanged
        if (GameManager.SelectedGeneration == 1)
        {
            OnGenerationChanged(generationDropdown);
        }
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

        GameManager.SelectedGeneration = generationDropdown.value + 1;
        generationProgressText.text = "You own cards for " + PlayerStats.GetGeneration(GameManager.SelectedGeneration).cards.Count + " out of " + CardFactory.numberOfNPNsInGeneration[GameManager.SelectedGeneration]
                                        + "\r\nNational Pokedex Numbers of this generation";
        // set up pages for new generation
        int totalPages = (CardFactory.numberOfNPNsInGeneration[GameManager.SelectedGeneration] + pageSize - 1) / pageSize;
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
        numberOfDoubles = PlayerStats.CheckDoubles(GameManager.SelectedGeneration);
        doublesButton.SetActive(numberOfDoubles > 0);
        // pageSwiper will publish event pageChanged which will fill some pages
        pageSwiper.BackToPage1();
    }

    private void FillPage(int pageNumber, GameObject page)
    {
        for (int i = 0; i < pageSize; i++)
        {
            int nationalPokedexNumber = CardFactory.startNPNOfGeneration[GameManager.SelectedGeneration] + i + pageNumber * pageSize;
            if (nationalPokedexNumber < CardFactory.startNPNOfGeneration[GameManager.SelectedGeneration] + CardFactory.numberOfNPNsInGeneration[GameManager.SelectedGeneration])
            {
                if (generation.cards.ContainsKey(nationalPokedexNumber))
                { // we own cards of this NationalPokedexNumber
                    Dictionary<string, PossibleCard> cardsOfNumber = generation.cards[nationalPokedexNumber];
                    string favorite = PlayerStats.GetFavorite(nationalPokedexNumber);
                    PossibleCard lastFoundOrFavorite = null;
                    if (favorite != null)
                    {
                        lastFoundOrFavorite = cardsOfNumber[favorite];
                    }
                    if (lastFoundOrFavorite == null)
                    {
                        lastFoundOrFavorite = cardsOfNumber.Values.OrderBy(p => p.foundOn).Last();
                    }
                    PlaceSmallCard(lastFoundOrFavorite, page);
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
        if (imageType == ImageType.Small)
        {
            smallImages[ownedCard.nationalPokedexNumber] = image;
        }
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

    private void SingleNPNStart(int nationalPokedexNumber)
    {
        GameManager.selectedNPN = nationalPokedexNumber;
        generation = PlayerStats.GetGeneration(GameManager.SelectedGeneration);
        cardsOfNumber = generation.cards[nationalPokedexNumber];
        npnProgressText.text = "You own " + cardsOfNumber.Count + " out of " + CardFactory.GetNumberOfAvailableCards(GameManager.SelectedGeneration, nationalPokedexNumber)
                                        + "\r\ncards of this National Pokedex Number " + nationalPokedexNumber;
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
        generationProgressText.gameObject.SetActive(false);
        singleNPNPageHolder.SetActive(true);
        npnProgressText.gameObject.SetActive(true);
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
            // from NPN to singleCard
            singleCardImage.sprite = image.sprite;
            currentSingle = ownedCard;
            singleCard.SetActive(true);
            if (ownedCard.id.Equals(PlayerStats.GetFavorite(ownedCard.nationalPokedexNumber)))
            {
                favoriteButtonImage.color = new Color(1f, 1f, 1f);
            } else
            {
                favoriteButtonImage.color = new Color(0f, 0f, 1f);
            }
            favoriteButton.SetActive(true);
            singleNPNPageHolder.SetActive(false);
            npnProgressText.gameObject.SetActive(false);
        }
        else
        {
            // from generation to NPN
            SingleNPNStart(ownedCard.nationalPokedexNumber);
        }
    }

    public void OnBack()
    {
        if (singleCard.activeSelf)
        {
            // from singleCard to NPN
            singleCard.SetActive(false);
            favoriteButton.SetActive(false);
            singleNPNPageHolder.SetActive(true);
            npnProgressText.gameObject.SetActive(true);
        }
        else if (singleNPNPageHolder.activeSelf)
        {
            // from NPN to generation
            // Destroy stuff of this NPN
            foreach (GameObject page in singleNPNPages.Values)
            {
                GameObject.Destroy(page);
            }
            singleNPNPages.Clear();
            maxSingleNPNPageFilled = 0;
            npnProgressText.gameObject.SetActive(false);

            singleNPNPageHolder.SetActive(false);
            pageHolder.SetActive(true);
            generationDropdown.gameObject.SetActive(true);
            generationProgressText.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Back();
        }
    }

    public void OnSingleCardClick()
    {
        singleCard.SetActive(false);
        favoriteButton.SetActive(false);
        singleNPNPageHolder.SetActive(true);
        npnProgressText.gameObject.SetActive(true);
    }

    public void OnDoublesClick()
    {
        doublesText.text = string.Format("You have\r\n{0} double card(s)\r\nof this generation.\r\n\r\nDo you want to\r\ntrade them for\r\nClick - Power?", numberOfDoubles);

        finalClickPowerText.text = string.Format("Your Click-Power\r\nwill go from\r\n{0:0.00} to {1:0.00}",
            PlayerStats.GetClickPower() / (float)GameManager.coinFactor,
            (PlayerStats.GetClickPower() + numberOfDoubles) / (float)GameManager.coinFactor);
        turnInDoublesPanel.SetActive(true);
    }

    public void OnYesTradeClick()
    {
        PlayerStats.TradeInDoubles(GameManager.SelectedGeneration);
        numberOfDoubles = 0;
        doublesButton.SetActive(false);
        turnInDoublesPanel.SetActive(false);
    }

    public void OnNoTradeClick()
    {
        turnInDoublesPanel.SetActive(false);
    }

    public void OnFavoriteClick()
    {
        PlayerStats.SetFavorite(currentSingle);
        // yes, now smallImage contains a largeImage, but it's already loaded so I don't care
        smallImages[currentSingle.nationalPokedexNumber].sprite = singleCardImage.sprite;
        favoriteButtonImage.color = new Color(1f,1f,1f);
    }

}
