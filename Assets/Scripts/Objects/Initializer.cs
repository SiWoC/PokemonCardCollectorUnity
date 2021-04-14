using Factories;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite roundedBack;
    public Sprite squareBack;

    private void Awake()
    {
        CardFactory.cardPrefab = cardPrefab;
        CardFactory.roundedBack = roundedBack;
        CardFactory.squareBack = squareBack;
    }

}
