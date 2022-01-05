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
        {Action.WAIT,"Default" },
        {Action.MOVE,"Move" }
    };

    private Dictionary<Action, string> playActionAnimName = new Dictionary<Action, string>() {
        {Action.UPPER, "Upper" },
        {Action.STRIKE,"Strike" },
        {Action.MIDDLE,"Middle" },
        {Action.LOWER,"Lower" },
        {Action.WAIT,"Default" },
        {Action.MOVE,"Move" }
    };

    [SerializeField] private float actionTime;
    private float actionTimer;
    private bool playsAction;
    private Action playingAction = Action.WAIT;
    public bool isError;

    private void Awake()
    {
        simpleAnimation = GetComponent<SimpleAnimation>();
    }

    public void PlayStartAnim()
    {
        simpleAnimation.Play("Start");
    }

    public void PlayDamageAnim()
    {
        simpleAnimation.Play("Damage");
    }

    public void PlayBackAnim()
    {
        simpleAnimation.Play("Back");
    }

    public void Pause()
    {
        simpleAnimation.Stop();
    }

    public void PlayErrorAnim()
    {
        simpleAnimation.Play("Error");
        isError = true;
    }

    public void PlayPreAnim(Action action)
    {
        simpleAnimation.Play(playPreAnimName[action]);
    }

    public void PlayActionAnim(Action action)
    {
        simpleAnimation.Play(playActionAnimName[action]);
    }
}
