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
        string colorCharacter = "";
        switch (color)
        {
            case 0:
                colorCharacter = "A"; break;
            case 1:
                colorCharacter = "B"; break;
            case 2:
                colorCharacter = "C"; break;
            case 3:
                colorCharacter = "D"; break;
            default:
                colorCharacter = "A"; break;
        }
        var image = this.gameObject.GetComponent<Image> ();
        LogMgr.Inst.Log("Card update called. cardValue=" + "Card/" + colorCharacter + "" + num, (int)LogLevels.CardLog);
        image.sprite = Resources.Load<Sprite>("Card/" + colorCharacter + "" + num);
    }

    internal void Init(bool v)
    {
        isSelected = false;
        this.gameObject.SetActive(v);
        mCover.sprite = Resources.Load<Sprite>("Card/_black");
    }

    internal void moveDealCard(Vector3 srcPos)
    {
        iTween.MoveFrom(this.gameObject, srcPos, 3.0f);
        this.gameObject.SetActive(true);
    }

    internal void SetValue(Card card)
    {
        this.gameObject.SetActive(true);
        color = card.color;
        num = card.num;
        UpdateValue();
    }

    public Card GetValue()
    {
        Card card = new Card();
        card.num = num;
        card.color = color;
        return card;
    }
}
