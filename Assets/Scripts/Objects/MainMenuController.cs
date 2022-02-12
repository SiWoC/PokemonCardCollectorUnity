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
        tutorialGoToOpeningPanels.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack));
        tutorialGoToCollectionPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToCollection));
        tutorialGoToWorkPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.GoToCollection) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToWork));
        tutorialGoToShopPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.GoToWork) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToShop));
    }

    private void Update()
    {
        tutorialGoToOpeningPanels.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack));
        tutorialGoToCollectionPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToCollection));
        tutorialGoToWorkPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.GoToCollection) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToWork));
        tutorialGoToShopPanels.SetActive(PlayerStats.GetTutorialCompleted(TutorialStep.GoToWork) && !PlayerStats.GetTutorialCompleted(TutorialStep.GoToShop));
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
