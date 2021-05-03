using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnOpenPack : MonoBehaviour
{

    public int generation;
    public Text text;
    public GameObject counter;
    public GameObject sceneController;

    private OpeningController openingController;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        openingController = sceneController.GetComponent<OpeningController>();
        text.text = "Generation " + generation;
        counter.GetComponent<PacksOwnedCounter>().generation = generation;
        button = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        
        button.interactable = (PlayerStats.GetAvailablePacks(generation) > 0);
    }

    public void OnClick()
    {
        openingController.OnGenerationClick(generation);
    }
}
