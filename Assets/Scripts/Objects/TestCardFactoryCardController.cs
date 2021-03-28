using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Factories;
using UnityEngine.SceneManagement;

public class TestCardFactoryCardController : MonoBehaviour
{
    public Button startButton;
    public Dropdown generationDropdown;
    public Dropdown rarityDropdown;
    
    public GameObject cardPrefab;
    public Sprite roundedBack;
    public Sprite squareBack;
    
    public Canvas cardCanvas;
    private GameObject cardInstance;

    private void Awake()
    {
        // should be set from initialize somewhere
        CardFactory.cardPrefab = cardPrefab;
        CardFactory.roundedBack = roundedBack;
        CardFactory.squareBack = squareBack;
    }

    void Start()
    {
        //cardCanvas = 
        //startButton.onClick.AddListener(OnStartClicked);
    }

    public void OnStartClicked()
    {
        startButton.interactable = false;
        if (cardInstance != null)
        {
            Destroy(cardInstance);
        }

        StartCoroutine(CardFactory.GetCard(generationDropdown.options[generationDropdown.value].text, rarityDropdown.options[rarityDropdown.value].text,
            (GameObject newCardInstance) =>
            {
                cardInstance = newCardInstance;
                cardInstance.transform.SetParent(cardCanvas.transform);
                cardInstance.transform.localPosition = new Vector3(0f, -75f, 0f);
                cardInstance.transform.localScale = new Vector3(1f, 1f, 1f);
                cardInstance.SetActive(true);
                startButton.interactable = true;
            }));
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
                // Back to the main menu
                BackToMainMenu();
            }
        }
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity");
    }

}
