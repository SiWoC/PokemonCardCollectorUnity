using Factories;
using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningController : MonoBehaviour
{
    public Canvas packCanvas;

    private int selectedGeneration = 1;
    private GameObject packInstance;

    private void Awake()
    {
        GameManager.Initialize();
        CardFactory.packReadyEvent += OnPackReady;
    }

    private void OnDestroy()
    {
        CardFactory.packReadyEvent -= OnPackReady;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (packInstance != null)
        {
            Destroy(packInstance);
        }
        packInstance = CardFactory.GetPack(selectedGeneration);
        packInstance.transform.SetParent(packCanvas.transform);
        packInstance.transform.localPosition = new Vector3(0f, 0f, 0f);
        packInstance.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPackReady()
    {
        Debug.Log("enableing wrapper");
        GameObject stillLoading = packInstance.transform.Find("StillLoading").gameObject;
        stillLoading.SetActive(false);
        GameObject wrapper = packInstance.transform.Find("PackWrapper").gameObject;
        wrapper.GetComponent<BoxCollider2D>().enabled = true;
    }
}
