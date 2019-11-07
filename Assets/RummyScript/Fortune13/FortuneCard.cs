using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortuneCard : MonoBehaviour
{
    public Image mCard;
    public Image mCover;
    public bool isSelected;
    public int color;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        mCover.sprite = Resources.Load<Sprite>("new_card/card_cover");
        mCard.sprite = Resources.Load<Sprite>("new_card/cover_pan");
    }

    public void UpdateValue()
    {
        mCover.sprite = Resources.Load<Sprite>("new_card/card_pan");
        mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
    }

    internal void Init(bool v)
    {
        isSelected = false;
        this.gameObject.SetActive(v);
        mCover.sprite = Resources.Load<Sprite>("new_card/card_cover");
        mCard.sprite = Resources.Load<Sprite>("new_card/cover_pan");
    }

    internal void moveDealCard(Vector3 srcPos)
    {
        this.gameObject.SetActive(true);
        iTween.MoveFrom(this.gameObject, srcPos, 0.5f);
    }

    public void Move()
    {
        

    }

}
