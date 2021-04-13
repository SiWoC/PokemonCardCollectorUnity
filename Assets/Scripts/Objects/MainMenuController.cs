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

    public GameObject cardPrefab;
    public Sprite roundedBack;
    public Sprite squareBack;

    private float buttonAnchorY;

    private void Awake()
    {
        // should be set from initialize somewhere
        CardFactory.cardPrefab = cardPrefab;
        CardFactory.roundedBack = roundedBack;
        CardFactory.squareBack = squareBack;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        // recalculate anchors according to screen resolution/ratio
        buttonAnchorY = ((0.5f * Screen.width + 0.5f * Screen.height) / Screen.height) - 0.5f;
        RectTransform rectTransform;
        // collectionButton
        rectTransform = collectionButton.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f + buttonAnchorY);
        // workButton
        rectTransform = workButton.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(1.0f, 0.5f + buttonAnchorY);
        // shopButton
        rectTransform = shopButton.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.0f, 0.5f - buttonAnchorY);
        // openButton
        rectTransform = openButton.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f - buttonAnchorY);
        */
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
        //SceneManager.LoadScene("Assets/Scenes/TestCardFactoryCard.unity");
        GameManager.Forward("Assets/Scenes/TestCardFactoryCard.unity");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
