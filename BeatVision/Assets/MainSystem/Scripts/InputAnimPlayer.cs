using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAnimPlayer : MonoBehaviour
{
    SimpleAnimation simpleAnimation;
    [SerializeField] InputProvider inputProvider;

    [SerializeField] private float actionTime;
    private float actionTimer;
    private bool isAction;

    public bool canInput = true;

    [SerializeField] private GameObject barrierEffect;
    [SerializeField] private GameObject barrierCanvas;

    private void Awake()
    {
        simpleAnimation = GetComponent<SimpleAnimation>();
    }

    private void Update()
    {
        if (!canInput)
        {
            return;
        }

        if (inputProvider.up)
        {
            simpleAnimation.Play("GuardUpper");
            isAction = true;
            actionTimer = 0;
        }
        else if (inputProvider.right)
        {
            simpleAnimation.Play("GuardStrike");
            isAction = true;
            actionTimer = 0;
        }
        else if (inputProvider.down)
        {
            simpleAnimation.Play("GuardLower");
            isAction = true;
            actionTimer = 0;
        }

        if (inputProvider.left)
        {
            simpleAnimation.Play("GuardMiddle");
            isAction = true;
            actionTimer = 0;

            barrierEffect.SetActive(true);
            barrierCanvas.SetActive(true);
        }
        else
        {
            barrierEffect.SetActive(false);
            barrierCanvas.SetActive(false);
        }

        if (isAction)
        {
            if(actionTimer >= actionTime)
            {
                isAction = false;
                simpleAnimation.Play("Default");
            }
            else
            {
                actionTimer += Time.deltaTime;
            }
        }
    }
}
