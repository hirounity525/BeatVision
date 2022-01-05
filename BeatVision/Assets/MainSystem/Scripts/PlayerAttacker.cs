using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
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

    private void PreAction()
    {
        //データを読み込む・リセット
        if (!loadsBeatData)
        {
            beatData = playerDataLoader.LoadRandomBeatData();

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

        if (nowActionDataNum < actionDataNum)
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
                        playerAnim.PlayPreAnim(action);

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
                    if (actionTimer >= saveActionDataTime + (nowActionNum * actionInterval))
                    {
                        if (startTimer >= actionInterval)
                        {
                            loadsBeatData = false;
                            startsMovementPart = true;
                        }
                        else
                        {
                            postProcess.ChangeMovementPostProcess();
                            playerAnim.PlayStartAnim();
                            startTimer += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                playerAnim.PlayPreAnim(Action.WAIT);

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
                playerAnim.PlayStartAnim();
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

    private void Movement()
    {
        if (!isResetMovement)
        {
            actionTimer = 0;
            nowCheckNum = 0;

            isResetMovement = true;
        }

        if (nowCheckNum < actionTimings.Count)
        {
            //チェックするアクションタイミングを読み取る
            if (!loadsActionTiming)
            {
                checkActionTiming = actionTimings[nowCheckNum];

                loadsActionTiming = true;
            }

            if (actionTimer <= checkActionTiming.timing + goodInterval)
            {
                float diff = Mathf.Abs(checkActionTiming.timing - actionTimer);

                playerAnim.PlayActionAnim(Action.WAIT);

                if (inputProvider.buttonDownComand)
                {
                    if (diff <= excelentInterval)
                    {
                        checkActionTiming.isCheck = true;

                        if (inputProvider.inputAction == checkActionTiming.action)
                        {
                            playerAnim.PlayActionAnim(checkActionTiming.action);
                            enemyDamager.Damage();
                        }
                        else
                        {
                            //effectPlayer.PlayExcellentEffect();
                        }

                        nowCheckNum++;
                        loadsActionTiming = false;
                    }
                    else if (diff <= goodInterval)
                    {
                        checkActionTiming.isCheck = true;

                        if (inputProvider.inputAction == checkActionTiming.action)
                        {
                            playerAnim.PlayActionAnim(checkActionTiming.action);
                            enemyDamager.smallDamage();
                            effectPlayer.PlayGoodEffect();
                        }
                        else
                        {
                            //effectPlayer.PlayExcellentEffect();
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
                    //effectPlayer.PlayExcellentEffect();

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
            //startsGlaringPart = true;
        }
    }
}
