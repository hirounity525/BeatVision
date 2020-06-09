using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChangeJudger : MonoBehaviour
{
    public bool changesTurn;

    [SerializeField] private int changePoint;
    [SerializeField] private int excellentPoint;
    [SerializeField] private int goodPoint;

    private int nowChangePoint;

    // Update is called once per frame
    void Update()
    {
        if(nowChangePoint >= changePoint)
        {
            nowChangePoint = 0;
            changesTurn = true;
        }
    }

    public void AddGoodPoint()
    {
        nowChangePoint += goodPoint;
    }

    public void AddExcellentPoint()
    {
        nowChangePoint += excellentPoint;
    }


}
