using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Text text;
    public GameObject coinsImage;
    public GameObject packsImage;

    private Color coinsColor = new Color(1f, 0.8352f,0f,1f);
    private Color packsColor = new Color(1f, 0.1725f, 0.2705f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.earnType == EarnType.Coins)
        {
            coinsImage.SetActive(true);
            packsImage.SetActive(false);
            text.color = coinsColor;
        } else
        {
            packsImage.SetActive(true);
            coinsImage.SetActive(false);
            text.color = packsColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.earnType == EarnType.Coins)
        {
            text.text = GameManager.GetCoins().ToString("D8");
        }
        else
        {
            text.text = string.Format("{0:000.00000}",GameManager.GetRandomPackPercentage() / 100000f);
        }
    }
}