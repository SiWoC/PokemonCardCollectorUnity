using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{

    void Awake()
    {
        GameManager.Initialize();
    }

    public void Buy(int generation)
    {
        Debug.Log("buying " + GameManager.SelectedMultiplier + "x gen " + generation + " for " + GameManager.price[generation] * GameManager.SelectedMultiplier + " pokedollars");
        GameManager.BuyPack(generation);
    }

}
