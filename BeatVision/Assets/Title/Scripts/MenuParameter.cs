using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Menu
{
    NEWGAME,
    CONTINUE,
    STAGESELECT,
    SCORE,
    OPTION,
    EXIT,
    MANUAL
}

public class MenuParameter : MonoBehaviour
{
    public Menu menu;
    public bool canSelect;
}
