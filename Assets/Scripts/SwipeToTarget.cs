using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeToTarget : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject deactivateOnDone;
    public GameObject target;
    public Vector3 offset;
    private float easing = 0.7f;
    private Vector3 startPosition;
    private Vector3 startingScale;

    void Start()
    {
        startPosition = transform.localPosition;
        startingScale = transform.localScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData data)
    {
        StartCoroutine(SmoothMove(transform.localPosition, transform.localScale));
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 startScale)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / easing;
            transform.localPosition = Vector3.Lerp(startpos, target.transform.localPosition + offset, Mathf.SmoothStep(0f, 1f, t));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        if (deactivateOnDone != null)
        {
            deactivateOnDone.SetActive(false);
        }
        transform.localPosition = startPosition;
        transform.localScale = startingScale;
    }
}
