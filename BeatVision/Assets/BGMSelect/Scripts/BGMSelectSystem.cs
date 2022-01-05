using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BGMSelectSystem : MonoBehaviour
{
    [SerializeField] private TitleInputProvider titleInput;
    [SerializeField] private SEPlayer sEPlayer;
    [SerializeField] private BGMController bgm;

    [Header("BGMセレクト")]
    [SerializeField] List<RectTransform> BGMRectTransforms;
    [SerializeField] private float selectDuration;
    [SerializeField] private GameObject nowSelectBGM;
    private BGMParamater bgmParamater;

    [Header("ディスク")]
    [SerializeField] private RectTransform discTrans;
    [SerializeField] private float rotateAngle;

    [Header("フェード")]
    [SerializeField] private Image fadeImage;

    SceneLoader sceneLoader;

    int bgmCount;
    int nowSelectbgmNum;

    int[] fadebgmTemp = new int[5];

    float nextMoveYTemp;

    bool isAction;

    private bool isStart;

    private bool isFirstFade;

    private void Awake()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bgmCount = BGMRectTransforms.Count;
        nowSelectBGM = BGMRectTransforms[0].gameObject;
        bgmParamater = nowSelectBGM.GetComponent<BGMParamater>();

        bgm.Play(bgmParamater.bgm.ToString());

        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0f, selectDuration).SetEase(Ease.InCubic);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (fadeImage.color.a >= 1)
            {
                sceneLoader.Load(bgmParamater.sceneName);
            }

            return;
        }

        if (!isAction)
        {
            if (titleInput.upButtonDown)
            {
                MoveUpMenu();
                sEPlayer.SEPlay("Move");
                discTrans.DORotate(discTrans.rotation.eulerAngles - Vector3.forward * rotateAngle, selectDuration, RotateMode.Fast);
                if (bgmParamater.bgm != BGM.Exit)
                {
                    bgm.Play(bgmParamater.bgm.ToString());
                }
                else
                {
                    bgm.Stop();
                }
                StartCoroutine(StartAction());
            }
            else if (titleInput.downButtonDown)
            {
                MoveDownMenu();
                sEPlayer.SEPlay("Move");
                discTrans.DORotate(discTrans.rotation.eulerAngles + Vector3.forward * rotateAngle, selectDuration, RotateMode.Fast);
                if (bgmParamater.bgm != BGM.Exit)
                {
                    bgm.Play(bgmParamater.bgm.ToString());
                }
                else
                {
                    bgm.Stop();
                }
                StartCoroutine(StartAction());
            }
            else if (titleInput.selectButtonDown && bgmParamater.bgm == BGM.Exit)
            {
                isStart = true;
                sEPlayer.SEPlay("Select");
                fadeImage.DOFade(1.0f, selectDuration).SetEase(Ease.OutCubic);
                StartCoroutine(StartAction());
            }
        }
    }

    private void MoveUpMenu()
    {
        // フェードさせるメニューの値の格納
        for (int n = 0; n < fadebgmTemp.Length; n++)
        {
            fadebgmTemp[n] = (nowSelectbgmNum - 2) + n;

            if (fadebgmTemp[n] < 0)
            {
                fadebgmTemp[n] += bgmCount;
            }
            else if (fadebgmTemp[n] > (bgmCount - 1))
            {
                fadebgmTemp[n] -= bgmCount;
            }
        }

        //メニューの移動実行
        nextMoveYTemp = BGMRectTransforms[0].position.y;

        for (int i = bgmCount - 1; i >= 0; i--)
        {
            BGMRectTransforms[i].DOMoveY(nextMoveYTemp, selectDuration);

            nextMoveYTemp = BGMRectTransforms[i].position.y;

            for (int n = 0; n < fadebgmTemp.Length; n++)
            {
                if (i == fadebgmTemp[n])
                {
                    switch (n)
                    {
                        case 0:
                            BGMRectTransforms[i].DOScale(new Vector3(0.8f, 0.8f, 1), selectDuration);
                            break;
                        case 1:
                            BGMRectTransforms[i].DOScale(Vector3.one, selectDuration);
                            break;
                        case 2:
                            BGMRectTransforms[i].DOScale(new Vector3(0.8f, 0.8f, 1), selectDuration);
                            break;
                        case 3:
                            BGMRectTransforms[i].DOScale(new Vector3(0.6f, 0.6f, 1), selectDuration);
                            break;
                        case 4:
                            BGMRectTransforms[i].SetAsFirstSibling();
                            break;
                    }

                    break;
                }
            }

        }

        //選択するメニューの変更
        if (nowSelectbgmNum == 0)
        {
            nowSelectbgmNum = bgmCount - 1;
        }
        else
        {
            nowSelectbgmNum--;
        }

        nowSelectBGM = BGMRectTransforms[nowSelectbgmNum].gameObject;
        bgmParamater = nowSelectBGM.GetComponent<BGMParamater>();
    }

    private void MoveDownMenu()
    {
        // フェードさせるメニューの値の格納
        for (int n = 0; n < fadebgmTemp.Length; n++)
        {
            fadebgmTemp[n] = (nowSelectbgmNum + 2) - n;

            if (fadebgmTemp[n] < 0)
            {
                fadebgmTemp[n] += bgmCount;
            }
            else if (fadebgmTemp[n] > (bgmCount - 1))
            {
                fadebgmTemp[n] -= bgmCount;
            }
        }

        //メニューの移動実行
        nextMoveYTemp = BGMRectTransforms[bgmCount - 1].position.y;

        for (int i = 0; i < bgmCount; i++)
        {
            BGMRectTransforms[i].DOMoveY(nextMoveYTemp, selectDuration);

            nextMoveYTemp = BGMRectTransforms[i].position.y;

            for (int n = 0; n < fadebgmTemp.Length; n++)
            {
                if (i == fadebgmTemp[n])
                {
                    switch (n)
                    {
                        case 0:
                            BGMRectTransforms[i].DOScale(new Vector3(0.8f, 0.8f, 1), selectDuration);
                            break;
                        case 1:
                            BGMRectTransforms[i].DOScale(Vector3.one, selectDuration);
                            break;
                        case 2:
                            BGMRectTransforms[i].DOScale(new Vector3(0.8f, 0.8f, 1), selectDuration);
                            break;
                        case 3:
                            BGMRectTransforms[i].DOScale(new Vector3(0.6f, 0.6f, 1), selectDuration);
                            break;
                        case 4:
                            BGMRectTransforms[i].SetAsFirstSibling();
                            break;
                    }

                    break;
                }
            }

        }

        //選択するメニューの変更
        if (nowSelectbgmNum == bgmCount - 1)
        {
            nowSelectbgmNum = 0;
        }
        else
        {
            nowSelectbgmNum++;
        }

        nowSelectBGM = BGMRectTransforms[nowSelectbgmNum].gameObject;
        bgmParamater = nowSelectBGM.GetComponent<BGMParamater>();
    }

    private IEnumerator StartAction()
    {
        isAction = true;

        yield return new WaitForSeconds(selectDuration);

        isAction = false;
    }
}
