using System.Collections;
using System.Collections.Generic;
using UITween;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LamiMyCard : MonoBehaviour
{
    public Image mCard;
    public bool isSelected;
    private bool isBeforeSelected;
    public int color;
    public int num;
    public int MyCardId;
    public int virtual_num;
    public const int JOKER_NUM = 15;
    void Start()
    {
        isSelected = false;
        isBeforeSelected = false;
        UpdateValue();
    }

    public void UpdateFinishCard(Card card)// when Finish
    {
        num = card.num;
        color = card.color;
        UpdateValue();
        if (card.MyCardId == 0)  //default:-1 , 0:disable
        {
            Color col = mCard.color;
            col.a = 0.5f;
            mCard.color = col;
            GetComponent<Image>().color = col;
        }
    }

    public void UpdateValue()
    {
        if(num == JOKER_NUM)
            mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_15_15");
        else
            mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
    }

    public void OnClick()
    {
        isSelected = (!isSelected);
        SetUpdate();
        LamiGameUIManager.Inst.myCardPanel.SetPlayButtonState();
    }
    public void SetUpdate()
    {
            
        if (isSelected != isBeforeSelected)
        {
            int move = 10 * (isSelected ? -1 : 1);
            LeanTween.moveY(gameObject, transform.position.y - move, 0.1f);
        }
        isBeforeSelected=isSelected;
//        LogMgr.Inst.Log("Card Clicked: num:=" + num + ", color:=" + color + ", seat:=" + MyCardId + ", selected:=" + isSelected, (int)LogLevels.RoomLog2);
    }
}