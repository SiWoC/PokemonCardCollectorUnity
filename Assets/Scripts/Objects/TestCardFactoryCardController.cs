using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Factories;

public class TestCardFactoryCardController : MonoBehaviour
{
    public Button startButton;
    public Dropdown generationDropdown;
    public Dropdown rarityDropdown;
    
    public GameObject cardPrefab;
    public Sprite roundedBack;
    public Sprite squareBack;

    private GameObject cardInstance;

    private void Awake()
    {
        CardFactory.cardPrefab = cardPrefab;
        CardFactory.roundedBack = roundedBack;
        CardFactory.squareBack = squareBack;
    }

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
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
                cardInstance.transform.position = new Vector3(0, -7.5f, 60f);
                cardInstance.SetActive(true);
                startButton.interactable = true;
            }));
    }

}
