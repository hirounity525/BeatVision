using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TitleInputProvider titleInput;
    [SerializeField] private SEPlayer sEPlayer;
    [SerializeField] private AudioSource BGM;

    [Header("メニューセレクト")]
    [SerializeField] List<RectTransform> menuRectTransforms;
    [SerializeField] private float selectDuration;
    [SerializeField] private GameObject nowSelectMenu;

    [Header("NewGame")]
    [SerializeField] private string startSceneName;
    [Header("Option")]
    [SerializeField] private GameObject optionWindow;
    [Header("Exit")]
    [SerializeField] private GameObject exitWindow;

    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject protoWindow;

    [SerializeField] private string musicGarallyName;

    SceneLoader sceneLoader;
    string sceneName;

    int menuCount;
    int nowSelectMenuNum;

    int[] fadeMenuTemp = new int[4];

    float nextMoveYTemp;

    bool isAction;
    bool isOpenWindow;
    GameObject openWindowObj;

    bool isStart;

    private void Awake()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        menuCount = menuRectTransforms.Count;
        nowSelectMenu = menuRectTransforms[0].gameObject;

        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0f, selectDuration).SetEase(Ease.InCubic);
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpenWindow)
        {
            if(openWindowObj != null)
            {
                if (titleInput.backButtonDown)
                {
                    sEPlayer.SEPlay("Select");
                    openWindowObj.GetComponent<SimpleAnimation>().Play("Close");
                }

                if (openWindowObj.activeSelf)
                {
                    return;
                }
                else
                {
                    isOpenWindow = false;
                    StartCoroutine(StartAction());
                }
            }
        }

        if (isStart)
        {
            if(fadeImage.color.a >= 1)
            {
                sceneLoader.Load(sceneName);
            }

            return;
        }

        if (!isAction)
        {
            if (titleInput.upButtonDown)
            {
                MoveUpMenu();
                sEPlayer.SEPlay("Move");
                StartCoroutine(StartAction());
            }
            else if (titleInput.downButtonDown)
            {
                MoveDownMenu();
                sEPlayer.SEPlay("Move");
                StartCoroutine(StartAction());
            }
            else if (titleInput.selectButtonDown)
            {
                SelectMenu();
                StartCoroutine(StartAction());
            }
        }
    }

    private void MoveUpMenu()
    {
        // フェードさせるメニューの値の格納
        for (int n = 0; n < fadeMenuTemp.Length; n++)
        {
            fadeMenuTemp[n] = (nowSelectMenuNum - 2) + n;

            if (fadeMenuTemp[n] < 0)
            {
                fadeMenuTemp[n] += menuCount;
            }
            else if (fadeMenuTemp[n] > (menuCount - 1))
            {
                fadeMenuTemp[n] -= menuCount;
            }
        }

        //メニューの移動実行
        nextMoveYTemp = menuRectTransforms[0].position.y;

        for (int i = menuCount - 1; i >= 0; i--)
        {
            menuRectTransforms[i].DOMoveY(nextMoveYTemp, selectDuration);

            nextMoveYTemp = menuRectTransforms[i].position.y;

            //フェード処理
            TextMeshProUGUI menuText = menuRectTransforms[i].GetComponent<TextMeshProUGUI>();

            for (int n = 0; n < fadeMenuTemp.Length; n++)
            {
                if (i == fadeMenuTemp[n])
                {
                    switch (n)
                    {
                        case 0:
                            menuText.DOFade(0.5f, selectDuration);
                            break;
                        case 1:
                            menuText.DOFade(1.0f, selectDuration);
                            break;
                        case 2:
                            menuText.DOFade(0.5f, selectDuration);
                            break;
                        case 3:
                            menuText.DOFade(0f, selectDuration);
                            break;
                    }

                    break;
                }
            }

        }

        //選択するメニューの変更
        if (nowSelectMenuNum == 0)
        {
            nowSelectMenuNum = menuCount - 1;
        }
        else
        {
            nowSelectMenuNum--;
        }

        nowSelectMenu = menuRectTransforms[nowSelectMenuNum].gameObject;
    }

    private void MoveDownMenu()
    {
        // フェードさせるメニューの値の格納
        for (int n = 0; n < fadeMenuTemp.Length; n++)
        {
            fadeMenuTemp[n] = (nowSelectMenuNum + 2) - n;

            if (fadeMenuTemp[n] < 0)
            {
                fadeMenuTemp[n] += menuCount;
            }
            else if (fadeMenuTemp[n] > (menuCount - 1))
            {
                fadeMenuTemp[n] -= menuCount;
            }
        }

        //メニューの移動実行
        nextMoveYTemp = menuRectTransforms[menuCount - 1].position.y;

        for (int i = 0; i < menuCount; i++)
        {
            menuRectTransforms[i].DOMoveY(nextMoveYTemp, selectDuration);

            nextMoveYTemp = menuRectTransforms[i].position.y;

            //フェード処理
            TextMeshProUGUI menuText = menuRectTransforms[i].GetComponent<TextMeshProUGUI>();

            for (int n = 0; n < fadeMenuTemp.Length; n++)
            {
                if (i == fadeMenuTemp[n])
                {
                    switch (n)
                    {
                        case 0:
                            menuText.DOFade(0.5f, selectDuration);
                            break;
                        case 1:
                            menuText.DOFade(1.0f, selectDuration);
                            break;
                        case 2:
                            menuText.DOFade(0.5f, selectDuration);
                            break;
                        case 3:
                            menuText.DOFade(0f, selectDuration);
                            break;
                    }

                    break;
                }
            }

        }

        //選択するメニューの変更
        if (nowSelectMenuNum == menuCount - 1)
        {
            nowSelectMenuNum = 0;
        }
        else
        {
            nowSelectMenuNum++;
        }

        nowSelectMenu = menuRectTransforms[nowSelectMenuNum].gameObject;
    }

    private void SelectMenu()
    {
        MenuParameter menuParameter = nowSelectMenu.GetComponent<MenuParameter>();

        Menu menu = menuParameter.menu;
        bool canSelect = menuParameter.canSelect;

        if (canSelect)
        {
            sEPlayer.SEPlay("Select");
            switch (menu)
            {
                case Menu.NEWGAME:
                    fadeImage.DOFade(1.0f, selectDuration).SetEase(Ease.OutCubic);
                    BGM.DOFade(0, selectDuration);
                    sceneName = startSceneName;
                    isStart = true;
                    break;
                case Menu.CONTINUE:
                    break;
                case Menu.STAGESELECT:
                    break;
                case Menu.SCORE:
                    fadeImage.DOFade(1.0f, selectDuration).SetEase(Ease.OutCubic);
                    BGM.DOFade(0, selectDuration);
                    sceneName = musicGarallyName;
                    isStart = true;
                    break;
                case Menu.OPTION:
                    protoWindow.SetActive(true);
                    openWindowObj = protoWindow;
                    isOpenWindow = true;
                    break;
                case Menu.EXIT:
                    Application.Quit();
                    break;
                case Menu.MANUAL:
                    protoWindow.SetActive(true);
                    openWindowObj = protoWindow;
                    isOpenWindow = true;
                    break;
            }
        }
        else
        {
            //sEPlayer.SEPlay("Error");
        }
    }

    private IEnumerator StartAction()
    {
        isAction = true;

        yield return new WaitForSeconds(selectDuration);

        isAction = false;
    }
}
