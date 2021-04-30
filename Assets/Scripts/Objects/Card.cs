using UnityEngine;
using Factories.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Factories;

public class Card : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private bool showingFront;

    public Sprite front;
    public Sprite back;

    private PossibleCard createdFrom;

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

}
