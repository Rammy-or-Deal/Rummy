using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LamiGameBot
{
    List<Card> original_cardList = new List<Card>(); // selected cards
    List<Card> remained_cardList = new List<Card>(); // selected cards
    public List<ATTACH_CLASS> availList = new List<ATTACH_CLASS>();
    public int id;
    public string name = "";
    public string pic;
    public string winRate;
    public string coinPic;
    public int coinValue;
    public string leafPic;
    public int leafValue;
    public string announce;
    public string message;
    public string email;
    public int giftItemId;
    public int giftItemCount;
    public int skillId;
    public int skillValue;
    public string skillLevel;
    public int frameId;
    public int friendItemId;
    public int requestId;
    public int status;
    LamiPlayerMgr parent;
    bool isFirstTurn = true;
    public string[] skillLevelList = new string[] { "Novice", "Expert", "Hero", "Elite", "King", "Master" };

    public LamiGameBot(LamiPlayerMgr parent)
    {
        this.parent = parent;
    }
    public void Init()
    {
        status = (int)LamiPlayerStatus.Ready;
        id = -(UnityEngine.Random.Range(1000, 9999));
        name = "Guest" + "[" + UnityEngine.Random.Range(1000, 9999).ToString() + "]";
        pic = "new_avatar/avatar_" + UnityEngine.Random.Range(1, 26).ToString();

        coinValue = UnityEngine.Random.Range(1000, 9999);
        leafValue = UnityEngine.Random.Range(100, 999);
        skillLevel = skillLevelList[UnityEngine.Random.Range(0, 6)];

        winRate = "12/20";
        leafPic = "new_symbol/leaf";

        announce = "Announce Text";
        message = "There is no message";
        email = "There is no email";
        coinPic = "new_symbol/coin";
        skillLevel = "expert";
        frameId = 3;
    }
    internal string getBotString()
    {
        //data : 0:id, 1:name, 2:picUrl, 3:coinValue, 4:skillLevel, 5:frameId
        //      format: id:name:picUrl:coinValue:skillLevel:frameId:status
        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                id,
                name,
                pic,
                coinValue,
                skillLevel,
                frameId,
                status
                );
        return infoString;
    }

    internal void SetBotInfo(string v)
    {
        LogMgr.Inst.Log("Bot String: " + v, (int)LogLevels.BotLog);

        var tmp = v.Split(':');
        if (tmp.Length > 5)
        {
            id = int.Parse(tmp[0]);
            name = tmp[1];
            pic = tmp[2];
            coinValue = int.Parse(tmp[3]);
            skillLevel = tmp[4];
            frameId = int.Parse(tmp[5]);
            status = int.Parse(tmp[6]);
        }
    }
    public void PublishMe()
    {
        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                id,
                name,
                pic,
                coinValue,
                skillLevel,
                frameId,
                status
                );

        // Send Add New player Message. - OnUserEnteredRoom
        Hashtable props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserEnteredRoom_M},
                {Common.NEW_PLAYER_INFO, infoString},
                {Common.NEW_PLAYER_STATUS, status},
                {Common.IS_BOT, true}
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        ReadyMessageSend();
    }
    async void ReadyMessageSend()
    {
        try
        {
            await Task.Delay(1000);
            status = (int)LamiPlayerStatus.Ready;

            Hashtable props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserReady_BOT},
                {Common.BOT_ID, id},
                {Common.BOT_STATUS, status},
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        catch { }
    }
    internal void SetMyCards(string cardString)
    {
        var tmp = LamiCardMgr.ConvertCardStrToCardList(cardString);
        remained_cardList.Clear();
        original_cardList.Clear();
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].MyCardId = i;
            remained_cardList.Add(tmp[i]);
            original_cardList.Add(tmp[i]);
        }
    }

    internal void SetMyTurn()
    {
        //status = (int)LamiPlayerStatus.Burnt;
        //SendMyStatus();
        if (!Init_FlashList()) return;

        DealCard();
    }

    private async void DealCard()
    {
        await Task.Delay(Constants.botWaitTime*1000);
        var list = availList[0].list;
        string cardStr = LamiCardMgr.ConvertSelectedListToString(availList[0].list);
        cardStr = id + ":" + cardStr;

        int remainCard = remained_cardList.Count - list.Count;

        Hashtable gameCards = new Hashtable
        {
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
            {Common.PLAYER_ID, id},
            {Common.REMAIN_CARD_COUNT, remainCard},
            {Common.GAME_CARD, cardStr},
            {Common.GAME_CARD_PAN, availList[0].lineNo},
        };
        try{
        PhotonNetwork.CurrentRoom.SetCustomProperties(gameCards);
        }catch{}
        LogMgr.Inst.Log("Bot dealt card: " + id + " ------" + cardStr, (int)LogLevels.BotLog);
        isFirstTurn = false;
    }

    private bool Init_FlashList()
    {
        LogMgr.Inst.ShowLog(remained_cardList, "Bot" + id, (int)LogLevels.BotLog);
        try
        {
            for (int i = 0; i < availList.Count; i++)
            {
                availList[i].list.Clear();
            }
            availList.Clear();
        }
        catch { }

        List<List<Card>> panList = new List<List<Card>>();
        foreach (var list in LamiGameUIManager.Inst.mGameCardPanelList)
        {
            panList.Add(list.mGameCardList);
        }

        remained_cardList.Sort((x, y) => x.color - y.color);

        string tmp = "";
        foreach (var card in remained_cardList)
        {
            tmp += card.num + "(" + card.color + ")" + "/";
        }
        var flushList = LamiMe.GetAvailableCards_Flush_With_Card(remained_cardList);
        var setList = LamiMe.GetAvailableCards_Set_With_Card(remained_cardList);

        var allFlushList = LamiMe.FilterByCurrentTurn(flushList, panList, isFirstTurn);
        var allSetList = LamiMe.FilterByCurrentTurn(setList, panList, isFirstTurn);



        var allFlushList_nonJoker = allFlushList.Where(x => x.list.Count(y => y.num == 15) == 0).ToList();
        allFlushList_nonJoker.Sort((a, b) => b.list.Sum(x => x.virtual_num) - a.list.Sum(x => x.virtual_num));

        var allFlushList_Joker = allFlushList.Where(x => x.list.Count(y => y.num == 15) > 0).ToList();
        allFlushList_Joker.Sort((a, b) => a.list.Count(x => x.num == 15) - b.list.Count(x => x.num == 15));

        var allSetList_nonJoker = allSetList.Where(x => x.list.Count(y => y.num == 15) == 0).ToList();
        allSetList_nonJoker.Sort((a, b) => b.list.Sum(x => x.virtual_num) - a.list.Sum(x => x.virtual_num));

        var allSetList_Joker = allSetList.Where(x => x.list.Count(y => y.num == 15) > 0).ToList();
        allSetList_Joker.Sort((a, b) => a.list.Count(x => x.num == 15) - b.list.Count(x => x.num == 15));

        availList.AddRange(allFlushList_nonJoker);
        availList.AddRange(allSetList_nonJoker);
        availList.AddRange(allFlushList_Joker);
        availList.AddRange(allSetList_Joker);

        if (availList.Count == 0)
        {
            if (isFirstTurn)
            {
                status = (int)LamiPlayerStatus.Burnt;
            }
            else
            {
                status = (int)LamiPlayerStatus.GiveUp;
            }

            Hashtable props = new Hashtable{
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnPlayerStatusChanged},
                {Common.PLAYER_ID, id},
                {Common.PLAYER_STATUS, status},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            return false;
        }
        Debug.Log("BOT Avail String=(" + id + ")   ");
        LogMgr.Inst.ShowLog(availList.Select(x => x.list).ToList(), "Bot Avail String", (int)LogLevels.BotLog);
        //LogMgr.Inst.ShowLog(remained_cardList, "Bot"+id, (int)LogLevels.BotLog);
        Debug.Log("---------------------");
        return true;

    }

    private async void SendMyStatus()
    {
        await Task.Delay(1000);
        Hashtable props = new Hashtable{
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnPlayerStatusChanged},
                {Common.PLAYER_ID, id},
                {Common.PLAYER_STATUS, status},
            };
        try
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        catch { }
    }

    internal void OnUserDealt(string dealString)
    {
        var numList = dealString.Split(':')[1].Split(',').Select(Int32.Parse).ToArray();
        var colList = dealString.Split(':')[2].Split(',').Select(Int32.Parse).ToArray();

        for (int i = 0; i < numList.Length; i++)
        {
            foreach (var card in remained_cardList)
            {
                if ((card.num == numList[i] && card.color == colList[i]) || (card.num == 15 && numList[i] == 15))
                {
                    remained_cardList.Remove(card);
                    break;
                }
            }
        }
    }
    /*************************************************** */
}