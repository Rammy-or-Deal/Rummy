using System;
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
            var tmp = bot.cardList.OrderBy(x => x.num).ToList();
            var frontList = new List<Card>();
            var middleList = new List<Card>();
            var backList = new List<Card>();
            for (int i = 0; i < tmp.Count; i++)
            {
                var card = new Card(tmp[i].num, tmp[i].color);
                if (i <= 2)
                {
                    frontList.Add(card);
                }
                else if (i <= 7)
                {
                    middleList.Add(card);
                }
                else
                {
                    backList.Add(card);
                }
            }
            var frontType = FortuneRuleMgr.GetCardType(frontList, ref frontList);
            var middleType = FortuneRuleMgr.GetCardType(middleList, ref middleList);
            var backType = FortuneRuleMgr.GetCardType(backList, ref backList);

            var frontScore = FortuneRuleMgr.GetScore(frontList, frontType);
            var middleScore = FortuneRuleMgr.GetScore(middleList, middleType);
            var backScore = FortuneRuleMgr.GetScore(backList, backType);

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
