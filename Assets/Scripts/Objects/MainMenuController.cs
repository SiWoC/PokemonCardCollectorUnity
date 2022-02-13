using Globals;
using UnityEngine;
using Factories;
using UnityEngine.UI;
using Assets.Scripts.Classes.Globals;

public class MainMenuController : MonoBehaviour
{
    public Button collectionButton;
    public Button workButton;
    public Button shopButton;
    public Button openButton;
    public GameObject tutorialGoToCollectionPanels;
    public GameObject tutorialGoToWorkPanels;
    public GameObject tutorialGoToShopPanels;
    public GameObject tutorialGoToOpeningPanels;

    private void Awake()
    {
        GameManager.Initialize();
    }

    void Start()
    {
        SetPanelVisibility();
    }

    private void Update()
    {
        SetPanelVisibility();
    }

    private void SetPanelVisibility()
    {
        tutorialGoToOpeningPanels.SetActive(PlayerStats.GetShowTutorialStep(TutorialStep.OpenPack));
        tutorialGoToCollectionPanels.SetActive(!PlayerStats.GetShowTutorialStep(TutorialStep.OpenPack)
                                            && PlayerStats.GetShowTutorialStep(TutorialStep.GoToCollection));
        tutorialGoToWorkPanels.SetActive(!PlayerStats.GetShowTutorialStep(TutorialStep.GoToCollection)
                                            && !PlayerStats.GetShowTutorialStep(TutorialStep.OpenPack)
                                            && PlayerStats.GetShowTutorialStep(TutorialStep.GoToWork));
        tutorialGoToShopPanels.SetActive(!PlayerStats.GetShowTutorialStep(TutorialStep.GoToCollection)
                                            && !PlayerStats.GetShowTutorialStep(TutorialStep.OpenPack)
                                            && !PlayerStats.GetShowTutorialStep(TutorialStep.GoToWork)
                                            && PlayerStats.GetShowTutorialStep(TutorialStep.GoToShop));
    }

    public void OnCollectionClicked()
    {
        GameManager.Forward("Assets/Scenes/Collection.unity");
    }

    public void OnWorkClicked()
    {
        GameManager.Forward("Assets/Scenes/WorkCoinPack.unity");
    }

    public void OnShopClicked()
    {
        GameManager.Forward("Assets/Scenes/Shop.unity");
    }

    public void OnOpenClicked()
    {
        GameManager.Forward("Assets/Scenes/Opening.unity");
    }

    public void OnStatisticsClicked()
    {
        GameManager.Forward("Assets/Scenes/Statistics.unity");
    }

    public void OnSettingsClicked()
    {
        GameManager.Forward("Assets/Scenes/Settings.unity");
    }

}
