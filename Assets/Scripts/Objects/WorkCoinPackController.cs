using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCoinPackController : MonoBehaviour
{
    public void OnCoinsClicked()
    {
        GameManager.earnType = EarnType.Coins;
        GameManager.Forward("Assets/Scenes/WorkPickChore.unity");
    }

    public void OnPacksClicked()
    {
        GameManager.earnType = EarnType.Packs;
        GameManager.Forward("Assets/Scenes/WorkPickChore.unity");
    }

}
