using Globals;
using UnityEngine;

public class BtnBack : MonoBehaviour
{
    public void OnClicked()
    {
        GameManager.Back();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GameManager.Back();
        }
    }
}
