using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAnimPlayer : MonoBehaviour
{
    SimpleAnimation simpleAnimation;
    [SerializeField] InputProvider inputProvider;

    [SerializeField]MainSystem mainSystem;

    private void Awake()
    {
        simpleAnimation = GetComponent<SimpleAnimation>();
    }

    private void Update()
    {
        if(mainSystem.turn == Turn.ATTACK)
        {
            return;
        }

        if (inputProvider.up)
        {
            simpleAnimation.Play("GuardUpper");
        }
        else if (inputProvider.right)
        {
            simpleAnimation.Play("GuardStrike");
        }
        else if (inputProvider.left)
        {
            simpleAnimation.Play("GuardMiddle");
        }
        else if (inputProvider.down)
        {
            simpleAnimation.Play("GuardLower");
        }
    }
}
