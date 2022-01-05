using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerAttackNotesDrawer : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private RectTransform iconTrans;

    [SerializeField] private GameObject note;

    [SerializeField] private Sprite upperNoteSprite;
    [SerializeField] private Sprite strikeNoteSprite;
    [SerializeField] private Sprite lowerNoteSprite;
    [SerializeField] private Sprite qNoteSprite;
    [SerializeField] private Sprite missNoteSprite;

    [SerializeField] private float iconMoveStartValue;
    [SerializeField] private float iconMoveEndValue;

    private Tweener iconTweener;
    private List<GameObject> actionNoteList = new List<GameObject>();
    private int nowActionNum;

    public void Display()
    {
        bar.SetActive(true);
        iconTrans.localPosition = new Vector3(iconMoveStartValue, iconTrans.localPosition.y, iconTrans.localPosition.z);
    }

    public void MoveIcon(float moveTime)
    {
        if(iconTweener != null)
        {
            iconTweener.Kill();
        }
        iconTrans.localPosition = new Vector3(iconMoveStartValue, iconTrans.localPosition.y, iconTrans.localPosition.z);
        iconTweener = iconTrans.DOLocalMoveX(iconMoveEndValue, moveTime).SetEase(Ease.Linear);
    }

    public void Prot(Action protAction)
    {
        GameObject actionNote = Instantiate(note, iconTrans.position, Quaternion.identity, bar.transform);
        Image noteImage = actionNote.GetComponent<Image>();

        switch (protAction)
        {
            case Action.UPPER:
                noteImage.sprite = upperNoteSprite;
                break;
            case Action.STRIKE:
                noteImage.sprite = strikeNoteSprite;
                break;
            case Action.LOWER:
                noteImage.sprite = lowerNoteSprite;
                break;
        }

        actionNoteList.Add(actionNote);
    }

    public void ReplaceQ()
    {
        for (nowActionNum = 0; nowActionNum < actionNoteList.Count; nowActionNum++)
        {
            actionNoteList[nowActionNum].GetComponent<Image>().sprite = qNoteSprite;
        }

        nowActionNum = 0;
    }

    public void Replace(bool isSuccess, Action replaceAction)
    {
        if (isSuccess)
        {
            switch (replaceAction)
            {
                case Action.UPPER:
                    actionNoteList[nowActionNum].GetComponent<Image>().sprite = upperNoteSprite;
                    break;
                case Action.STRIKE:
                    actionNoteList[nowActionNum].GetComponent<Image>().sprite = strikeNoteSprite;
                    break;
                case Action.LOWER:
                    actionNoteList[nowActionNum].GetComponent<Image>().sprite = lowerNoteSprite;
                    break;
            }
        }
        else
        {
            actionNoteList[nowActionNum].GetComponent<Image>().sprite = missNoteSprite;
        }

        nowActionNum++;
    }

    public void ResetParameter()
    {
        for (nowActionNum = 0; nowActionNum < actionNoteList.Count; nowActionNum++)
        {
            Destroy(actionNoteList[nowActionNum]);
        }
        actionNoteList.Clear();
        //iconTweener = iconTrans.DOLocalMoveX(iconMoveStartValue, 0.01f).SetEase(Ease.Linear);
        bar.SetActive(false);
    }
}
