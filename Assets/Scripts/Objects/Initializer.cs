using Factories;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    public GameObject packPrefab;
    public GameObject cardPrefab;
    public Sprite roundedBack;
    public Sprite squareBack;

    private void Awake()
    {
        CardFactory.packPrefab = packPrefab;
        CardFactory.cardPrefab = cardPrefab;
        CardFactory.roundedBack = roundedBack;
        CardFactory.squareBack = squareBack;
    }

}
