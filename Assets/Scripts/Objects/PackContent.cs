using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackContent : MonoBehaviour
{
    public GameObject normalCardsHolder;
    public GameObject specialCardsHolder;
    public GameObject wrapper;

    private Animator animator;
    private bool openedFired = false;
    private SpriteRenderer[] normalCardsSR;
    private SpriteRenderer[] specialCardsSR;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        normalCardsSR = normalCardsHolder.GetComponentsInChildren<SpriteRenderer>(true);
        specialCardsSR = specialCardsHolder.GetComponentsInChildren<SpriteRenderer>(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!openedFired && wrapper.transform.position.y < -70)
        {
            Debug.Log("firing");
            animator.SetTrigger("Opened");
            openedFired = true;
        }
    }

    public void NormalCardsToBack()
    {
        int i = 1;
        // normal cards to the back during swap
        foreach (SpriteRenderer sr in normalCardsSR)
        {
            sr.sortingOrder = i;
            i++;
        }
        i = 11;
        // special cards to the front during swap
        foreach (SpriteRenderer sr in specialCardsSR)
        {
            sr.sortingOrder = i;
            i++;
        }
    }

    public void NormalCardsToFront()
    {
        int i = 11;
        // normal cards to the front during rotation
        foreach (SpriteRenderer sr in normalCardsSR)
        {
            sr.sortingOrder = i;
            i++;
        }
        i = 1;
        // special cards to the back during rotation
        foreach (SpriteRenderer sr in specialCardsSR)
        {
            sr.sortingOrder = i;
            i++;
        }
    }
}
