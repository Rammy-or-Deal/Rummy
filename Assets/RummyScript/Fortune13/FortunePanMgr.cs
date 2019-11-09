using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class FortunePanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortunePanMgr Inst;
    public GameObject centerCard;
    public GameObject centerCoin;
    
    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnInitCard()
    {

    }
    public void OnCardDistributed()
    {
        var playerList = FortunePlayMgr.Inst.m_playerList;
        centerCard.SetActive(true);
        foreach (var player in playerList)
        {
            player.InitCards();
            player.moveDealCard(centerCard.transform.position);
        }
    }

    internal void OnOpenCard()
    {
        int lineNo = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_OPEN_CARD_LINE];
        var playerList = FortunePlayMgr.Inst.m_playerList;
        foreach (var user in FortunePlayMgr.Inst.userCardList)
        {
            try
            {
                var seat = playerList.Where(x => x.actorNumber == user.actorNumber).First();
                List<Card> showList = new List<Card>();
                switch (lineNo)
                {
                    case 0:
                        showList = user.frontCard;
                        break;
                    case 1:
                        showList = user.middleCard;
                        break;
                    case 2:
                        showList = user.backCard;
                        break;
                }
                seat.ShowCards(lineNo, showList);
            }
            catch
            {
                break;
            }
        }
    }
}
