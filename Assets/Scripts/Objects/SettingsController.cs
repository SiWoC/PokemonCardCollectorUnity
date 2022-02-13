using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider showTutorial;

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
    }

    // Update is called once per frame
    void Update()
    {
        
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
