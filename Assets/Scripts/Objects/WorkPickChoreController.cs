using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPickChoreController : MonoBehaviour
{
    public GameObject forCoins;
    public GameObject forPacks;

    private void Awake()
    {
        if (GameManager.earnType == EarnType.Coins)
        {
            forCoins.SetActive(true);
        } else
        {
            forPacks.SetActive(true);
        }
    }

    public void OnRotomClicked()
    {
        Debug.Log("You picked Rotom");
    }
}
