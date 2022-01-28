using UnityEngine;
using Factories.Config;
using UnityEngine.EventSystems;
using Factories;
using System.Collections;
using System;
using Globals;
using Assets.Scripts.Classes.Globals;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static event Action CardDestroyEvent;

    public Sprite front;
    public Sprite back;
    private Vector3 endPosition = new Vector3(4.5f, 30.0f, 0f);

    private SpriteRenderer spriteRenderer;
    private bool showingFront;
    private float easing = 0.7f;
    private PossibleCard createdFrom;
    private Canvas parentCanvas;
    private Vector3 moveOffset;

    public PossibleCard CreatedFrom 
    { 
        get => createdFrom;
        set {
            createdFrom = value;
            StartCoroutine(CardFactory.FillSprite(ImageType.Large, createdFrom.imageUrlLarge, this));
        }

    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spriteRenderer.sprite = back;
        endPosition += new Vector3(0, 0, transform.position.z);
        GetComponent<BoxCollider2D>().enabled = false;
        parentCanvas = GetComponentsInParent<Canvas>()[0];
    }

    // Update is called once per frame
    void Update()
    {

        if (((transform.eulerAngles.y - 90) % 360) < 0)
        {
            // go to showingFrontside non-flipped = (showingFront == true)
            if (!showingFront) // we need to change things
            {
                showingFront = true;
                spriteRenderer.sprite = front;
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            // go to showingBackside X-flipped = (showingFront == false)
            if (showingFront) // we need to change things
            {
                showingFront = false;
                spriteRenderer.sprite = back;
                spriteRenderer.flipX = true;
            }
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {

        //For Offset Position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out Vector2 pos);
        moveOffset = transform.position - parentCanvas.transform.TransformPoint(pos);

    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out Vector2 pos);
        transform.position = parentCanvas.transform.TransformPoint(pos) + moveOffset;
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!PlayerStats.GetTutorialCompleted(TutorialStep.SwipeToBook))
        {
            PlayerStats.SetTutorialCompleted(TutorialStep.SwipeToBook);
        }
        StartCoroutine(SmoothMove(transform.position, transform.localScale));
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 startScale)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / easing;
            transform.position = Vector3.Lerp(startpos, endPosition, Mathf.SmoothStep(0f, 1f, t));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        // self-destruct
        CardDestroyEvent?.Invoke();
        Destroy(gameObject);
    }

}
