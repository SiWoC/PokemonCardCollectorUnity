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
        GameManager.BuyPacks(generation);
    }

}
