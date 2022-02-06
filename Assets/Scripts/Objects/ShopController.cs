using Assets.Scripts.Classes.Globals;
using Globals;
using UnityEngine;

public class ShopController : MonoBehaviour
{

    void Awake()
    {
        GameManager.Initialize();
    }

    public void Buy(int generation)
    {
        PlayerStats.SetTutorialCompleted(TutorialStep.GoToShop);
        GameManager.BuyPacks(generation);
    }

}
