using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackContent : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject normalCards;
    public GameObject specialCards;

    private Transform normalCardsTransform;
    private Transform specialCardsTransform;
    private Vector3 normalCardsLocation;
    private Vector3 specialCardsLocation;
    private Animator animator;
    private float highestY = 0;

    // Start is called before the first frame update
    void Start()
    {
        normalCardsTransform = normalCards.transform;
        normalCardsLocation = normalCardsTransform.position;
        specialCardsTransform = specialCards.transform;
        specialCardsLocation = specialCardsTransform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        float difference = eventData.pressPosition.x - eventData.position.x;
        normalCardsTransform.position = normalCardsLocation - new Vector3(difference/10, 0, 0);
        //normalCardsLocation =  eventData.pressPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        animator.SetTrigger("FinishSwap");
    }
}
