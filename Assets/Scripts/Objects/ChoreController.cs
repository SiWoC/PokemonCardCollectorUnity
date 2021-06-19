using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoreController : MonoBehaviour
{

    public GameObject randomPackEarnedPanel;
    public TextMeshProUGUI randomPackEarnedText;
    public Image randomPackImage;
    public GameObject bagImage;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.earnType == EarnType.Coins)
        {
            bagImage.SetActive(false);
        }
    }

    private void Awake()
    {
        GameManager.Initialize();
        GameManager.RandomPackEarnedEvent += RandomPackEarnedEvent;
    }

    private void OnDestroy()
    {
        GameManager.RandomPackEarnedEvent -= RandomPackEarnedEvent;
    }

    private void RandomPackEarnedEvent(int generation)
    {
        randomPackImage.sprite = Resources.Load<Sprite>("Images/Packs/" + PlayerStats.GetNextPack(generation)); // zonder png !!!
        randomPackEarnedText.text = "You earned\r\na pack of\r\ngeneration " + generation + "!!!";
        randomPackEarnedPanel.SetActive(true);
    }

}
