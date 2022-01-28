using Assets.Scripts.Classes.Globals;
using Factories;
using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpeningController : MonoBehaviour
{
    public GameObject generationsHolder;
    public GameObject openPackButtonPrefab;
    public Canvas packCanvas;
    public GameObject generationUnlockedPanel;
    public TextMeshProUGUI generationUnlockedText;
    public GameObject backButton;
    public GameObject tutorialSelectPackPanel;
    public GameObject tutorialOpenPackPanel;
    public GameObject tutorialSwipeToBookPanel;

    private GameObject packInstance;
    private bool showGenerationUnlockedOnBack = false;

    private void Awake()
    {
        GameManager.Initialize();
        PackContent.AllCardsSwipedEvent += AllCardsSwipedEvent;
        Pack.OpenedEvent += PackOpened;
        GameManager.GenerationUnlockedEvent += GenerationUnlocked;
    }

    private void OnDestroy()
    {
        PackContent.AllCardsSwipedEvent -= AllCardsSwipedEvent;
        Pack.OpenedEvent -= PackOpened;
        GameManager.GenerationUnlockedEvent -= GenerationUnlocked;
    }

    // Start is called before the first frame update
    void Start()
    {
        packCanvas.gameObject.SetActive(false);
        tutorialSelectPackPanel.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.SelectPack));
        tutorialOpenPackPanel.SetActive(false);
        for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
        {
            GenerateOpenPackButton(i);
        }
    }

    private void Update()
    {
        tutorialSelectPackPanel.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.SelectPack));
        // this might be responsibility of the Pack/Wrapper, but it is easier here on a 2D canvas
        tutorialOpenPackPanel.SetActive(packCanvas.gameObject.activeSelf && 
                                        !PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack));
    }

    private void GenerateOpenPackButton(int generation)
    {
        GameObject openPackButton = GameObject.Instantiate(openPackButtonPrefab);
        BtnOpenPack btnOpenPack = openPackButton.GetComponent<BtnOpenPack>();
        btnOpenPack.generation = generation;
        btnOpenPack.sceneController = this.gameObject;
        openPackButton.transform.SetParent(generationsHolder.transform);
        openPackButton.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnGenerationClick(int generation) 
    {
        PlayerStats.SetTutorialCompleted(TutorialStep.SelectPack);
        generationsHolder.SetActive(false);
        packCanvas.gameObject.SetActive(true);
        if (packInstance != null)
        {
            Destroy(packInstance);
        }
        packInstance = CardFactory.GetPack(generation);
        packInstance.transform.SetParent(packCanvas.transform);
        packInstance.transform.localPosition = new Vector3(0f, 0f, 0f);
        packInstance.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void GenerationUnlocked()
    {
        showGenerationUnlockedOnBack = true;
    }

    IEnumerator ShowGenerationUnlocked()
    {
        generationUnlockedText.text = "You unlocked\r\ngeneration " + PlayerStats.GetHighestUnlockedGeneration() + "!!!";
        generationUnlockedPanel.SetActive(true);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        generationUnlockedPanel.SetActive(false);
        showGenerationUnlockedOnBack = false;
    }

    private void PackOpened(int generation)
    {
        backButton.SetActive(false);
    }

    private void AllCardsSwipedEvent()
    {
        backButton.SetActive(true);
        OnBack();
    }

    public void OnBack()
    {
        if (packCanvas.gameObject.activeSelf)
        {
            packCanvas.gameObject.SetActive(false);
            if (showGenerationUnlockedOnBack)
            {
                StartCoroutine(ShowGenerationUnlocked());
            }
            generationsHolder.SetActive(true);
        }
        else
        {
            GameManager.Back();
        }
    }

}
