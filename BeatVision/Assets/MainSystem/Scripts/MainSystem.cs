using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn
{
    ATTACK,
    DEFENSE
}

public enum Part
{
    GLARING,
    PRE,
    MOVEMENT
}

[System.Serializable]
public struct ActionTiming
{
    public Action action;
    public float timing;
    public bool isCheck;
}

public class MainSystem : MonoBehaviour
{
    public Turn turn;
    [SerializeField] private Part part;
    [SerializeField] private List<ActionTiming> actionTimings = new List<ActionTiming>();
    [SerializeField] private AnimationPlayer enemyAnim;
    [SerializeField] private AnimationPlayer playerAnim;
    [SerializeField] private HPDamager playerDamager;
    [SerializeField] private HPDamager enemyDamager;
    [SerializeField] private PostProcessController postProcess;

    [Header("ビートと次のビートの間")]
    [SerializeField] private float minBeatInterval;
    [SerializeField] private float maxBeatInterval;

    [Header("アクションのインターバル（譜刻み）")]
    [SerializeField] private float actionInterval;
    [SerializeField] private int maxActionNum;

    [Header("判定")]
    [SerializeField] private float goodInterval;
    [SerializeField] private float excelentInterval;
    [SerializeField] private EffectPlayer effectPlayer;

    InputProvider inputProvider;
    [SerializeField] BeatDataLoader enemyDataLoader;
    [SerializeField] BeatDataLoader playerDataLoader;
    ModeChangeJudger changeJudger;

    float actionTimer;
    bool startsGlaringPart;
    bool startsPrePart;
    bool startsMovementPart;

    //にらみ合い
    float beatInterval;
    bool getsBeatInterval;

    //予備動作
    BeatData beatData;
    int actionDataNum;
    bool loadsBeatData;

    float saveActionDataTime;

    ActionData actionData;
    int nowActionDataNum;
    int actionNum;
    bool loadsActionData;

    Action action;
    int nowActionNum;
    bool loadsAction;

    float startTimer;

    //行動
    bool isResetMovement;
    int nowCheckNum;

    bool loadsActionTiming;
    ActionTiming checkActionTiming;

    private void Awake()
    {
        inputProvider = GetComponent<InputProvider>();
        changeJudger = GetComponent<ModeChangeJudger>();
    }

    private void Update()
    {
        switch (turn)
        {
            case Turn.DEFENSE:
                switch (part)
                {
                    case Part.GLARING:

                        Glaring();

                        if (startsPrePart)
                        {
                            postProcess.ChangePrePostProcess();
                            startsGlaringPart = false;
                            part = Part.PRE;
                        }
                        break;
                    case Part.PRE:

                        PreAction(true);

                        if (startsMovementPart)
                        {
                            startsPrePart = false;
                            part = Part.MOVEMENT;
                        }
                        break;
                    case Part.MOVEMENT:

                        Movement(true);

                        if (startsGlaringPart)
                        {
                            startsMovementPart = false;
                            if (changeJudger.changesTurn)
                            {
                                postProcess.ChangePrePostProcess();
                                changeJudger.changesTurn = false;
                                startsGlaringPart = false;
                                part = Part.PRE;
                                turn = Turn.ATTACK;
                                return;
                            }
                            part = Part.GLARING;
                        }
                        break;
                }
                break;
            case Turn.ATTACK:
                switch (part)
                {
                    case Part.PRE:
                        PreAction(false);
                        if (startsMovementPart)
                        {
                            startsPrePart = false;
                            part = Part.MOVEMENT;
                        }
                        break;
                    case Part.MOVEMENT:
                        Movement(false);
                        if (startsGlaringPart)
                        {
                            startsMovementPart = false;
                            part = Part.GLARING;
                            turn = Turn.DEFENSE;
                        }
                        break;
                }
                break;
        }

    }

    private void Glaring()
    {
        enemyAnim.PlayPreAnim(Action.WAIT);

        if (!getsBeatInterval)
        {
            startTimer = 0;
            beatInterval = Random.Range(minBeatInterval, maxBeatInterval);
            getsBeatInterval = true;
        }

        if(startTimer >= beatInterval)
        {
            getsBeatInterval = false;
            startsPrePart = true;
        }
        else
        {
            startTimer += Time.deltaTime;
        }
    }

