using Assets.Scripts.Classes.Globals;
using Globals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoreHolderButton : MonoBehaviour
{
    public GameObject animatedImage;
    public Sprite[] frames;
    public TextMeshProUGUI tutorialTargetCoinsText;

    private Image animatedSprite;
    private int frameNumber = 0;
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        animatedSprite = animatedImage.GetComponent<Image>();
    }

    public void OnClicked()
    {
        if (direction == -1 && frameNumber == 0)
        {
            // going up
            direction = 1;
        } else if (direction == 1 && frameNumber == frames.Length - 1)
        {
            // going down
            direction = -1;
        }
        frameNumber += direction;
        animatedSprite.sprite = frames[frameNumber];
        GameManager.ChoreClick();

        if (PlayerStats.GetShowTutorialStep(TutorialStep.EarnCoins) && PlayerStats.GetCoins() >= GameManager.GetPriceInCents(1))
        {
            PlayerStats.SetTutorialStepCompleted(TutorialStep.EarnCoins);
            tutorialTargetCoinsText.text = "Go to the Shop to spend those Coins.";
        }

    }

}
