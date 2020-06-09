using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public bool up,right,left,down;

    public Action inputAction;
    public bool buttonDownComand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        up = Input.GetKeyDown(KeyCode.UpArrow);
        right = Input.GetKeyDown(KeyCode.RightArrow);
        left = Input.GetKeyDown(KeyCode.LeftArrow);
        down = Input.GetKeyDown(KeyCode.DownArrow);

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
    }
}
