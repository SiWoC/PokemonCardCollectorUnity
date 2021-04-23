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

    void Awake()
    {
        generationText.text = "Generation " + generation;
        priceText.text = GameManager.price[generation].ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        buttonText.text = "Buy " + GameManager.SelectedMultiplier + " for\r\n" + (GameManager.price[generation] * GameManager.SelectedMultiplier);
    }
}
