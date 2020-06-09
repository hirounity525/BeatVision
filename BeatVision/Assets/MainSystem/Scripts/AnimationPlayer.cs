using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    SimpleAnimation simpleAnimation;

    private Dictionary<Action, string> playPreAnimName = new Dictionary<Action, string>() {
        {Action.UPPER, "PreUpper" },
        {Action.STRIKE,"PreStrike" },
        {Action.MIDDLE,"PreMiddle" },
        {Action.LOWER,"PreLower" },
        {Action.WAIT,"Default" }
    };

    private Dictionary<Action, string> playActionAnimName = new Dictionary<Action, string>() {
        {Action.UPPER, "Upper" },
        {Action.STRIKE,"Strike" },
        {Action.MIDDLE,"Middle" },
        {Action.LOWER,"Lower" },
        {Action.WAIT,"Default" }
    };

    [SerializeField] private float actionTime;
    private float actionTimer;
    private bool playsAction;
    private Action playingAction = Action.WAIT;

    private void Awake()
    {
        simpleAnimation = GetComponent<SimpleAnimation>();
    }

    public void PlayStartAnim()
    {
        simpleAnimation.Play("Start");
    }

    public void PlayPreAnim(Action action)
    {
        if(action == Action.WAIT)
        {
            if (!playsAction)
            {
                simpleAnimation.Play(playPreAnimName[action]);
            }
        }
        else
        {
            if(playingAction != action)
            {
                playsAction = true;
                actionTimer = 0;
                playingAction = action;
                simpleAnimation.Play(playPreAnimName[action]);
            }
        }

        if (playsAction)
        {
            if (actionTimer >= actionTime)
            {
                playingAction = Action.WAIT;
                playsAction = false;
            }
            else
            {
                actionTimer += Time.deltaTime;
            }
        }
    }

    public void PlayActionAnim(Action action)
    {
        if (action == Action.WAIT)
        {
            if (!playsAction)
            {
                simpleAnimation.Play(playPreAnimName[action]);
            }
        }
        else
        {
            if (playingAction != action)
            {
                playsAction = true;
                actionTimer = 0;
                playingAction = action;
                simpleAnimation.Play(playActionAnimName[action]);
            }
        }

        if (playsAction)
        {
            if (actionTimer >= actionTime)
            {
                playingAction = Action.WAIT;
                playsAction = false;
            }
            else
            {
                actionTimer += Time.deltaTime;
            }
        }
    }
}
