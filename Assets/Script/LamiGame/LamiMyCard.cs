using System.Collections;
using System.Collections.Generic;
using UITween;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LamiMyCard : LamiGameCard
{
    public bool isSelected;
    private bool isBeforeSelected;
    
    public int MyCardId;
    public int virtual_num;
    
    void Start()
    {
        Init();
    }
    public void Init()
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
        Color col = mCard.color;
        col.a = 1f;
        if (card.MyCardId == 1)  //default:-1 , 1:disable
        {
            col.a = 0.5f;    
        }
        mCard.color = col;
        GetComponent<Image>().color = col;
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
            RectTransform rt = (RectTransform)gameObject.transform;
            int move = (int)(rt.rect.width*0.4 * (isSelected ? -1 : 1));
            LeanTween.moveY(gameObject, transform.position.y - move, 0.1f);
        }
        isBeforeSelected=isSelected;
//        LogMgr.Inst.Log("Card Clicked: num:=" + num + ", color:=" + color + ", seat:=" + MyCardId + ", selected:=" + isSelected, (int)LogLevels.RoomLog2);
    }
}