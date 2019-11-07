using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FortuneMe : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortuneMe Inst;
    List<Card> cardList;
    void Start()
    {
        if(!Inst)
        {
            Inst = this;
            cardList = new List<Card>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        if(actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

        cardList.Clear();
        var cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.CARD_LIST_STRING];
        foreach(var str in cardString.Split(','))
        {
            Card card = new Card();
            card.cardString = str;
            cardList.Add(card);
        }
    }
}