    private void PreAction(bool isDefense)
    {
        //データを読み込む・リセット
        if (!loadsBeatData)
        {
            if (isDefense)
            {
                beatData = enemyDataLoader.LoadBeatData();
            }
            else
            {
                beatData = playerDataLoader.LoadRandomBeatData();
            }

            actionDataNum = beatData.actionDatas.Length;

            actionTimer = 0;
            nowActionDataNum = 0;
            nowActionNum = 0;
            startTimer = 0;

            loadsActionData = false;
            loadsAction = false;

            actionTimings.Clear();

            loadsBeatData = true;
            return;
        }

        if(nowActionDataNum < actionDataNum)
        {
            //データからアクションデータを読み取る
            if (!loadsActionData)
            {
                actionData = beatData.actionDatas[nowActionDataNum];
                actionNum = actionData.action.Length;

                nowActionNum = 0;
                saveActionDataTime = actionTimer;

                loadsActionData = true;
            }

            if (nowActionNum < actionNum)
            {
                //アクションデータからアクションを読み取る
                if (!loadsAction)
                {
                    action = actionData.action[nowActionNum];
                    loadsAction = true;
                }

                //アクションがスタートなのか
                if (action != Action.START)
                {
                    if (actionTimer >= saveActionDataTime + (nowActionNum * actionInterval))
                    {
                        if (isDefense)
                        {
                            enemyAnim.PlayPreAnim(action);
                        }
                        else
                        {
                            playerAnim.PlayPreAnim(action);
                        }

                        if (action != Action.WAIT)
                        {
                            SaveActionData(action, actionTimer);
                        }

                        nowActionNum++;
                        loadsAction = false;
                    }
                    
                }
                else
                {
                    if(actionTimer >= saveActionDataTime + (nowActionNum * actionInterval))
                    {
                        if (startTimer >= actionInterval)
                        {
                            loadsBeatData = false;
                            startsMovementPart = true;
                        }
                        else
                        {
                            postProcess.ChangeMovementPostProcess();
                            if (isDefense)
                            {
                                enemyAnim.PlayStartAnim();
                            }
                            else
                            {
                                playerAnim.PlayStartAnim();
                            }
                            startTimer += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                if (isDefense)
                {
                    enemyAnim.PlayPreAnim(Action.WAIT);
                }
                else
                {
                    playerAnim.PlayPreAnim(Action.WAIT);
                }

                //次のアクションデータを読み取るまで待ち
                if (actionTimer >= saveActionDataTime + (maxActionNum * actionInterval))
                {
                    nowActionDataNum++;
                    loadsActionData = false;
                }
            }

            //タイマープラス
            actionTimer += Time.deltaTime;
        }
        else
        {
            if (startTimer >= actionInterval)
            {
                loadsBeatData = false;
                startsMovementPart = true;
            }
            else
            {
                postProcess.ChangeMovementPostProcess();
                if (isDefense)
                {
                    enemyAnim.PlayStartAnim();
                }
                else
                {
                    playerAnim.PlayStartAnim();
                }
                startTimer += Time.deltaTime;
            }
        }
    }

    private void SaveActionData(Action action, float timing)
    {
        ActionTiming actionTiming;
        actionTiming.action = action;
        actionTiming.timing = timing;
        actionTiming.isCheck = false;

        actionTimings.Add(actionTiming);
    }

    private void Movement(bool isDefense)
    {
        if (!isResetMovement)
        {
            actionTimer = 0;
            nowCheckNum = 0;

            isResetMovement = true;
        }

        if(nowCheckNum < actionTimings.Count)
        {
            //チェックするアクションタイミングを読み取る
            if (!loadsActionTiming)
            {
                checkActionTiming = actionTimings[nowCheckNum];

                loadsActionTiming = true;
            }

            if(actionTimer <= checkActionTiming.timing + goodInterval)
            {
                float diff = Mathf.Abs(checkActionTiming.timing - actionTimer);

                if (isDefense)
                {
                    if (diff <= goodInterval)
                    {
                        enemyAnim.PlayActionAnim(checkActionTiming.action);
                    }
                    else
                    {
                        enemyAnim.PlayActionAnim(Action.WAIT);
                    }
                }
                else
                {
                    playerAnim.PlayActionAnim(Action.WAIT);
                }

                if (inputProvider.buttonDownComand)
                {
                    if(diff <= excelentInterval)
                    {
                        checkActionTiming.isCheck = true;

                        if(inputProvider.inputAction == checkActionTiming.action)
                        {
                            if (isDefense)
                            {
                                effectPlayer.PlayExcellentEffect();
                                changeJudger.AddExcellentPoint();
                            }
                            else
                            {
                                playerAnim.PlayActionAnim(checkActionTiming.action);
                                enemyDamager.Damage();
                            }
                        }
                        else
                        {
                            if (isDefense)
                            {
                                playerDamager.Damage();
                            }
                            else
                            {
                                //effectPlayer.PlayExcellentEffect();
                            }
                        }

                        nowCheckNum++;
                        loadsActionTiming = false;
                    }
                    else if(diff <= goodInterval)
                    {
                        checkActionTiming.isCheck = true;

                        if (inputProvider.inputAction == checkActionTiming.action)
                        {
                            if (isDefense)
                            {
                                effectPlayer.PlayGoodEffect();
                                changeJudger.AddGoodPoint();
                            }
                            else
                            {
                                playerAnim.PlayActionAnim(checkActionTiming.action);
                                enemyDamager.smallDamage();
                                effectPlayer.PlayGoodEffect();
                            }
                        }
                        else
                        {
                            if (isDefense)
                            {
                                playerDamager.Damage();
                            }
                            else
                            {
                                //effectPlayer.PlayExcellentEffect();
                            }
                        }

                        nowCheckNum++;
                        loadsActionTiming = false;
                    }
                }
            }
            else
            {
                if (!checkActionTiming.isCheck)
                {
                    if (isDefense)
                    {
                        playerDamager.Damage();
                    }
                    else
                    {
                        //effectPlayer.PlayExcellentEffect();
                    }
                    nowCheckNum++;
                    loadsActionTiming = false;
                }
            }

            //タイマープラス
            actionTimer += Time.deltaTime;
        }
        else
        {
            isResetMovement = false;
            startsGlaringPart = true;
        }
    }
}
