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
    public int color;
    public int num;
    public int MyCardId;
    [HideInInspector] public bool disable; // disable effect status When game finish window
    float initPos;
    void Start()
    {
        isSelected = false;
        UpdateValue();
        initPos = transform.position.y;        
    }

    public void UpdateValue()
    {
        mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
        if (disable)
        {
            Color col = mCard.color;
            col.a = 0.5f;
            mCard.color = col;
            GetComponent<Image>().color = col;
        }
    }

    public void OnClick()
    {
        isSelected = (!isSelected);
        int move = 10 * (isSelected ? -1 : 0);
        LeanTween.moveY(gameObject, initPos - move, 0.1f);
        LamiGameUIManager.Inst.myCardPanel.SetPlayButtonState(this);
    }
    public void SetUpdate(bool sel)
    {
        isSelected = sel;
        int move = 10 * (isSelected ? -1 : 0);
        LeanTween.moveY(gameObject, initPos - move, 0.1f);
    }
}
