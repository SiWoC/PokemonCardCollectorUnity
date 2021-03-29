﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardConstants;

public class Card : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool showingFront;

    public CardShape cardShape;
    public Sprite front;
    public Sprite back;

    public string url;

    private void OnMouseDown()
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