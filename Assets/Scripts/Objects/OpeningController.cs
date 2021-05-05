using Factories;
using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningController : MonoBehaviour
{
    public GameObject generationsHolder;
    public GameObject openPackButtonPrefab;
    public Canvas packCanvas;

    private GameObject packInstance;

    private void Awake()
    {
        GameManager.Initialize();
        PackContent.AllCardsSwipedEvent += OnBack;
    }

    private void OnDestroy()
    {
        PackContent.AllCardsSwipedEvent -= OnBack;
    }

    // Start is called before the first frame update
    void Start()
    {
        packCanvas.gameObject.SetActive(false);
        for (int i = 1; i <= CardFactory.numberOfGenerations; i++)
        {
            GenerateOpenPackButton(i);
        }
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

    public void OnBack()
    {
        if (packCanvas.gameObject.activeSelf)
        {
            packCanvas.gameObject.SetActive(false);
            generationsHolder.SetActive(true);
        }
        else
        {
            GameManager.Back();
        }
    }

}
