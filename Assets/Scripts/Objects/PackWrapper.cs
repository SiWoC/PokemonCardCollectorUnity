using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackWrapper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public Sprite[] sprites;
    
    private float easing = 2f;
    private SpriteRenderer spriteRenderer;
    private bool opened = false;
    private int frame = 0;
    private int framesHandled = 0;
    public float stepWidthCoverage = 0.8f;
    private int stepWidth;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Images/Packs/BaseSet2_3_649x1080x7"); // zonder png !!!
        stepWidth = (int)((Screen.width / sprites.Length) * stepWidthCoverage * -1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!opened)
        {
            int numberOfFrames = (int)((eventData.pressPosition.x - eventData.position.x) / stepWidth);
            AddFrame(numberOfFrames);
        }
    }

    private void AddFrame(int numberOfFrames)
    {
        if (numberOfFrames > framesHandled)
        {
            frame = Math.Min(frame + numberOfFrames - framesHandled, sprites.Length - 1);
            framesHandled = numberOfFrames;
            spriteRenderer.sprite = sprites[frame];
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (frame == sprites.Length - 1)
        {
            opened = true;
            StartCoroutine(SmoothMove(transform.position, new Vector3(transform.position.x, -100, transform.position.z), easing));
        }
        framesHandled = 0;
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
