using Globals;
using UnityEngine;
using Factories;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button collectionButton;
    public Button workButton;
    public Button shopButton;
    public Button openButton;

    private void Awake()
    {
        GameManager.Initialize();
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
    }

    public void OnOpenClicked()
    {
        GameManager.Forward("Assets/Scenes/TestCardFactoryCard.unity");
    }

}
