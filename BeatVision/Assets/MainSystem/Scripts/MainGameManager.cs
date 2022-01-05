using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GameMode
{
    START,
    MAIN,
    FINISH
}

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

public enum EnemyAttackType
{
    SINGLE,
    TUPLET
}

[System.Serializable]
public struct EnemyAttackInfo
{
    public float glaringTime;
    public EnemyAttackType attackType;
    public int attackNum;
    public List<Action> attackActions;
}

[System.Serializable]
public struct ActionTiming
{
    public Action action;
    public float timing;
    public bool isCheck;
}

public class MainGameManager : MonoBehaviour
{
    InputProvider inputProvider;
    ModeChangeJudger changeJudger;
    SceneLoader sceneLoader;

    [SerializeField] TimelineController openingTimeline;
    [SerializeField] TimelineController winTimeline;
    [SerializeField] TimelineController loseTimeline;
    private bool playsOpening;
    private bool playsEnding;
    private bool playerWin;
    private TimelineController playTimeline;

    //プレイヤー・エネミー情報
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject playerCloneObj;
    [SerializeField] private GameObject enemyObj;

    Transform playerTrans;
    AnimationPlayer playerAnim;
    PlayerHPController playerHP;
    InputAnimPlayer inputAnim;
    SEPlayer playerSE;
    BarrierController playerBarrier;

    Transform cloneTrans;
    AnimationPlayer cloneAnim;
    AuraEffectPlayer cloneAura;
    SEPlayer cloneSE;

    Transform enemyTrans;
    AnimationPlayer enemyAnim;
    AuraEffectPlayer enemyAura;
    PlayerHPController enemyHP;
    SEPlayer enemySE;

    [SerializeField] EnemyAttackInfo attackInfo;

    //ターン制御
    [SerializeField] GameMode mode;
    [SerializeField] Part part;
    [SerializeField] Turn turn;

    private bool finishesInsert;
    private bool finishesGlaring;
    private bool finishesAttack;

    [Header("判定")]
    [SerializeField] private float goodInterval;
    [SerializeField] private float excelentInterval;
    [SerializeField] private EffectPlayer effectPlayer;
    [SerializeField] private SEPlayer judgeSE;

    [Header("にらみ合い")]
    [SerializeField] private float minGlaringTime;
    [SerializeField] private float maxGlaringTime;
    [SerializeField] private float faceDistance;
    [SerializeField] private float faceTime;
    [SerializeField] private float maxNearDistance;
    [SerializeField] private float maxLeaveDisntace;
    [SerializeField] private float distanceOffset;
    [SerializeField] private float minMoveDuration;
    [SerializeField] private float maxMoveDuration;

    private Tweener playerTweener;
    private Tweener enemyTweener;
    private float playerMoveDistance;
    private float playerMoveDuration;
    private float enemyMoveDistance;
    private float enemyMoveDuration;
    private bool isSetPlayerMoveDistance;
    private bool isPlayerMoveRight = true;
    private bool isPlayerMove;
    private bool isSetEnemyMoveDistance;
    private bool isEnemyMoveRight = true;
    private bool isEnemyMove;
    private bool isEnemyFaceMove;
    private float glaringTimer;

    [Header("敵の攻撃")]
    [Header("Single")]
    [SerializeField] private int maxSingleAttackNum;
    [SerializeField] private float minNextAttackInterval;
    [SerializeField] private float maxNextAttackInterval;
    [SerializeField] private float singleAttackInterval;
    [SerializeField] private float singlePlayAnimBeforeTime;

    [Header("Tuplet")]
    [SerializeField] private int maxTupletAttackNum;
    [SerializeField] private float tupletAttackInterval;
    [SerializeField] private float tupletPreAttackInterval;
    [SerializeField] private float tupletPlayAnimBeforeTime;

    private int nowActionNum;
    private Action nowAction;
    private float attackTimer;
    private float saveTime;

    private float nextAttackTime;
    private float nextAttackTimer;

