using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button collectionButton;
    public Button workButton;
    public Button shopButton;
    public Button openButton;

    private float buttonAnchorY;

    // Start is called before the first frame update
    void Start()
    {
        // recalculate anchors according to screen resolution/ratio
        buttonAnchorY = ((0.5f * Screen.width + 0.5f * Screen.height) / Screen.height) - 0.5f;
        Debug.Log("Screen : " + Screen.width + "x" + Screen.height);
        Debug.Log("y: " + buttonAnchorY);
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
    }

    public void OnCollectionClicked()
    {
    }

    public void OnWorkClicked()
    {
    }

    public void OnShopClicked()
    {
    }

    public void OnOpenClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/TestCardFactoryCard.unity");
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                // Quit the application
                Application.Quit();
            }
        }
    }
}
