﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LamiGameCard : MonoBehaviour
{
    public Image mCard;
    public int color;
    public int num = 0;
    public GameObject lastCardEffect;
    bool isLast;
    
    
    public const int JOKER_NUM = 15;
    // Start is called before the first frame update

    void Start()
    {
        UpdateValue();        
    }

    public void UpdateValue()
    {
        if(num == JOKER_NUM)
            mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_15_15");
        else
            mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
        if (lastCardEffect)
            lastCardEffect.SetActive(isLast);
    }

    public void UpdateCard(Card card)
    {
        num = card.num;
        color = card.color;
        isLast = (card.MyCardId==1);
        UpdateValue();
    }
}
