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
    public Sprite noPacksAvailable;

    private OpeningController openingController;
    private Button button;
    private Image image;
    private bool refreshState = false;

    // Start is called before the first frame update
    void Start()
    {
        openingController = sceneController.GetComponent<OpeningController>();
        text.text = "Generation " + generation;
        counter.GetComponent<PacksOwnedCounter>().generation = generation;
        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();

        RefreshState();
    }

    void Update()
    {
        if (refreshState)
        {
            refreshState = false;
            RefreshState();
        }
    }

    private void RefreshState()
    {
        bool interactable = (PlayerStats.GetAvailablePacks(generation) > 0);
        if (interactable)
        {
            text.gameObject.SetActive(false);
            image.sprite = Resources.Load<Sprite>("Images/Packs/" + PlayerStats.GetNextPack(generation)); // zonder png !!!
        }
        else
        {
            text.gameObject.SetActive(true);
            image.sprite = noPacksAvailable;
        }
        button.interactable = interactable;
    }

    public void OnClick()
    {
        openingController.OnGenerationClick(generation);
        refreshState = true;
    }

}
