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
    public bool useFixedEarntype = false;
    public EarnType fixedEarnType;

    private Color coinsColor = new Color(1f, 0.8352f,0f,1f);
    private Color packsColor = new Color(1f, 0.1725f, 0.2705f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        if ((useFixedEarntype && fixedEarnType == EarnType.Coins) || (!useFixedEarntype && GameManager.earnType == EarnType.Coins))
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
        if ((useFixedEarntype && fixedEarnType == EarnType.Coins) || (!useFixedEarntype && GameManager.earnType == EarnType.Coins))
        {
            text.text = PlayerStats.GetCoins().ToString("D8");
        }
        else
        {
            text.text = string.Format("{0:000.00000}",PlayerStats.GetRandomPackPercentage() / 100000f);
        }
    }
}