    private bool setsNextAttack;
    private bool playsPreAttack;
    private bool playsAttack;
    private bool checkedAttack;

    [SerializeField]private List<ActionTiming> actionTimings = new List<ActionTiming>();
    private bool isTupletPreAttack = true;
    bool isResetMovement;
    int nowCheckNum;
    bool loadsActionTiming;
    ActionTiming checkActionTiming;
    bool isTupletInfoReset;

    [Header("下がる")]
    [SerializeField]private float backDistance;
    [SerializeField]private float backDuration;
    

    [Header("自分の攻撃")]
    [SerializeField] BeatDataLoader playerDataLoader;
    [SerializeField] private float nearAttackDisOffset;
    [Header("アクションのインターバル（譜刻み）")]
    [SerializeField] private float actionInterval;
    [SerializeField] private int maxActionNum;
    [SerializeField] private float attackSEBeforeTime;
    [SerializeField] private GameObject postProcessObj;
    [SerializeField] private PlayerAttackNotesDrawer notesDrawer;

    float actionTimer;

    BeatData beatData;
    int actionDataNum;
    bool loadsBeatData;

    float saveActionDataTime;

    ActionData actionData;
    int nowActionDataNum;
    int actionNum;
    bool loadsActionData;

    Action action;
    bool loadsAction;

    float startTimer;

    private bool finishesPreAttackPlayer;
    private bool finishesAttackPlayer;

    private bool isBackPlayer;
    private bool isPreAttackPlayer;

    private bool startsCloneMove;
    private bool startsPreAttack;

    private bool startsPlayerMove;
    private bool startsPlayerAttack;

