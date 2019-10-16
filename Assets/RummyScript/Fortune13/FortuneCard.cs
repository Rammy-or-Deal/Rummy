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
}
