using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
public class LamiPanMgr : MonoBehaviour
{
    public List<LamiCardLine> m_cardLineList = new List<LamiCardLine>();

    public static LamiPanMgr Inst;
    private void Awake()
    {
        if (!Inst)
            Inst = this;
    }
    public void Add(string data)
    {
        // data format: lineNumber-color:number,color:number,...
        var lineNumber = int.Parse(data.Split('-')[0]);
        var cards = data.Split('-')[1];

        LamiCardLine line;

        if (lineNumber != 0)    // if the dealed card is in existing line
        {
            line = m_cardLineList[lineNumber];
        }
        else     // if card is in new line
        {
            line = new LamiCardLine(m_cardLineList.Count + 1);
            m_cardLineList.Add(line);
        }
        line.Add(cards);

        // Show
        Show();
    }

    private void Show()
    {
        for (int i = 0; i < m_cardLineList.Count; i++)
        {
            m_cardLineList[i].Show();
        }
    }

    internal void OnDealCard()
    {
        //         Hashtable gameCards = new Hashtable
        // {   
        //     {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
        //     {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
        //     {Common.REMAIN_CARD_COUNT, remainCard},
        //     {Common.GAME_CARD, cardStr},
        //     {Common.GAME_CARD_PAN, 0},
        // };
        string cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.GAME_CARD];

        LamiGameUIManager.Inst.OnDealCard(cardString);
    }

    internal void OnGameRestart()
    {
        foreach(var line in m_cardLineList)
        {
            line.Init_Clear();
        }
    }
}
