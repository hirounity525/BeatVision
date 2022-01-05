using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TitleInputProvider : MonoBehaviour
{
    [SerializeField] private int playerId;
    Player player;

    [Header("Input")]
    public bool upButtonDown;
    public bool downButtonDown;
    public bool rightButtonDown;
    public bool leftButtonDown;
    public bool selectButtonDown;
    public bool backButtonDown;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        upButtonDown = player.GetButton("Up");
        downButtonDown = player.GetButton("Down");
        rightButtonDown = player.GetButtonDown("Right");
        leftButtonDown = player.GetButtonDown("Left");
        selectButtonDown = player.GetButtonDown("Select");
        backButtonDown = player.GetButtonDown("Back");
    }
}
