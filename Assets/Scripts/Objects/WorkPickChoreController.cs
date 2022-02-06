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
        tutorialPickChorePanel.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.PickChore));
    }

    public void OnRotomClicked()
    {
        PlayerStats.SetTutorialCompleted(TutorialStep.PickChore);
        tutorialPickChorePanel.SetActive(false);
        GameManager.Forward("Assets/Scenes/WorkChargingRotom.unity");
    }
}
