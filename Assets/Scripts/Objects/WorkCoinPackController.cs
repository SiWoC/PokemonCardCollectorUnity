using Assets.Scripts.Classes.Globals;
using Globals;
using UnityEngine;

public class WorkCoinPackController : MonoBehaviour
{
    public GameObject tutorialCoinsPacksPanel;

    void Start()
    {
        tutorialCoinsPacksPanel.SetActive(PlayerStats.GetShowTutorialStep(TutorialStep.CoinsPacks));
    }

    public void OnCoinsClicked()
    {
        PlayerStats.SetTutorialStepCompleted(TutorialStep.CoinsPacks);
        tutorialCoinsPacksPanel.SetActive(false);
        GameManager.earnType = EarnType.Coins;
        GameManager.Forward("Assets/Scenes/WorkPickChore.unity");
    }

    public void OnPacksClicked()
    {
        GameManager.earnType = EarnType.Packs;
        GameManager.Forward("Assets/Scenes/WorkPickChore.unity");
    }

    public void OnBack()
    {
        // you must have earned enough to buy your first package
        if (PlayerStats.GetCoins() >= GameManager.GetPriceInCents(1))
        {
            PlayerStats.SetTutorialStepCompleted(TutorialStep.GoToWork);
        }
        GameManager.Back();
    }
}
