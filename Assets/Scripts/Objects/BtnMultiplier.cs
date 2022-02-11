using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnMultiplier : MonoBehaviour
{
    public Text text;
    private int[] values = { 1, 5, 10 };
    private int index = 0;

    void Awake()
    {
        GameManager.Initialize();
        for (; index < 3; index++)
        {
            if (GameManager.SelectedMultiplier == values[index])
            {
                break;
            }
        }
        text.text = "x" + values[index];
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "x" + values[index];
    }

    public void OnClicked()
    {
        index++;
        if (index > 2)
        {
            index = 0;
        }
        GameManager.SelectedMultiplier = values[index];
        // FOR TESTMODUS, EASY EARNING
        PlayerStats.AddCoins(36912);
    }
}
