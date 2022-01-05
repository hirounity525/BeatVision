using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputProvider : MonoBehaviour
{
    public bool up,right,left,down;

    public Action inputAction;
    public bool buttonDownComand;

    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Move
        up = player.GetButtonDown("Upper");
        right = player.GetButtonDown("Strike");
        left = player.GetButton("Barrier");
        down = player.GetButtonDown("Lower");

        if (up)
        {
            inputAction = Action.UPPER;
            buttonDownComand = true;
        }
        else if (right)
        {
            inputAction = Action.STRIKE;
            buttonDownComand = true;
        }
        else if (left)
        {
            inputAction = Action.MIDDLE;
            buttonDownComand = true;
        }
        else if (down)
        {
            inputAction = Action.LOWER;
            buttonDownComand = true;
        }
        else
        {
            buttonDownComand = false;
        }

        //Attack
        
    }
}
