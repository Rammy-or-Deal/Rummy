using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class RummyMeMgr : MeMgr
{
    
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.Inst.meMgr = this;
        PublishMe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void PublishMe()
    {
        base.PublishMe();
    }

    internal void OnReadyClick()
    {
        var myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        var info = RummyGameMgr.Inst.seatMgr.m_playerList.Where(x=>x.m_playerInfo.m_actorNumber == myActorNumber).First();
        BotMgr.PublishIamReady(info.m_playerInfo);
    }

    internal void ShowMyCards(string data)
    {
        Card[] cards = LamiCardMgr.ConvertCardStrToCardList((string)data);

        LamiGameUIManager.Inst.myCardPanel.InitCards(cards);
        LamiGameUIManager.Inst.InitButtonsFirst();
        
        LamiGameUIManager.Inst.myCardPanel.ArrangeMyCard();
        RummyGameMgr.Inst.isFirstTurn = true;
    }
}
