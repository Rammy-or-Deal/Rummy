using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LamiGameCard : MonoBehaviour
{
    public Image mCard;
    public int color;
    public int num;
    
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
    }

    public void UpdateCard(Card card)
    {
        num = card.num;
        color = card.color;
        UpdateValue();
    }
}
