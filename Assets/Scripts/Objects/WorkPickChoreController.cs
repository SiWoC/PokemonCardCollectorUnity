using Assets.Scripts.Classes.Globals;
using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPickChoreController : MonoBehaviour
{
    public GameObject tutorialPickChorePanel;

    void Start()
    {
        tutorialPickChorePanel.SetActive(PlayerStats.GetShowTutorialStep(TutorialStep.PickChore));
    }

    public void OnRotomClicked()
    {
        PlayerStats.SetTutorialStepCompleted(TutorialStep.PickChore);
        tutorialPickChorePanel.SetActive(false);
        GameManager.Forward("Assets/Scenes/WorkChargingRotom.unity");
    }
}
