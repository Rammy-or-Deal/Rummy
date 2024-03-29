﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class FortuneBot
{
    public int actorNumber;
    public List<Card> cardList;
    public FortuneBot()
    {
        cardList = new List<Card>();
    }
}
public class FortuneBotMgr : BotMgr
{
    // Start is called before the first frame update
    public static FortuneBotMgr Inst;
    public List<FortuneBot> botList;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            GameMgr.Inst.botMgr = this;
            base.CreateBot();
            botList = new List<FortuneBot>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void RejectThisBot(UserSeat bot)
    {
        base.RejectThisBot(bot);
        //OnPlayerLeftRoom_onlyMaster_bot
    }

    internal void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        if (actorNumber > 0) return;

        FortuneBot bot = new FortuneBot();
        bot.actorNumber = actorNumber;

        bot.cardList.Clear();
        var cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.CARD_LIST_STRING];
        LogMgr.Inst.Log("Card Received. cardString=" + cardString, (int)LogLevels.PlayerLog1);

        foreach (var str in cardString.Split(','))
        {
            Card card = new Card();
            card.cardString = str;
            bot.cardList.Add(card);
        }
        botList.Add(bot);


        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnUserReady},
                {Common.PLAYER_ID, actorNumber}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    internal void OnGameStarted()
    {

        foreach (var bot in botList)
        {
            var backList = new List<Card>();
            var frontList = new List<Card>();
            var middleList = new List<Card>();

            #region Making backList
            var tmpList = new List<Card>();
            for (int i = 0; i < bot.cardList.Count; i++)
            {
                Card card = new Card(bot.cardList[i].num, bot.cardList[i].color);
                tmpList.Add(card);
            }
            GameMgr.Inst.Log("tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
            FortuneRuleMgr.GetCardType(tmpList, ref tmpList);
            GameMgr.Inst.Log("sorted tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
            for (int i = 0; i < 5; i++)
            {
                Card card = new Card(tmpList[i].num, tmpList[i].color);
                if (tmpList[i].num == 14) card.num = 1;
                backList.Add(card);
                try
                {
                    bot.cardList.Remove(bot.cardList.Where(x => x.num == card.num && x.color == card.color).First());
                }
                catch
                {
                    bot.cardList.Remove(bot.cardList.Where(x => x.num == card.num).First());
                }
            }

            #endregion

            #region Making middleList
            tmpList.Clear();
            for (int i = 0; i < bot.cardList.Count; i++)
            {
                Card card = new Card(bot.cardList[i].num, bot.cardList[i].color);
                tmpList.Add(card);
            }
            GameMgr.Inst.Log("tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
            FortuneRuleMgr.GetCardType(tmpList, ref tmpList);
            GameMgr.Inst.Log("sorted tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
            for (int i = 0; i < 5; i++)
            {
                Card card = new Card(tmpList[i].num, tmpList[i].color);
                if (tmpList[i].num == 14) card.num = 1;
                middleList.Add(card);
                try
                {
                    bot.cardList.Remove(bot.cardList.Where(x => x.num == card.num && x.color == card.color).First());
                }
                catch
                {
                    bot.cardList.Remove(bot.cardList.Where(x => x.num == card.num).First());
                }
            }

            #endregion
            GameMgr.Inst.Log("last tmpList:=" + string.Join(", ", bot.cardList.Select(x => x.cardString)));
            #region Making frontList
            for (int i = 0; i < bot.cardList.Count; i++)
            {
                Card card = new Card(bot.cardList[i].num, bot.cardList[i].color);
                frontList.Add(card);
            }

            #endregion

            GameMgr.Inst.Log("tmpList backList:=" + string.Join(", ", backList.Select(x => x.cardString)));
            GameMgr.Inst.Log("tmpList middleList:=" + string.Join(", ", middleList.Select(x => x.cardString)));
            GameMgr.Inst.Log("tmpList frontList:=" + string.Join(", ", frontList.Select(x => x.cardString)));


            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable props = new Hashtable{
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnPlayerDealCard},
                    {Common.PLAYER_ID, bot.actorNumber},
                    {Common.FORTUNE_PLAYER_FRONT_CARD, string.Join(",", frontList.Select(x=>x.cardString))},
                    {Common.FORTUNE_PLAYER_MIDDLE_CARD, string.Join(",", middleList.Select(x=>x.cardString))},
                    {Common.FORTUNE_PLAYER_BACK_CARD, string.Join(",", backList.Select(x=>x.cardString))},
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
        botList.Clear();
    }
}
