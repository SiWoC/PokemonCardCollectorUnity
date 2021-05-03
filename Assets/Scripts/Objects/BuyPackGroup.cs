using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPackGroup : MonoBehaviour
{
    public int generation;
    public Text generationText;
    public Text priceText;
    public Text buttonText;
    public GameObject buyButton;
    public GameObject lockImage;

    private Button button;
    private int totalPrice;

    void Awake()
    {
        generationText.text = "Generation " + generation;
        priceText.text = GameManager.price[generation].ToString();
        button = buyButton.GetComponent<Button>();
        lockImage.SetActive(!PlayerStats.IsGenerationUnlocked(generation));
    }

    // Update is called once per frame
    void Update()
    {
        totalPrice = GameManager.price[generation] * GameManager.SelectedMultiplier;
        buttonText.text = "Buy " + GameManager.SelectedMultiplier + " for\r\n" + totalPrice;
        // you must have enough coins and you cannot own more than 100 packs
        button.interactable = (PlayerStats.IsGenerationUnlocked(generation) &&
                               (PlayerStats.GetCoins() >= totalPrice) && 
                               (PlayerStats.GetAvailablePacks(generation) + GameManager.SelectedMultiplier < 100));
    }
}
