using Factories;
using Factories.Config;
using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pack : MonoBehaviour
{

    public delegate void OpenedEventHandler(int generation);
    public static event OpenedEventHandler OpenedEvent;

    public int generation;
    public GameObject stillLoading;
    public GameObject wrapper;
    public GameObject content;

    private bool openedFired = false;
    public List<PossibleCard> cardInThisPack;

    // Start is called before the first frame update
    void Start()
    {
        wrapper.GetComponent<BoxCollider>().enabled = false;
    }
    
    private void Awake()
    {
        GameManager.Initialize();
        CardFactory.PackReadyEvent += OnPackReady;
    }

    private void OnDestroy()
    {
        CardFactory.PackReadyEvent -= OnPackReady;
    }

    // Update is called once per frame
    void Update()
    {
        if (!openedFired && wrapper.transform.position.y < -70)
        {
            content.GetComponent<PackContent>().Opened();
            openedFired = true;
        }
    }

    public void OnPackReady()
    {
        stillLoading.SetActive(false);
        wrapper.GetComponent<BoxCollider>().enabled = true;
    }

    internal void Opened()
    {
        GameManager.OpenedPack(generation, cardInThisPack);
        OpenedEvent?.Invoke(generation);
    }
}
