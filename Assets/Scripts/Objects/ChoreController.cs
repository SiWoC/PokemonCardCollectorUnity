using Assets.Scripts.Classes.Globals;
using Globals;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoreController : MonoBehaviour
{

    public GameObject randomPackEarnedPanel;
    public TextMeshProUGUI randomPackEarnedText;
    public Image randomPackImage;
    public GameObject bagImage;
    public GameObject tutorialEarnCoinsPanel;
    public TextMeshProUGUI tutorialTargetCoinsText;

    // Start is called before the first frame update
    void Start()
    {
        tutorialEarnCoinsPanel.SetActive(PlayerStats.GetShowTutorialStep(TutorialStep.EarnCoins));
        tutorialTargetCoinsText.text = "Earn " + (GameManager.GetPriceInCents(1) / GameManager.coinFactor).ToString() + " Coins.";
        /*
        if (PlayerStats.GetCoins() >= GameManager.GetPriceInCents(1))
        {
            PlayerStats.SetTutorialCompleted(TutorialStep.GoToWork);
        }
         */
        bagImage.SetActive(false);
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
        bagImage.SetActive(true);
    }

}
