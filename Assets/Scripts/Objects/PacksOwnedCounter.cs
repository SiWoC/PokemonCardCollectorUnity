using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacksOwnedCounter : MonoBehaviour
{
    public int generation;
    public Text counterText;

    // Update is called once per frame
    void Update()
    {
        counterText.text = PlayerStats.GetAvailablePacks(generation).ToString();
    }
}
