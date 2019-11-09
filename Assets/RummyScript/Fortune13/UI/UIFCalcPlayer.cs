using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFCalcPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<FortuneCard> cardList;
    public Image levelImg;
    public Text coinText;
    public Text cardText;
    public int actorNumber;

    void Start()
    {
        var cards = GetComponentsInChildren<FortuneCard>();
        cardList = new List<FortuneCard>();
        foreach(var card in cards)
        {
            cardList.Add(card);
        }
    }

    internal void Init(FortuneUserSeat seat)
    {
        this.gameObject.SetActive(seat.IsSeat);
        actorNumber = seat.actorNumber;
        coinText.text = "";
        cardText.text = "";
    }

    internal void ShowCards(List<Card> showList)
    {
        foreach(var card in cardList)
            card.gameObject.SetActive(false);
        
        for(int i = 0; i < showList.Count; i++)
        {
            cardList[i].SetValue(showList[i]);
        }
    }
}
