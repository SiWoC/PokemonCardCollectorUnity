using Factories;
using Factories.Config;
using Globals;
using Globals.PlayerStatsSegments;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSingleNPNController : MonoBehaviour
{
    public GameObject pageHolder;
    public GameObject pagePrefab;

    PageSwiper pageSwiper;
    private Generation generation;
    Dictionary<string, PossibleCard> cardsOfNumber;
    private int pageSize = 9;
    private Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int maxPageFilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        generation = GameManager.playerStats.generations[GameManager.selectedGeneration];
        cardsOfNumber = generation.cards[GameManager.selectedNPN];
        // set up pages for new NPN
        int totalPages = (cardsOfNumber.Count + pageSize - 1) / pageSize;
        pageSwiper = pageHolder.GetComponent<PageSwiper>();
        pageSwiper.totalPages = totalPages;
        RectTransform phRectTransform = pageHolder.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(900 * totalPages, phRectTransform.sizeDelta.y);
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
        IEnumerable<PossibleCard> cards = cardsOfNumber.Values.OrderBy(p => p.foundOn).Skip(pageNumber * pageSize);
        foreach (PossibleCard card in cards)
        {
            PlaceCard(card, page);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)page.transform);
        }
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
