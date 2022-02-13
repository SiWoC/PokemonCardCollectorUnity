using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider showTutorial;
    public Button restarTutorialButton;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerStats.GetShowTutorial())
        {
            showTutorial.value = 1;
        } else
        {
            showTutorial.value = 0;
        }
        restarTutorialButton.interactable = (PlayerStats.GetAvailablePacks() > 0);
    }

    public void OnShowTutorialChanged()
    {
        PlayerStats.SetShowTutorial(showTutorial.value == 1);
    }

    public void OnRestartTutorialClicked()
    {
        PlayerStats.RestartTutorial();
        showTutorial.value = 1;
    }
}
