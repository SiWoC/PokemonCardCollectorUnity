using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPickChoreController : MonoBehaviour
{
    public void OnRotomClicked()
    {
        GameManager.Forward("Assets/Scenes/WorkChargingRotom.unity");
    }
}
