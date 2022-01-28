using Assets.Scripts.Classes.Globals;
using Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackContent : MonoBehaviour
{

    public static event Action AllCardsSwipedEvent;

    public GameObject normalCardsHolder;
    public GameObject specialCardsHolder;
    public GameObject tutorialSpriteSwipeToBook;
    public GameObject tutorialTextSwipeToBook;

    private Animator animator;
    private SpriteRenderer[] normalCardsSR;
    private Transform[] normalCardsTF;
    private SpriteRenderer[] specialCardsSR;
    private BoxCollider2D[] cardsColliders;
    private float swapDelay = 0.25f;
    private int numberOfCards = 10;
    private bool cardsEnabled;

    private void Awake()
    {
        Card.CardDestroyEvent += CardDeletedFromContent;
    }

    private void OnDestroy()
    {
        Card.CardDestroyEvent -= CardDeletedFromContent;
    }

    private void CardDeletedFromContent()
    {
        numberOfCards--;
        if (numberOfCards == 0)
        {
            AllCardsSwipedEvent?.Invoke();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        normalCardsSR = normalCardsHolder.GetComponentsInChildren<SpriteRenderer>(true);
        normalCardsTF = normalCardsHolder.GetComponentsInChildren<Transform>(true);
        specialCardsSR = specialCardsHolder.GetComponentsInChildren<SpriteRenderer>(true);
        cardsColliders = GetComponentsInChildren<BoxCollider2D>(true);
        tutorialSpriteSwipeToBook.SetActive(false);
        tutorialTextSwipeToBook.SetActive(false);
    }

    private void Update()
    {
        if (cardsEnabled)
        {
            tutorialSpriteSwipeToBook.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.SwipeToBook));
            tutorialTextSwipeToBook.SetActive(!PlayerStats.GetTutorialCompleted(TutorialStep.SwipeToBook));
        }
    }
    public void Opened()
    {
        int i = 1;
        // normal cards fanning out before swap
        foreach (Transform tf in normalCardsTF)
        {
            StartCoroutine(SmoothMove(tf, tf.position, new Vector3(i, i, 0f)));
            i++;
        }
        animator.SetTrigger("Opened");
    }

    //Animation Event
    public void EnableCards()
    {
        // normal cards fanning back before rotate
        foreach (BoxCollider2D bc in cardsColliders)
        {
            bc.enabled = true;
        }
        cardsEnabled = true;
    }

    //Animation Event
    public void NormalCardsToZero()
    {
        // normal cards fanning back before rotate
        foreach (Transform tf in normalCardsTF)
        {
            StartCoroutine(SmoothMove(tf, tf.position, new Vector3(0f, 0f, 0f)));
            //tf.position = new Vector3(0f,0f, 0f);
        }
    }

    //Animation Event
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

    //Animation Event
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

    IEnumerator SmoothMove(Transform tf, Vector3 startpos, Vector3 endpos)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / swapDelay;
            tf.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