    private void Awake()
    {
        inputProvider = GetComponent<InputProvider>();
        changeJudger = GetComponent<ModeChangeJudger>();
        sceneLoader = GetComponent<SceneLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = playerObj.GetComponent<Transform>();
        playerAnim = playerObj.GetComponent<AnimationPlayer>();
        playerHP = playerObj.GetComponent<PlayerHPController>();
        inputAnim = playerObj.GetComponent<InputAnimPlayer>();
        playerSE = playerObj.GetComponent<SEPlayer>();
        playerBarrier = playerObj.GetComponent<BarrierController>();

        cloneTrans = playerCloneObj.GetComponent<Transform>();
        cloneAnim = playerCloneObj.GetComponent<AnimationPlayer>();
        cloneAura = playerCloneObj.GetComponent<AuraEffectPlayer>();
        cloneSE = playerCloneObj.GetComponent<SEPlayer>();

        enemyTrans = enemyObj.GetComponent<Transform>();
        enemyAnim = enemyObj.GetComponent<AnimationPlayer>();
        enemyAura = enemyObj.GetComponent<AuraEffectPlayer>();
        enemyHP = enemyObj.GetComponent<PlayerHPController>();
        enemySE = enemyObj.GetComponent<SEPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case GameMode.START:
                if (!playsOpening)
                {
                    openingTimeline.PlayTimeline();
                    enemyAnim.PlayStartAnim();
                    enemySE.SEPlay("Start");
                    playsOpening = true;
                }

                if (openingTimeline.isFinish)
                {
                    playsOpening = false;
                    mode = GameMode.MAIN;
                }

                break;
            case GameMode.MAIN:

                if(playerHP.hitPoints <= 0)
                {
                    playerWin = false;
                    mode = GameMode.FINISH;
                }
                else if(enemyHP.hitPoints <= 0)
                {
                    playerWin = true;
                    mode = GameMode.FINISH;
                }

                switch (turn)
                {
                    case Turn.DEFENSE:
                        switch (part)
                        {
                            case Part.GLARING:

                                if (!finishesInsert)
                                {
                                    InsertAttackInfo();
                                    finishesInsert = true;
                                }

                                Glaring(attackInfo.glaringTime);

                                if (finishesGlaring)
                                {
                                    part = Part.MOVEMENT;

                                    finishesInsert = false;
                                    finishesGlaring = false;
                                }
                                break;
                            case Part.MOVEMENT:

                                if (changeJudger.changesTurn)
                                {
                                    turn = Turn.ATTACK;
                                    part = Part.PRE;
                                    ResetStatus();
                                    changeJudger.changesTurn = false;
                                    return;
                                }

                                switch (attackInfo.attackType)
                                {
                                    case EnemyAttackType.SINGLE:
                                        SingleAttackEnemy();
                                        break;
                                    case EnemyAttackType.TUPLET:
                                        TupletAttackEnemy();
                                        break;
                                }

                                if (finishesAttack)
                                {
                                    part = Part.GLARING;
                                    finishesAttack = false;
                                }

                                break;
                        }
                        break;
                    case Turn.ATTACK:
                        switch (part)
                        {
                            case Part.PRE:
                                if (!isBackPlayer)
                                {
                                    BackPlayer();
                                    isBackPlayer = true;
                                }

                                if (isPreAttackPlayer)
                                {
                                    PreAction();
                                }

                                if (finishesPreAttackPlayer)
                                {
                                    part = Part.MOVEMENT;
                                    isPreAttackPlayer = false;
                                    isBackPlayer = false;
                                    finishesPreAttackPlayer = false;
                                }

                                break;
                            case Part.MOVEMENT:

                                Movement();

                                if (finishesAttackPlayer)
                                {
                                    ResetStatus();
                                    turn = Turn.DEFENSE;
                                    part = Part.GLARING;
                                    finishesAttackPlayer = false;
                                }

                                break;
                        }
                        break;
                }
                break;
            case GameMode.FINISH:

                if (!playsEnding)
                {
                    inputAnim.canInput = false;

                    if (playerWin)
                    {
                        winTimeline.PlayTimeline();
                        playTimeline = winTimeline;
                    }
                    else
                    {
                        loseTimeline.PlayTimeline();
                        playTimeline = loseTimeline;
                    }

                    playsEnding = true;
                }

                if (playTimeline.isFinish)
                {
                    sceneLoader.Load("ProtoTitle");
                }

                break;
        }
    }


    private void InsertAttackInfo()
    {

        attackInfo.glaringTime = Random.Range(minGlaringTime, maxGlaringTime);

        int attackTypeNum = Random.Range(0, 5);

        if(attackTypeNum <= 2)
        {
            attackInfo.attackType = EnemyAttackType.SINGLE;
        }
        else
        {
            attackInfo.attackType = EnemyAttackType.TUPLET;
        }

        attackInfo.attackActions.Clear();

        if (attackInfo.attackType == EnemyAttackType.SINGLE)
        {
            attackInfo.attackNum = Random.Range(3, maxSingleAttackNum + 1);

            for(int i = 0; i < attackInfo.attackNum; i++)
            {
                int randamAttack = Random.Range(0, 3);

                if (randamAttack == 0)
                {
                    attackInfo.attackActions.Add(Action.UPPER);
                }
                else if (randamAttack == 1)
                {
                    attackInfo.attackActions.Add(Action.STRIKE);
                }
                else
                {
                    attackInfo.attackActions.Add(Action.LOWER);
                }
            }
        }
        else
        {
            attackInfo.attackNum = maxTupletAttackNum;

            for (int i = 0; i < attackInfo.attackNum; i++)
            {
                int randamAttack = Random.Range(0, 3);

                if (randamAttack == 0)
                {
                    attackInfo.attackActions.Add(Action.UPPER);
                }
                else if (randamAttack == 1)
                {
                    attackInfo.attackActions.Add(Action.STRIKE);
                }
                else
                {
                    attackInfo.attackActions.Add(Action.LOWER);
                }
            }
        }

    }

    private void Glaring(float time)
    {
        if (glaringTimer >= time)
        {
            isEnemyFaceMove = false;
            glaringTimer = 0;
            finishesGlaring = true;
        }
        else
        {
            glaringTimer += Time.deltaTime;
        }

        if(glaringTimer >= time - faceTime)
        {
            isSetPlayerMoveDistance = false;
            isPlayerMoveRight = true;
            isPlayerMove = false;

            isSetEnemyMoveDistance = false;
            isEnemyMoveRight = true;
            isEnemyMove = false;

            if (!isEnemyFaceMove)
            {
                playerTweener.Kill();
                enemyTweener.Kill();

                enemyAnim.PlayPreAnim(Action.MOVE);
                enemyTrans.DOMoveX(playerTrans.position.x + faceDistance, faceTime - 0.1f).OnComplete(() => enemyAnim.PlayActionAnim(Action.WAIT));
                isEnemyFaceMove = true;
            }
            return;
        }

        if (!isSetPlayerMoveDistance)
        {
            if (isPlayerMoveRight)
            {
                playerMoveDistance = Random.Range(-maxLeaveDisntace, -maxLeaveDisntace + distanceOffset);
                isPlayerMoveRight = false;
            }
            else
            {
                playerMoveDistance = Random.Range(-maxNearDistance - distanceOffset, -maxNearDistance);
                isPlayerMoveRight = true;
            }

            playerMoveDuration = Random.Range(minMoveDuration, maxMoveDuration);

            isSetPlayerMoveDistance = true;
        }
        else
        {
            if (!isPlayerMove)
            {
                playerTweener = playerTrans.DOMoveX(playerMoveDistance, playerMoveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    isPlayerMove = false;
                    isSetPlayerMoveDistance = false;
                }
                );

                isPlayerMove = true;
            }
        }

        if (!isSetEnemyMoveDistance)
        {
            if (isEnemyMoveRight)
            {
                enemyMoveDistance = Random.Range(maxLeaveDisntace - distanceOffset, maxLeaveDisntace);
                isPlayerMoveRight = false;
            }
            else
            {
                enemyMoveDistance = Random.Range(maxNearDistance, maxNearDistance + distanceOffset);
                isPlayerMoveRight = true;
            }

            enemyMoveDuration = Random.Range(minMoveDuration, maxMoveDuration);

            isSetEnemyMoveDistance = true;
        }
        else
        {
            if (!isEnemyMove)
            {
                enemyAnim.PlayActionAnim(Action.WAIT);

                enemyTweener = enemyTrans.DOMoveX(enemyMoveDistance, enemyMoveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    isEnemyMove = false;
                    isSetEnemyMoveDistance = false;
                }
                );

                isEnemyMove = true;
            }
        }
    }

    private void SingleAttackEnemy()
    {
        if(nowActionNum < attackInfo.attackActions.Count)
        {
            if (!setsNextAttack)
            {
                nowAction = attackInfo.attackActions[nowActionNum];

                nextAttackTimer = 0;
                attackTimer = 0;
                playsPreAttack = false;
                playsAttack = false;
                checkedAttack = false;

                nextAttackTime = Random.Range(minNextAttackInterval, maxNextAttackInterval);
                setsNextAttack = true;
            }

            if (nextAttackTimer >= nextAttackTime)
            {
                //呼び動作再生
                if (!playsPreAttack)
                {
                    enemySE.SEPlay(nowAction.ToString());
                    enemyAnim.PlayPreAnim(nowAction);
                    enemyAura.PlayEffect(nowAction);

                    playsPreAttack = true;
                }

                //攻撃モーション再生
                if (!playsAttack)
                {
                    if (attackTimer >= singleAttackInterval - singlePlayAnimBeforeTime)
                    {
                        enemyAnim.PlayActionAnim(nowAction);
                        playsAttack = true;
                    }
                }

                if (attackTimer <= singleAttackInterval + goodInterval)
                {
                    float diff = Mathf.Abs(singleAttackInterval - attackTimer);

                    if (inputProvider.buttonDownComand)
                    {
                        if (!checkedAttack)
                        {
                            if (diff <= excelentInterval)
                            {
                                checkedAttack = true;

                                if (inputProvider.inputAction == nowAction)
                                {
                                    effectPlayer.PlayExcellentEffect();
                                    judgeSE.SEPlay("Excellent");
                                    changeJudger.AddExcellentPoint();
                                }
                                else if (inputProvider.inputAction == Action.MIDDLE)
                                {
                                    if (!playerBarrier.isBarrierBroken)
                                    {
                                        judgeSE.SEPlay("Excellent");
                                    }
                                    else
                                    {
                                        playerHP.GoodDamage();
                                    }
                                }
                                else
                                {
                                    judgeSE.SEPlay("Damage");
                                    playerAnim.PlayDamageAnim();
                                    playerHP.GoodDamage();
                                }
                            }
                            else if (diff <= goodInterval)
                            {
                                checkedAttack = true;

                                if (inputProvider.inputAction == nowAction)
                                {
                                    judgeSE.SEPlay("Good");
                                    effectPlayer.PlayGoodEffect();
                                    changeJudger.AddGoodPoint();
                                }
                                else if(inputProvider.inputAction == Action.MIDDLE)
                                {
                                    if (!playerBarrier.isBarrierBroken)
                                    {
                                        judgeSE.SEPlay("Good");
                                    }
                                    else
                                    {
                                        playerHP.GoodDamage();
                                    }
                                }
                                else
                                {
                                    judgeSE.SEPlay("Damage");
                                    playerAnim.PlayDamageAnim();
                                    playerHP.GoodDamage();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!checkedAttack)
                    {
                        judgeSE.SEPlay("Damage");
                        playerAnim.PlayDamageAnim();
                        playerHP.GoodDamage();
                    }

                    nowActionNum++;
                    setsNextAttack = false;
                }

                attackTimer += Time.deltaTime;
            }
            else
            {
                nextAttackTimer += Time.deltaTime;
            }
        }
        else
        {
            nowActionNum = 0;
            playerAnim.PlayActionAnim(Action.WAIT);
            finishesAttack = true;
        }
    }

    private void TupletAttackEnemy()
    {
        if (!isTupletInfoReset)
        {
            actionTimings.Clear();
            isTupletInfoReset = true;
        }

        if (isTupletPreAttack)
        {
            if (nowActionNum < attackInfo.attackActions.Count)
            {
                if (!setsNextAttack)
                {
                    nowAction = attackInfo.attackActions[nowActionNum];

                    SaveActionData(nowAction, tupletAttackInterval * (nowActionNum + 1));

                    attackTimer = 0;

                    setsNextAttack = true;
                }

                if (attackTimer >= tupletPreAttackInterval)
                {
                    enemySE.SEPlay(nowAction.ToString());
                    enemyAnim.PlayPreAnim(nowAction);
                    enemyAura.PlayEffect(nowAction);

                    nowActionNum++;
                    setsNextAttack = false;
                }
                else
                {
                    attackTimer += Time.deltaTime;
                }
            }
            else
            {
                if(nextAttackTimer >= tupletPreAttackInterval)
                {
                    nextAttackTimer = 0;
                    nowActionNum = 0;
                    isTupletPreAttack = false;
                }
                else
                {
                    nextAttackTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            if (!isResetMovement)
            {
                attackTimer = -goodInterval;
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

                if (attackTimer >= checkActionTiming.timing - tupletPlayAnimBeforeTime)
                {
                    enemySE.SEPlay(checkActionTiming.action.ToString());
                    enemyAnim.PlayPreAnim(checkActionTiming.action);
                    enemyAnim.PlayActionAnim(checkActionTiming.action);
                }

                if (attackTimer <= checkActionTiming.timing + 0.1f)
                {
                    float diff = Mathf.Abs(checkActionTiming.timing - attackTimer);

                    if (inputProvider.buttonDownComand)
                    {
                        if (!checkActionTiming.isCheck)
                        {
                            if (diff <= excelentInterval)
                            {
                                checkActionTiming.isCheck = true;

                                if (inputProvider.inputAction == nowAction)
                                {
                                    effectPlayer.PlayExcellentEffect();
                                    judgeSE.SEPlay("Excellent");
                                    changeJudger.AddExcellentPoint();
                                }
                                else if (inputProvider.inputAction == Action.MIDDLE)
                                {
                                    if (!playerBarrier.isBarrierBroken)
                                    {
                                        judgeSE.SEPlay("Excellent");
                                    }
                                    else
                                    {
                                        playerHP.GoodDamage();
                                    }
                                }
                                else
                                {
                                    judgeSE.SEPlay("Damage");
                                    playerAnim.PlayDamageAnim();
                                    playerHP.GoodDamage();
                                }
                            }
                            else if (diff <= goodInterval)
                            {
                                checkActionTiming.isCheck = true;

                                if (inputProvider.inputAction == nowAction)
                                {
                                    effectPlayer.PlayGoodEffect();
                                    judgeSE.SEPlay("Good");
                                    changeJudger.AddGoodPoint();
                                }
                                else if (inputProvider.inputAction == Action.MIDDLE)
                                {
                                    if (!playerBarrier.isBarrierBroken)
                                    {
                                        judgeSE.SEPlay("Good");
                                    }
                                    else
                                    {
                                        playerHP.GoodDamage();
                                    }
                                }
                                else
                                {
                                    judgeSE.SEPlay("Damage");
                                    playerAnim.PlayDamageAnim();
                                    playerHP.GoodDamage();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!checkActionTiming.isCheck)
                    {
                        judgeSE.SEPlay("Damage");
                        playerAnim.PlayDamageAnim();
                        playerHP.GoodDamage();
                    }

                    nowCheckNum++;
                    loadsActionTiming = false;
                }

                attackTimer += Time.deltaTime;
            }
            else
            {
                nowCheckNum = 0;
                isResetMovement = false;
                isTupletPreAttack = true;

                isTupletInfoReset = false;
                playerAnim.PlayActionAnim(Action.WAIT);

                finishesAttack = true;
            }
        }
    }

    private void BackPlayer()
    {
        inputAnim.canInput = false;
        playerAnim.PlayBackAnim();
        playerTrans.DOMoveX(playerTrans.position.x - backDistance, backDuration)
            .OnComplete(()=> 
            {
                playerAnim.Pause();
                postProcessObj.SetActive(true);
                isPreAttackPlayer = true;
            });
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
            startsCloneMove = false;

            actionTimings.Clear();

            loadsBeatData = true;
        }

        if (!startsCloneMove)
        {
            cloneTrans.position = playerTrans.position;
            playerCloneObj.SetActive(true);
            cloneAnim.PlayPreAnim(Action.MOVE);
            cloneTrans.DOMoveX(cloneTrans.position.x + backDistance + nearAttackDisOffset, actionInterval)
                .OnComplete(()=> 
                {
                    notesDrawer.MoveIcon(actionInterval * maxActionNum * actionDataNum);
                    startsPreAttack = true;
                });

            notesDrawer.Display();

            startsCloneMove = true;
        }

        if (!startsPreAttack)
        {
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
                    if(actionTimer >= saveActionDataTime + (nowActionNum * actionInterval) - attackSEBeforeTime)
                    {
                        cloneSE.SEPlay(action.ToString());
                        cloneAnim.PlayPreAnim(action);
                    }

                    if (actionTimer >= saveActionDataTime + (nowActionNum * actionInterval))
                    {
                        cloneAnim.PlayActionAnim(action);
                        cloneAura.PlayEffect(action);

                        if (action != Action.WAIT)
                        {
                            SaveActionData(action, actionTimer);
                            notesDrawer.Prot(action);
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
                            postProcessObj.SetActive(false);
                            playerSE.SEPlay("Start");
                            playerCloneObj.SetActive(false);

                            loadsBeatData = false;
                            finishesPreAttackPlayer = true;
                        }
                        else
                        {
                            startTimer += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
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
                postProcessObj.SetActive(false);
                playerSE.SEPlay("Start");
                playerCloneObj.SetActive(false);

                loadsBeatData = false;
                finishesPreAttackPlayer = true;
            }
            else
            {
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
            actionTimer = -goodInterval;
            nowCheckNum = 0;
            startsPlayerMove = false;

            notesDrawer.ReplaceQ();

            isResetMovement = true;
        }

        if (!startsPlayerMove)
        {
            playerAnim.PlayPreAnim(Action.MOVE);
            playerTrans.DOMoveX(playerTrans.position.x + backDistance + nearAttackDisOffset, actionInterval)
                .OnComplete(() =>
                {
                    notesDrawer.MoveIcon(actionInterval * maxActionNum * actionDataNum);
                    startsPlayerAttack = true;
                });
            startsPlayerMove = true;
        }

        if (!startsPlayerAttack)
        {
            return;
        }

        if (nowCheckNum < actionTimings.Count)
        {
            //チェックするアクションタイミングを読み取る
            if (!loadsActionTiming)
            {
                checkActionTiming = actionTimings[nowCheckNum];

                loadsActionTiming = true;
            }

            if (actionTimer >= checkActionTiming.timing - attackSEBeforeTime)
            {
                playerSE.SEPlay(checkActionTiming.action.ToString());
                if (playerAnim.isError)
                {
                    playerAnim.PlayActionAnim(Action.WAIT);
                    playerAnim.isError = false;
                }

            }

            if (actionTimer <= checkActionTiming.timing + 0.1f)
            {
                float diff = Mathf.Abs(checkActionTiming.timing - actionTimer);

                if (!checkActionTiming.isCheck)
                {
                    if (inputProvider.buttonDownComand)
                    {
                        if (diff <= 0.05f)
                        {
                            checkActionTiming.isCheck = true;

                            if (inputProvider.inputAction == checkActionTiming.action)
                            {
                                playerAnim.PlayActionAnim(checkActionTiming.action);
                                enemyAnim.PlayDamageAnim();
                                enemyHP.ExcellentDamage();
                                effectPlayer.PlayExcellentEffect();
                                enemySE.SEPlay("Damage_S");

                                notesDrawer.Replace(true, checkActionTiming.action);
                            }
                            else
                            {
                                playerAnim.PlayErrorAnim();

                                notesDrawer.Replace(false, checkActionTiming.action);
                            }
                        }
                        else if (diff <= 0.1f)
                        {
                            checkActionTiming.isCheck = true;

                            if (inputProvider.inputAction == checkActionTiming.action)
                            {
                                playerAnim.PlayActionAnim(checkActionTiming.action);
                                enemyHP.GoodDamage();
                                enemyAnim.PlayDamageAnim();
                                effectPlayer.PlayGoodEffect();
                                enemySE.SEPlay("Damage_W");

                                notesDrawer.Replace(true, checkActionTiming.action);
                            }
                            else
                            {
                                playerAnim.PlayErrorAnim();

                                notesDrawer.Replace(false, checkActionTiming.action);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!checkActionTiming.isCheck)
                {
                    playerAnim.PlayErrorAnim();

                    notesDrawer.Replace(false, checkActionTiming.action);
                }

                nowCheckNum++;
                loadsActionTiming = false;
            }

            //タイマープラス
            actionTimer += Time.deltaTime;
        }
        else
        {
            isResetMovement = false;
            playerAnim.PlayActionAnim(Action.WAIT);
            notesDrawer.ResetParameter();
            finishesAttackPlayer = true;
        }
    }

    private void ResetStatus()
    {
        nowActionNum = 0;
        finishesAttack = false;
        nowCheckNum = 0;
        isResetMovement = false;
        isTupletPreAttack = true;
        isTupletInfoReset = false;
        loadsActionTiming = false;
        setsNextAttack = false;


        inputAnim.canInput = true;
    }
}
