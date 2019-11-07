using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortuneCard : MonoBehaviour
{
    public Image mCover;
    public bool isSelected;
    public int color;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;        
    }

    public void UpdateValue()
    {
        mCover.sprite = Resources.Load<Sprite>("Card/" + "card_" + color + "_" + num);
    }

    internal void Init(bool v)
    {
        isSelected = false;
        this.gameObject.SetActive(v);
        mCover.sprite = Resources.Load<Sprite>("Card/_black");
    }

    internal void moveDealCard(Vector3 srcPos)
    {
        
        iTween.MoveFrom(this.gameObject, srcPos, 0.5f);
        this.gameObject.SetActive(true);
    }

    public void Move()
    {
        

    }

}
