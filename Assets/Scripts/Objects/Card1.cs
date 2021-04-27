using UnityEngine;
using Factories.Config;
using UnityEngine.EventSystems;

public class Card1 : MonoBehaviour, IPointerClickHandler
{

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool showingFront;

    public Sprite front;
    public Sprite back;

    public string url;
    public PossibleCard createdFrom;

    public void OnPointerClick(PointerEventData eventData)
    {
        animator.SetTrigger("Click");
        /*
        if (showingFront)
        {
            AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);
            asi.speed = -1f;
            animator.speed = -1f;
        }
        */
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
