using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    UPPER,
    STRIKE,
    MIDDLE,
    LOWER,
    WAIT,
    START,
    MOVE
}

[System.Serializable]
public struct ActionData
{
    [Tooltip("何連符か、何のアクションがあるのか")] public Action[] action;
}


[CreateAssetMenu]
public class BeatData : ScriptableObject
{
    public ActionData[] actionDatas;
}
