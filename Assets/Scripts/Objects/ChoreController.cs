using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoreController : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public GameObject randomPackEarnedPanel;
    public TextMeshProUGUI randomPackEarnedText;
    public Image randomPackImage;
    public Vector3 endPosition = new Vector3(-14.0f, 28.0f, 0f);
    private float easing = 0.7f;

    private void Awake()
    {
        GameManager.Initialize();
        GameManager.RandomPackEarnedEvent += RandomPackEarnedEvent;
    }

    private void OnDestroy()
    {
        GameManager.RandomPackEarnedEvent -= RandomPackEarnedEvent;
    }

    private void RandomPackEarnedEvent(int generation)
    {
        randomPackImage.sprite = Resources.Load<Sprite>("Images/Packs/" + PlayerStats.GetNextPack(generation)); // zonder png !!!
        randomPackEarnedText.text = "You earned\r\na pack of\r\ngeneration " + generation + "!!!";
        randomPackEarnedPanel.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        randomPackImage.transform.position += (Vector3)eventData.delta / 10; // deze 10 moet iets met         Screen.currentResolution.height
    }

    public void OnEndDrag(PointerEventData data)
    {
        StartCoroutine(SmoothMove(randomPackImage.transform.position, randomPackImage.transform.localScale));
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 startScale)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / easing;
            randomPackImage.transform.position = Vector3.Lerp(startpos, endPosition, Mathf.SmoothStep(0f, 1f, t));
            randomPackImage.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        randomPackEarnedPanel.SetActive(false);
    }
}
