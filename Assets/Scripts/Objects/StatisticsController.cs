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
        statisticsGroup.progress = progress + " (" + fillPercentage.ToString("n2") + "%)";
        statisticsGroup.fillPercentage = fillPercentage;
        statisticsGroupObject.transform.SetParent(contentPanel.transform);
        statisticsGroupObject.transform.localScale = new Vector3(1f, 1f, 1f);
        RectTransform phRectTransform = contentPanel.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(phRectTransform.sizeDelta.x, contentPanelHeigth);
        contentPanelHeigth += statisticsGroupHeight;
    }
    private void AddDivider()
    {
        GameObject divider = GameObject.Instantiate(statisticsGroupDividerPrefab);
        divider.transform.SetParent(contentPanel.transform);
        divider.transform.localScale = new Vector3(1f, 1f, 1f);
        RectTransform phRectTransform = contentPanel.GetComponent<RectTransform>();
        phRectTransform.sizeDelta = new Vector2(phRectTransform.sizeDelta.x, contentPanelHeigth);
        contentPanelHeigth += dividerHeight;
    }
}
