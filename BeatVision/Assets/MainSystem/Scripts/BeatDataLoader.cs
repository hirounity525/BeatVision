using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDataLoader : MonoBehaviour
{
    public EnemyData enemyData;

    private int nowDataNum;

    public BeatData LoadBeatData()
    {
        if(nowDataNum >= enemyData.beatDatas.Count)
        {
            nowDataNum = 0;
        }
        int n = nowDataNum;
        nowDataNum++;
        return enemyData.beatDatas[n];
    }

    public BeatData LoadRandomBeatData()
    {
        int r = Random.Range(0, enemyData.beatDatas.Count);
        return enemyData.beatDatas[r];
    }
}
