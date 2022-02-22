using Factories;
using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsController : MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject statisticsGroupPrefab;
    public GameObject statisticsGroupDividerPrefab;

    private int contentPanelHeigth = 0;
    private int statisticsGroupHeight = 105;
    private int dividerHeight = 15;

    // Start is called before the first frame update
    void Start()
    {
        AddStatistic("Total coins earned", (PlayerStats.GetStatsTotalCoins() / 100f).ToString("n2"), -1f);
        AddStatistic("Random packs earned", PlayerStats.GetStatsRandomPacks().ToString("n0"), -1f);
        AddStatistic("Total clicks", PlayerStats.GetStatsTotalClicks().ToString("n0"), -1f);
        AddStatistic("Packs opened", PlayerStats.GetStatsPacksOpened().ToString("n0"), -1f);
        AddStatistic("Doubles traded in", PlayerStats.GetStatsDoublesTradedIn().ToString("n0"), -1f);

        AddStatistic("Generations unlocked",
            PlayerStats.GetHighestUnlockedGeneration() + " of " + CardFactory.numberOfGenerations,
            100f * PlayerStats.GetHighestUnlockedGeneration() / CardFactory.numberOfGenerations);

        int totalNumberOwned = 0;
        Dictionary<int, int> totalOwnedOfGeneration = new Dictionary<int, int>();
        for (int generation = 1; generation <= PlayerStats.GetHighestUnlockedGeneration(); generation++)
        {
            totalOwnedOfGeneration[generation] = PlayerStats.GetNumberOfOwnedCards(generation);
            totalNumberOwned += totalOwnedOfGeneration[generation];
        }
        AddStatistic("Total cards",
            totalNumberOwned.ToString("n0") + " out of " + CardFactory.GetTotalNumberOfCards().ToString("n0"),
            100f * totalNumberOwned / CardFactory.GetTotalNumberOfCards());

        for (int generation = 1; generation <= PlayerStats.GetHighestUnlockedGeneration(); generation++)
        {
            AddStatistic("Gen " + generation + " NPNs",
                PlayerStats.GetGeneration(generation).cards.Count.ToString("n0") + " out of " + CardFactory.numberOfNPNsInGeneration[generation].ToString("n0"),
                100f * PlayerStats.GetGeneration(generation).cards.Count / CardFactory.numberOfNPNsInGeneration[generation]);
            AddStatistic("Gen " + generation + " cards",
                totalOwnedOfGeneration[generation].ToString("n0") + " out of " + CardFactory.GetTotalNumberOfCards(generation).ToString("n0"),
                100f * totalOwnedOfGeneration[generation] / CardFactory.GetTotalNumberOfCards(generation));
        }
    }

    private void AddStatistic(string title, string progress, float fillPercentage)
    {
        if (contentPanelHeigth > 0)
        {
            AddDivider();
        }
        GameObject statisticsGroupObject = GameObject.Instantiate(statisticsGroupPrefab);
        StatisticsGroup statisticsGroup = statisticsGroupObject.GetComponent<StatisticsGroup>();
        statisticsGroup.title = title;
        if (fillPercentage == -1f)
        {
            statisticsGroup.progress = progress;
            statisticsGroup.fillPercentage = fillPercentage;
        }
        else
        {
            statisticsGroup.progress = progress + " (" + fillPercentage.ToString("n2") + "%)";
            statisticsGroup.fillPercentage = fillPercentage;
        }
        statisticsGroupObject.transform.SetParent(contentPanel.transform);
        statisticsGroupObject.transform.localScale = new Vector3(1f, 1f, 1f);
        RectTransform phRectTransform = contentPanel.GetComponent<RectTransform>();
        contentPanelHeigth += statisticsGroupHeight;
        phRectTransform.sizeDelta = new Vector2(phRectTransform.sizeDelta.x, contentPanelHeigth);
    }

    private void AddDivider()
    {
        GameObject divider = GameObject.Instantiate(statisticsGroupDividerPrefab);
        divider.transform.SetParent(contentPanel.transform);
        divider.transform.localScale = new Vector3(1f, 1f, 1f);
        RectTransform phRectTransform = contentPanel.GetComponent<RectTransform>();
        contentPanelHeigth += dividerHeight;
        phRectTransform.sizeDelta = new Vector2(phRectTransform.sizeDelta.x, contentPanelHeigth);
    }
}
