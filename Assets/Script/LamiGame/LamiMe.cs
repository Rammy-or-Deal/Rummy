using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.UI;

public class LamiMe : MeMgr
{
    public bool isAuto = false;
    public Text m_timer_description;
    public bool isFirstTurn = true;
    public List<ATTACH_CLASS> availList;
    int nowFlush = 0;
    public static LamiMe Inst;

    public List<LamiMyCard> m_cardList = new List<LamiMyCard>(); // my cards
    List<LamiMyCard> sel_cards = new List<LamiMyCard>(); // selected cards

    int cardLineNumber;
    private bool IsSortClicked = false;

    public LamiMe(bool isAuto, Text timer_description, bool isFirstTurn, List<ATTACH_CLASS> availList, int nowFlush, List<LamiMyCard> cardList, List<LamiMyCard> sel_cards, int cardLineNumber, bool isSortClicked, List<int> avail_lineList, LamiPlayerMgr parent, int status)
    {
        this.isAuto = isAuto;
        m_timer_description = timer_description;
        this.isFirstTurn = isFirstTurn;
        this.availList = availList;
        this.nowFlush = nowFlush;
        m_cardList = cardList;
        this.sel_cards = sel_cards;
        this.cardLineNumber = cardLineNumber;
        IsSortClicked = isSortClicked;
        this.avail_lineList = avail_lineList;
        this.parent = parent;
        this.status = status;
    }

    List<int> avail_lineList = new List<int>();
    LamiPlayerMgr parent;

    public int status;

    private void Start()
    {
        if (!Inst)
        {
            Inst = this;
            PublishMe();
        }
        LamiCountdownTimer.Inst.StartTimer();
        status = (int)enumPlayerStatus.Rummy_Init;
        availList = new List<ATTACH_CLASS>();
        // try
        // {
        //     PublishMe();
        // }
        // catch { }
    }

    public override void PublishMe()
    {
        base.PublishMe();
    }

    internal void OnClickShuffle()
    {
        string cardString = "";
        //LamiGameUIManager.Inst.myCardPanel.myCards
        int count = LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true);    // Get Selected Card
        if (count != 3)
        {
            LamiGameUIManager.Inst.shuffleButton.interactable = false;
            return;
        }
        foreach (var card in LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList())
        {
            cardString += card.num + ":" + card.color + ":" + card.MyCardId + ",";
        }
        cardString = cardString.Trim(',');

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnShuffleRequest},
            {Common.SHUFFLE_CARDS, cardString}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void SendMyStatus()
    {
        // Hashtable props = new Hashtable{
        //         {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserReady},
        //         {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
        //         {Common.PLAYER_STATUS, status}
        //     };
        // PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // var myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        // var info = RummyGameMgr.Inst.seatMgr.m_userList.Where(x=>x.m_playerInfo.m_actorNumber == myActorNumber).First();
        // BotMgr.PublishIamReady(info.m_playerInfo);
    }
    public void OnReadyButtonClicked()
    {
        status = (int)enumPlayerStatus.Rummy_Ready;
        SendMyStatus();
    }

    public void SetMyCards(string data)
    {        
        Card[] cards = LamiCardMgr.ConvertCardStrToCardList((string)data);

        LamiGameUIManager.Inst.myCardPanel.InitCards(cards);
        LamiGameUIManager.Inst.InitButtonsFirst();

        // Set I am ready to Start
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserReadyToStart_M},
            {Common.PLAYER_STATUS, (int)enumPlayerStatus.Rummy_ReadyToStart}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        LamiGameUIManager.Inst.myCardPanel.ArrangeMyCard();
        isFirstTurn = true;
    }
    public void SelectTipFirstCard()
    {
        nowFlush = 0;
        SelectTipCard();
    }
    public void SelectTipCard()
    {
        if (availList.Count == 0) return;

        if (availList.Count < nowFlush) nowFlush = 0;

        // unselect all cards
        for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
        {
            LamiGameUIManager.Inst.myCardPanel.myCards[i].isSelected = false;
        }

        string log = "";

        // select tip cards
        for (int i = 0; i < availList[nowFlush].list.Count; i++)
        {
            log += availList[nowFlush].list[i].num;
            if (availList[nowFlush].list[i].num == 15)
                log += "(" + availList[nowFlush].list[i].virtual_num + ")";
            log += "-" + availList[nowFlush].list[i].MyCardId;
            log += ",";
            LamiGameUIManager.Inst.myCardPanel.myCards[availList[nowFlush].list[i].MyCardId].isSelected = true;
        }

        // Update Card UI
        for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
        {
            LamiGameUIManager.Inst.myCardPanel.myCards[i].SetUpdate();
        }

        // Update Play Button
        LamiGameUIManager.Inst.myCardPanel.SetPlayButtonState();

        LogMgr.Inst.Log("now tip " + log, (int)LogLevels.PlayerLog2);

        LogMgr.Inst.Log("now tip turn = " + nowFlush + " / total= " + availList.Count, (int)LogLevels.PlayerLog2);

        nowFlush++;
        try
        {
            nowFlush = nowFlush % availList.Count;
        }
        catch { }
    }

    internal void OnGameRestart()
    {
        StopAllCoroutines();
        LamiCountdownTimer.Inst.StopTurnTimer();

        isAuto = false;
        isFirstTurn = true;

        LamiGameUIManager.Inst.Init_Clear();
        foreach (LamiUserSeat player in LamiPlayerMgr.Inst.m_playerList)
        {
            player.Init_Clear();
            if (player.m_playerInfo.m_actorNumber < 0)
            {
                player.m_playerInfo.m_status = enumPlayerStatus.Rummy_Ready;
                BotMgr.PublishIamReady(player.m_playerInfo);
            }
        }
    }



    internal void OnShuffleAccept()
    {
        var cardString = (string)PhotonNetwork.LocalPlayer.CustomProperties[Common.SHUFFLE_CARDS];

        var cardList = cardString.Split(',');
        for (int i = 0; i < cardList.Length; i++)
        {
            var tmp = cardList[i].Split(':').Select(Int32.Parse).ToArray();
            var card = LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.MyCardId == tmp[2]).First();
            card.num = tmp[0];
            card.color = tmp[1];
            card.UpdateValue();
        }
        Init_FlashList();
        LamiGameUIManager.Inst.playButton.interactable = false;
    }



    public void Init_FlashList()
    {
        try
        {
            for (int i = 0; i < availList.Count; i++)
            {
                availList[i].list.Clear();
            }
            availList.Clear();
        }
        catch { }
        nowFlush = 0;

        List<List<Card>> panList = new List<List<Card>>();
        foreach (var list in LamiGameUIManager.Inst.mGameCardPanelList)
        {
            panList.Add(list.mGameCardList);
        }


        List<ATTACH_CLASS> allFlushList = FilterByCurrentTurn_FLUSH(GetAvailableCards_Flush(LamiGameUIManager.Inst.myCardPanel.myCards), panList, isFirstTurn);
        List<ATTACH_CLASS> allSetList = new List<ATTACH_CLASS>();
        List<ATTACH_CLASS> allJokerList = new List<ATTACH_CLASS>();
        if (!isFirstTurn)
        {
            allSetList = FilterByCurrentTurn_SET(GetAvailableCards_Set(LamiGameUIManager.Inst.myCardPanel.myCards), panList);
            allJokerList = FilterByCurrentTurn_JOKER(GetAvailableCards_Joker(LamiGameUIManager.Inst.myCardPanel.myCards), panList);
        }

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
        if (allJokerList.Count > 0)
            availList.AddRange(allJokerList);
        if (availList.Count == 0)
        {
            if (isFirstTurn)
            {
                status = (int)enumPlayerStatus.Rummy_Burnt;
            }
            else
            {
                status = (int)enumPlayerStatus.Rummy_GiveUp;
            }

            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnPlayerStatusChanged},
                {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
                {Common.PLAYER_STATUS, status},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            LamiGameUIManager.Inst.myCardPanel.ClearMachedCardList();
            return;
        }
        CheckAllList("availList");
    }

    public static List<ATTACH_CLASS> FilterByCurrentTurn_JOKER(List<List<Card>> AllList, List<List<Card>> panList)
    {
        List<ATTACH_CLASS> resList = new List<ATTACH_CLASS>();

        for (int j = 0; j < panList.Count; j++)
        {
            var line = panList[j];
            bool isFlush = true;
            int firstNum = line[0].virtual_num;
            for (int tttt = 1; tttt < line.Count; tttt++)
            {
                if (line[tttt].virtual_num == firstNum)
                {
                    isFlush = false;
                    break;
                }
            }


            if (isFlush)
            {
                foreach (var list in AllList.Where(x => x.Count > 0))
                {
                    if (line[0].virtual_num - 1 >= list.Count) // Attach to the front
                    {



                        ATTACH_CLASS new_list = new ATTACH_CLASS();
                        new_list.list = new List<Card>();
                        new_list.lineNo = j;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Card new_card = new Card();
                            new_card.num = list[i].num;
                            new_card.virtual_num = line[0].virtual_num - 1 - i;
                            new_card.color = line[0].color;
                            new_card.MyCardId = list[i].MyCardId;
                            new_list.list.Add(new_card);
                        }
                        if (line[line.Count - 1].virtual_num != 14 || new_list.list[0].virtual_num != 1)
                            if (new_list.list.Count > 0)
                                resList.Add(new_list);
                    }

                    if ((14 - line[line.Count - 1].virtual_num) >= list.Count)  // attach to the last
                    {
                        ATTACH_CLASS new_list = new ATTACH_CLASS();
                        new_list.list = new List<Card>();
                        new_list.lineNo = j;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Card new_card = new Card();
                            new_card.num = list[i].num;
                            new_card.virtual_num = line[line.Count - 1].virtual_num + 1 + i;
                            new_card.color = line[line.Count - 1].color;
                            new_card.MyCardId = list[i].MyCardId;
                            new_list.list.Add(new_card);
                        }
                        if (line[0].virtual_num != 14 || new_list.list[new_list.list.Count - 1].virtual_num != 14)
                            if (new_list.list.Count > 0)
                                resList.Add(new_list);
                    }
                }
            }
            else    // It's set, add
            {

                foreach (var list in AllList.Where(x => x.Count > 0))
                {
                    ATTACH_CLASS new_list = new ATTACH_CLASS();
                    new_list.lineNo = j;
                    new_list.list = new List<Card>();

                    foreach (var card in list)
                    {
                        Card new_card = new Card(card.num, card.color);
                        new_card.MyCardId = card.MyCardId;
                        new_card.virtual_num = line[0].virtual_num;
                        new_list.list.Add(new_card);
                    }

                    if (new_list.list.Count > 0)
                        resList.Add(new_list);
                }
                //new_list.list.AddRange(new_list);
            }
        }
        //resList.Sort((a,b)=>(a.list.Count() - b.list.Count()));
        return resList;
    }

    public static List<List<Card>> GetAvailableCards_Joker(List<LamiMyCard> myCurrent)
    {
        List<Card> m_tmpCardList = new List<Card>();

        m_tmpCardList.Clear();

        for (int i = 0; i < myCurrent.Count; i++)
        {
            Card card = new Card();
            card.color = myCurrent[i].color;
            card.num = myCurrent[i].num;
            card.MyCardId = i;
            card.virtual_num = card.num;
            m_tmpCardList.Add(card);
        }
        return GetAvailableCards_Joker_With_Card(m_tmpCardList);
    }

    public static List<List<Card>> GetAvailableCards_Joker_With_Card(List<Card> m_tmpCardList)
    {

        List<List<Card>> same_List = new List<List<Card>>();
        //bool con = false;

        // Add first card
        for (int i = 0; i < m_tmpCardList.Count; i++)
        {
            if (m_tmpCardList[i].num == 15)
            {
                List<Card> tt11 = new List<Card>();
                tt11.Add(m_tmpCardList[i]);
                same_List.Add(tt11);
                break;
            }
        }

        // Append joker

        //int count = same_List.Count;
        //for (int i = 0; i < count; i++)
        //{
        var list = m_tmpCardList.Where(x => x.num == 15 && x.MyCardId > same_List[0][0].MyCardId).ToList();
        List<Card> tmpList = new List<Card>();

        foreach (var card in list)
        {
            Card new_card = new Card(15, same_List[0][0].color);
            new_card.MyCardId = card.MyCardId;
            tmpList.Add(new_card);

            List<Card> newList = new List<Card>();
            newList.AddRange(same_List[0]);

            newList.AddRange(tmpList.ToList());
            same_List.Add(newList);
        }
        //break;
        //}


        return same_List;
    }

    public static List<ATTACH_CLASS> FilterByCurrentTurn_SET(List<List<Card>> AllList, List<List<Card>> panList)
    {
        List<ATTACH_CLASS> resList = new List<ATTACH_CLASS>();

        List<ATTACH_CLASS> attachList = new List<ATTACH_CLASS>();
        List<ATTACH_CLASS> addList = new List<ATTACH_CLASS>();

        LogMgr.Inst.Log("------------------------------------ All lines -------------------------------------------", (int)LogLevels.SpecialLog);
        LogMgr.Inst.ShowLog(AllList);
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);



        LogMgr.Inst.Log("------------------------------------ Pane  lines -------------------------------------------", (int)LogLevels.SpecialLog);
        for (int j = 0; j < panList.Count; j++)
        {
            LogMgr.Inst.ShowLog(panList[j]);
        }
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);


        bool canAttach = false;

        // Get all cards
        for (int i = 0; i < AllList.Count; i++)
        {
            canAttach = false;

            int firstNum = AllList[i][0].virtual_num;

            //Check if the card can attach to the existing lines.
            for (int j = 0; j < panList.Count && canAttach == false; j++)
            {
                var line = panList[j];

                bool isFlush = true;
                firstNum = line[0].virtual_num;
                for (int tttt = 1; tttt < line.Count; tttt++)
                {
                    if (line[tttt].virtual_num == firstNum)
                    {
                        isFlush = false;
                        break;
                    }
                }

                if (AllList[i].Count == 1 && !isFlush)
                {
                    if (AllList[i][0].virtual_num == line[0].virtual_num)
                    {
                        canAttach = true;

                        ATTACH_CLASS new_item = new ATTACH_CLASS();
                        new_item.lineNo = j;
                        new_item.list = new List<Card>();
                        new_item.list.AddRange(AllList[i].ToList());

                        attachList.Add(new_item);

                        CheckCondition("SET 1 Success", AllList[i], line);
                        continue;
                    }
                    else
                    {
                        CheckCondition("SET 1 Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("SET 1 First Condition error", AllList[i], line);
                }


                if (AllList[i].Count > 1 && !isFlush)
                {
                    if (AllList[i][0].virtual_num == line[0].virtual_num)
                    {
                        if (AllList[i][0].virtual_num == line[1].virtual_num)
                        {
                            canAttach = true;

                            ATTACH_CLASS new_item = new ATTACH_CLASS();
                            new_item.lineNo = j;
                            new_item.list = new List<Card>();
                            new_item.list.AddRange(AllList[i].ToList());
                            attachList.Add(new_item);
                            CheckCondition("SET 22 Success", AllList[i], line);
                            continue;
                        }
                        else
                        {
                            CheckCondition("SET 22 Third Condition error", AllList[i], line);
                        }
                    }
                    else
                    {
                        CheckCondition("SET 22 Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("SET 22 First Condition error", AllList[i], line);
                }

            }
            //Check if the card can add to new Line
            if (canAttach == false && AllList[i].Count >= 3)
            {
                ATTACH_CLASS new_item = new ATTACH_CLASS();
                new_item.lineNo = -1;
                new_item.list = new List<Card>();
                new_item.list.AddRange(AllList[i].ToList());
                addList.Add(new_item);
            }
        }

        if (attachList.Count > 0)
        {
            resList.AddRange(attachList);
            LogMgr.Inst.Log("attach List is added", (int)LogLevels.SpecialLog);
        }


        if (addList.Count > 0)
        {
            resList.AddRange(addList);
        }
        return resList;
    }
    public static List<ATTACH_CLASS> FilterByCurrentTurn_FLUSH(List<List<Card>> AllList, List<List<Card>> panList, bool isFirst = false)
    {
        Debug.Log("Is First: = " + isFirst);
        List<ATTACH_CLASS> resList = new List<ATTACH_CLASS>();

        List<ATTACH_CLASS> attachList = new List<ATTACH_CLASS>();
        List<ATTACH_CLASS> addList = new List<ATTACH_CLASS>();

        LogMgr.Inst.Log("------------------------------------ All lines -------------------------------------------", (int)LogLevels.SpecialLog);
        LogMgr.Inst.ShowLog(AllList);
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);



        LogMgr.Inst.Log("------------------------------------ Pane  lines -------------------------------------------", (int)LogLevels.SpecialLog);
        for (int j = 0; j < panList.Count; j++)
        {
            LogMgr.Inst.ShowLog(panList[j]);
        }
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);


        bool canAttach = false;

        // Get all cards
        for (int i = 0; i < AllList.Count; i++)
        {
            canAttach = false;

            int firstNum = AllList[i][0].virtual_num;

            //Check if the card can attach to the existing lines.
            for (int j = 0; j < panList.Count; j++)
            {
                var line = panList[j];

                bool isFlush = true;
                firstNum = line[0].virtual_num;
                for (int tttt = 1; tttt < line.Count; tttt++)
                {
                    if (line[tttt].virtual_num == firstNum)
                    {
                        isFlush = false;
                        break;
                    }
                }
                if (!isFlush) continue;

                // Check if A is in the last or first of the pan line, and selected card has A

                if (line[0].virtual_num == 1 && AllList[i][AllList[i].Count - 1].virtual_num == 14) continue;
                if (line[line.Count - 1].virtual_num == 14 && AllList[i][0].virtual_num == 1) continue;


                // Check if it matches in normal method
                if (AllList[i][AllList[i].Count - 1].virtual_num == line[0].virtual_num - 1 || // can attach  dealt card to first
                        AllList[i][0].virtual_num == line[line.Count - 1].virtual_num + 1)
                {
                    if (isFlush)
                    {
                        if (AllList[i][0].color == line[0].color)
                        {
                            canAttach = true;
                            ATTACH_CLASS new_item = new ATTACH_CLASS();
                            new_item.lineNo = j;
                            new_item.list = new List<Card>();
                            new_item.list.AddRange(AllList[i].ToList());

                            attachList.Add(new_item);
                            CheckCondition("Flush Success", AllList[i], line);
                        }
                        else
                        {
                            CheckCondition("Flush Third Condition error", AllList[i], line);
                        }
                    }
                    else
                    {
                        CheckCondition("Flush Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("Flush First Condition error", AllList[i], line);
                }
            }
            //Check if the card can add to new Line
            if (AllList[i].Count >= 3)
            {
                ATTACH_CLASS new_item = new ATTACH_CLASS();
                new_item.lineNo = -1;
                new_item.list = new List<Card>();
                new_item.list.AddRange(AllList[i].ToList());
                addList.Add(new_item);
            }
        }

        if (!isFirst && attachList.Count > 0)
        {
            resList.AddRange(attachList);
            LogMgr.Inst.Log("attach List is added", (int)LogLevels.SpecialLog);
        }
        else
        {
            LogMgr.Inst.Log("attach List isn't added, Reason. !isFirst=" + (!isFirst) + ", attachList.Count=" + attachList.Count, (int)LogLevels.SpecialLog);
        }


        if (addList.Count > 0)
        {
            resList.AddRange(addList);
        }
        return resList;
    }

    public void CheckAllList(string header)
    {
        LogMgr.Inst.Log("--------------" + header + "-------------", (int)LogLevels.ShowLamiAllList);
        //LogMgr.Inst.ShowLog(availList, header, (int)LogLevels.ShowLamiAllList);
        //LogMgr.Inst.ShowLog(second, header);
        LogMgr.Inst.Log("------------------------------------------------", (int)LogLevels.ShowLamiAllList);
    }

    public static void CheckCondition(string header, List<Card> first, List<Card> second)
    {
        LogMgr.Inst.Log("--------------" + header + "-------------", (int)LogLevels.SpecialLog);
        LogMgr.Inst.ShowLog(first, header);
        //LogMgr.Inst.ShowLog(second, header);
        LogMgr.Inst.Log("------------------------------------------------", (int)LogLevels.SpecialLog);
    }

    public static List<ATTACH_CLASS> FilterByCurrentTurn(List<List<Card>> AllList, List<List<Card>> panList, bool isFirst = false)
    {
        Debug.Log("Is First: = " + isFirst);
        List<ATTACH_CLASS> resList = new List<ATTACH_CLASS>();

        List<ATTACH_CLASS> attachList = new List<ATTACH_CLASS>();
        List<ATTACH_CLASS> addList = new List<ATTACH_CLASS>();

        LogMgr.Inst.Log("------------------------------------ All lines -------------------------------------------", (int)LogLevels.SpecialLog);
        LogMgr.Inst.ShowLog(AllList);
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);



        LogMgr.Inst.Log("------------------------------------ Pane  lines -------------------------------------------", (int)LogLevels.SpecialLog);
        for (int j = 0; j < panList.Count; j++)
        {
            LogMgr.Inst.ShowLog(panList[j]);
        }
        LogMgr.Inst.Log("--------------------- end -------------------", (int)LogLevels.SpecialLog);


        bool canAttach = false;

        // Get all cards
        for (int i = 0; i < AllList.Count; i++)
        {
            canAttach = false;

            bool isFlush_created = true;
            int firstNum = AllList[i][0].virtual_num;


            for (int kk = 1; kk < AllList[i].Count; kk++)
            {
                if (AllList[i][kk].virtual_num == firstNum)
                {
                    isFlush_created = false;
                    break;
                }
            }

            //Check if the card can attach to the existing lines.
            for (int j = 0; j < panList.Count && canAttach == false; j++)
            {
                var line = panList[j];

                bool isFlush = true;
                firstNum = line[0].virtual_num;
                for (int tttt = 1; tttt < line.Count; tttt++)
                {
                    if (line[tttt].virtual_num == firstNum)
                    {
                        isFlush = false;
                        break;
                    }
                }

                if (AllList[i][AllList[i].Count - 1].virtual_num == line[0].virtual_num - 1 || // can attach  dealt card to first
                        AllList[i][0].virtual_num == line[line.Count - 1].virtual_num + 1)
                {
                    if (isFlush)
                    {
                        if (AllList[i][0].color == line[0].color)
                        {
                            if (isFlush_created)
                            {
                                canAttach = true;
                                ATTACH_CLASS new_item = new ATTACH_CLASS();
                                new_item.lineNo = j;
                                new_item.list = new List<Card>();
                                new_item.list.AddRange(AllList[i].ToList());

                                attachList.Add(new_item);
                                CheckCondition("Flush Success", AllList[i], line);
                                continue;
                            }
                            else
                            {
                                CheckCondition("Flush Fourth Condition error", AllList[i], line);
                            }
                        }
                        else
                        {
                            CheckCondition("Flush Third Condition error", AllList[i], line);
                        }
                    }
                    else
                    {
                        CheckCondition("Flush Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("Flush First Condition error", AllList[i], line);
                }


                if (AllList[i].Count == 1 && !isFlush)
                {
                    if (AllList[i][0].virtual_num == line[0].virtual_num)
                    {
                        canAttach = true;

                        ATTACH_CLASS new_item = new ATTACH_CLASS();
                        new_item.lineNo = j;
                        new_item.list = new List<Card>();
                        new_item.list.AddRange(AllList[i].ToList());

                        attachList.Add(new_item);

                        CheckCondition("SET 1 Success", AllList[i], line);
                        continue;
                    }
                    else
                    {
                        CheckCondition("SET 1 Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("SET 1 First Condition error", AllList[i], line);
                }


                if (AllList[i].Count > 1 && !isFlush && !isFlush_created)
                {
                    if (AllList[i][0].virtual_num == line[0].virtual_num)
                    {
                        if (AllList[i][0].virtual_num == line[1].virtual_num)
                        {
                            canAttach = true;

                            ATTACH_CLASS new_item = new ATTACH_CLASS();
                            new_item.lineNo = j;
                            new_item.list = new List<Card>();
                            new_item.list.AddRange(AllList[i].ToList());
                            attachList.Add(new_item);
                            CheckCondition("SET 22 Success", AllList[i], line);
                            continue;
                        }
                        else
                        {
                            CheckCondition("SET 22 Third Condition error", AllList[i], line);
                        }
                    }
                    else
                    {
                        CheckCondition("SET 22 Second Condition error", AllList[i], line);
                    }
                }
                else
                {
                    CheckCondition("SET 22 First Condition error", AllList[i], line);
                }

            }
            //Check if the card can add to new Line
            if (canAttach == false && AllList[i].Count >= 3)
            {
                ATTACH_CLASS new_item = new ATTACH_CLASS();
                new_item.lineNo = -1;
                new_item.list = new List<Card>();
                new_item.list.AddRange(AllList[i].ToList());
                addList.Add(new_item);
            }
        }

        if (!isFirst && attachList.Count > 0)
        {
            resList.AddRange(attachList);
            LogMgr.Inst.Log("attach List is added", (int)LogLevels.SpecialLog);
        }
        else
        {
            LogMgr.Inst.Log("attach List isn't added, Reason. !isFirst=" + (!isFirst) + ", attachList.Count=" + attachList.Count, (int)LogLevels.SpecialLog);
        }


        if (addList.Count > 0)
        {
            if (isFirst)
            {
                foreach (var line in addList)
                {
                    if (line.list[0].virtual_num != line.list[1].virtual_num)
                    {
                        resList.Add(line);
                    }
                }
            }
            else
            {
                resList.AddRange(addList);
            }
        }
        return resList;
    }

    public void OnClickLine(int lineNum)
    {
        LamiGameUIManager.Inst.myCardPanel.OnClickLine(lineNum);
    }
    internal void SetMyTurn(bool isMyTurn)
    {

        if (isMyTurn)
        {
            LogMgr.Inst.Log("This is my turn: " + PhotonNetwork.LocalPlayer.ActorNumber, (int)LogLevels.PlayerLog2);

            LamiGameUIManager.Inst.playButton.gameObject.SetActive(true);
            LamiGameUIManager.Inst.tipButton.gameObject.SetActive(true);
            m_timer_description.gameObject.SetActive(true);
            //LamiPlayerMgr.Inst.GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(true);
            //LamiCountdownTimer.Inst.turnTime = LamiPlayerMgr.Inst.GetUserSeat(PhotonNetwork.LocalPlayer).mClockTime;
            //LamiCountdownTimer.Inst.StartTurnTimer();

            // Get all available cards
            GameMgr.Inst.Log("Remain Card me:=" + LamiGameUIManager.Inst.myCardPanel.myCards.Count);
            Init_FlashList();

            LamiGameUIManager.Inst.myCardPanel.SetPlayButtonState();
        }
        else
        {
            LamiGameUIManager.Inst.playButton.gameObject.SetActive(false);
            LamiGameUIManager.Inst.tipButton.gameObject.SetActive(false);
            m_timer_description.gameObject.SetActive(false);
            //LamiCountdownTimer.Inst.StopTurnTimer();
        }
    }
    /************************* */

    //get selected cards are SET
    private bool IsSelSet()
    {
        int first_num = -1;
        bool isSet = false; // true: SET     false: FLUSH
        for (int i = 0; i < sel_cards.Count; i++)
        {
            if (first_num == -1) // Get the first number
            {
                first_num = Math.Abs(sel_cards[i].num);
            }
            else
            {
                isSet = true;
                if (first_num != Math.Abs(sel_cards[i].num)) // If first_num <> sel_cards[i] then, this line is not SET. it's FLUSH.
                    isSet = false;
                break;
            }
        }

        return isSet;
    }

    //get selected cards are FLUSH
    private bool IsSelFlush()
    {
        int first_num = -1;
        int first_col = -1; // true: FLUSH
        bool isFlush = false;
        for (int i = 0; i < sel_cards.Count; i++)
        {
            if (first_num == -1) // Get the first number
            {
                first_num = Math.Abs(sel_cards[i].num) - 1;
                first_col = sel_cards[i].color;
            }
            else
            {
                isFlush = true;
                first_num++;
                if (first_num != Math.Abs(sel_cards[i].num) || first_col != sel_cards[i].color
                ) // If first_num <> sel_cards[i] then, this line is not FLUSH.
                    isFlush = false;
                break;
            }
        }

        return isFlush;
    }

    private void Sort()
    {
        // Sort cards
        // If this line is SET, we will not sort.
        if (IsSelSet()) return;

        LamiMyCard[] array = sel_cards.ToArray();
        sel_cards.Clear();
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (Math.Abs(array[i].num) > Math.Abs(array[j].num))
                {
                    //SwitchCard(array[i], array[j]);
                }
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            sel_cards.Add(array[i]);
        }
    }
    public static List<List<Card>> GetAvailableCards_Set_With_Card(List<Card> m_tmpCardList)
    {
        List<List<Card>> same_List = new List<List<Card>>();

        bool con = false;

        // Add first card
        for (int i = 0; i < m_tmpCardList.Count; i++)
        {
            if (m_tmpCardList[i].num == 15) continue;
            con = true;
            for (int j = 0; j < same_List.Count; j++)
            {
                for (int k = 0; k < same_List[j].Count; k++)
                {
                    if (same_List[j][k].num == m_tmpCardList[i].num && same_List[j][k].color == m_tmpCardList[i].color)
                    {
                        con = false;
                    }
                }
            }
            if (con)
            {
                List<Card> tt11 = new List<Card>();
                tt11.Add(m_tmpCardList[i]);
                same_List.Add(tt11);
            }
        }

        // Add Same cards
        int count = same_List.Count;
        for (int i = 0; i < count; i++)
        {

            Card parent = new Card();
            parent = m_tmpCardList[i];

            Queue<Card> q = new Queue<Card>();
            q.Enqueue(parent);

            List<Card> m_tmp_same_List = new List<Card>();

            while (q.Count > 0)
            {
                Card current = q.Dequeue();
                if (current == null)
                    continue;

                foreach (var child in current.Children_Set(m_tmpCardList))
                {
                    var card = new Card();
                    card = child;
                    q.Enqueue(card);
                }

                m_tmp_same_List.Add(current);
            }

            foreach (var card in m_tmp_same_List)
            {
                if (card.parent == null) continue;
                List<Card> tmpList = new List<Card>();

                tmpList.Add(OnlyCardInfo(card));
                var tmpCard = card.parent;

                while (tmpCard != null)
                {
                    tmpList.Add(OnlyCardInfo(tmpCard));
                    tmpCard = tmpCard.parent;
                }
                same_List.Add(tmpList);
            }
        }

        // Add joker
        var joker_list = m_tmpCardList.Where(x => x.num == 15).ToList();

        if (m_tmpCardList.Count(x => x.num == 15) > 0)
        {
            count = same_List.Count;
            for (int i = 0; i < count; i++)
            {
                var list = m_tmpCardList.Where(x => x.num == 15).ToList();
                List<Card> tmpList = new List<Card>();

                foreach (var card in list)
                {
                    Card new_card = new Card(15, same_List[i][0].color);
                    new_card.MyCardId = card.MyCardId;
                    new_card.virtual_num = same_List[i][0].num;

                    tmpList.Add(new_card);

                    List<Card> newList = new List<Card>();
                    newList.AddRange(same_List[i]);

                    newList.AddRange(tmpList.ToList());
                    same_List.Add(newList);
                }
            }
        }

        // // Remove only one carded list
        // same_List = same_List.Where(x => x.Count > 1).ToList();

        return same_List;
    }

    private static Card OnlyCardInfo(Card card)
    {
        Card res = new Card();
        res.MyCardId = card.MyCardId;
        res.num = card.num;
        res.color = card.color;
        res.virtual_num = card.virtual_num;
        return res;
    }

    public static List<List<Card>> GetAvailableCards_Set(List<LamiMyCard> myCurrent)
    {
        List<Card> m_tmpCardList = new List<Card>();
        m_tmpCardList.Clear();

        for (int i = 0; i < myCurrent.Count; i++)
        {
            Card card = new Card();
            card.color = myCurrent[i].color;
            card.num = myCurrent[i].num;
            card.MyCardId = i;
            card.virtual_num = card.num;
            m_tmpCardList.Add(card);
        }

        return GetAvailableCards_Set_With_Card(m_tmpCardList);

    }

    public static List<List<Card>> GetAvailableCards_Flush_With_Card(List<Card> m_tmpCardList)
    {
        List<List<List<Card>>> continue_List = new List<List<List<Card>>>();

        List<List<Card>> t00 = new List<List<Card>>();
        continue_List.Add(t00);

        bool con = false;

        // Add first card
        for (int i = 0; i < m_tmpCardList.Count; i++)
        {
            if (m_tmpCardList[i].num == 15) continue;
            con = true;
            for (int j = 0; j < continue_List[0].Count; j++)
            {
                for (int k = 0; k < continue_List[0][j].Count; k++)
                {
                    if (continue_List[0][j][k].num == m_tmpCardList[i].num && continue_List[0][j][k].color == m_tmpCardList[i].color)
                    {
                        con = false;
                    }
                }
            }
            if (con)
            {
                List<Card> tt11 = new List<Card>();
                tt11.Add(m_tmpCardList[i]);
                continue_List[0].Add(tt11);

                if (m_tmpCardList[i].num == 1)
                {
                    List<Card> A_tt11 = new List<Card>();
                    Card card = new Card();
                    card.num = m_tmpCardList[i].num;
                    card.color = m_tmpCardList[i].color;
                    card.MyCardId = m_tmpCardList[i].MyCardId;
                    card.virtual_num = 14;

                    A_tt11.Add(m_tmpCardList[i]);
                    continue_List[0].Add(A_tt11);
                }
            }
        }

        // Inserting Joker
        int count = continue_List[0].Count;
        for (int j = 0; j < count; j++)
        {
            for (int i = 0; i < m_tmpCardList.Where(x => x.num == 15).Count(); i++)
            {
                int firstColor = continue_List[0][j][continue_List[0][j].Count - 1].color;
                int firstValue = continue_List[0][j][continue_List[0][j].Count - 1].virtual_num;

                if (firstValue != 1)
                {
                    firstValue = firstValue - (i + 1);

                    if (firstValue > 0)
                    {
                        List<Card> tt11 = continue_List[0][j].ToList();
                        List<Card> insert_List = new List<Card>();
                        for (int k = 0; k < i + 1; k++)
                        {
                            var j_card = new Card();
                            j_card.MyCardId = m_tmpCardList.Where(x => x.num == 15).ToList()[k].MyCardId;
                            j_card.num = 15;
                            j_card.color = firstColor;
                            j_card.virtual_num = firstValue + k;

                            insert_List.Add(j_card);
                        }
                        insert_List.AddRange(tt11);
                        continue_List[0].Add(insert_List);
                    }
                }
                else
                {
                    firstValue = 14 - (i + 1);

                    if (firstValue > 0)
                    {
                        List<Card> tt11 = continue_List[0][j].ToList();
                        Card card = new Card();
                        card.MyCardId = tt11[0].MyCardId;
                        card.color = tt11[0].color;
                        card.num = tt11[0].num;

                        card.virtual_num = -1;
                        List<Card> insert_List = new List<Card>();
                        for (int k = 0; k < i + 1; k++)
                        {
                            var j_card = new Card();
                            j_card.MyCardId = m_tmpCardList.Where(x => x.num == 15).ToList()[k].MyCardId;
                            j_card.num = 15;
                            j_card.color = firstColor;

                            j_card.virtual_num = firstValue + k;
                            j_card.color = firstColor;
                            insert_List.Add(j_card);
                        }
                        insert_List.Add(card);
                        continue_List[0].Add(insert_List);
                    }
                }
            }
        }



        bool canContinue = true;
        int level = 0;
        while (canContinue)
        {
            canContinue = false;
            level++;
            continue_List.Add(new List<List<Card>>());
            count = continue_List[level - 1].Count;

            for (int i = 0; i < count; i++)
            {
                // Add continuous card(except joker)
                if (continue_List[level - 1][i][continue_List[level - 1][i].Count - 1].virtual_num == -1) continue; // if Joker is used as Q, K, A(joker), continue;

                List<int> myIdList = new List<int>();
                foreach (var ttt in continue_List[level - 1][i])
                {
                    myIdList.Add(ttt.MyCardId);
                }

                for (int j = 0; j < m_tmpCardList.Count; j++)
                {
                    if (myIdList.Contains(m_tmpCardList[j].MyCardId)) continue;
                    if (m_tmpCardList[j].num == 15) continue;

                    if ((continue_List[level - 1][i][continue_List[level - 1][i].Count - 1].virtual_num + 1 == m_tmpCardList[j].num ||  // if continuous
                           (continue_List[level - 1][i][continue_List[level - 1][i].Count - 1].virtual_num + 1 == 14 && m_tmpCardList[j].num == 1)) // if A
                        && continue_List[level - 1][i][0].color == m_tmpCardList[j].color)  // if same color
                    {
                        List<Card> tmp2 = new List<Card>();
                        tmp2.AddRange(continue_List[level - 1][i].ToList());
                        var newCard = new Card();
                        newCard.num = m_tmpCardList[j].num;
                        newCard.color = m_tmpCardList[j].color;
                        newCard.MyCardId = m_tmpCardList[j].MyCardId;
                        newCard.virtual_num = m_tmpCardList[j].num;


                        if (tmp2[tmp2.Count - 1].virtual_num + 1 == 14)
                        {
                            newCard.virtual_num = -1;
                        }

                        tmp2.Add(newCard);

                        continue_List[level].Add(tmp2);
                        canContinue = true;
                        break;
                    }
                }


                // Add joker
                if (continue_List[level - 1][i].Count(x => x.num == 15) < m_tmpCardList.Count(x => x.num == 15))
                {
                    List<Card> tmp2 = new List<Card>();
                    tmp2.AddRange(continue_List[level - 1][i].ToList());

                    Card sel_joker = new Card();
                    sel_joker.color = tmp2[0].color;
                    bool isSelectJoker = false;
                    for (int tt = 0; tt < m_tmpCardList.Count && !isSelectJoker; tt++)
                    {
                        if (myIdList.Contains(m_tmpCardList[tt].MyCardId)) continue;
                        if (m_tmpCardList[tt].num != 15) continue;
                        for (int kk = 0; kk < tmp2.Count && !isSelectJoker; kk++)
                        {
                            if (tmp2[kk].MyCardId != m_tmpCardList[tt].MyCardId)
                            {

                                sel_joker.num = m_tmpCardList[tt].num;
                                sel_joker.MyCardId = m_tmpCardList[tt].MyCardId;
                                isSelectJoker = true;
                            }
                        }
                    }

                    if (isSelectJoker)
                    {
                        sel_joker.virtual_num = tmp2[tmp2.Count - 1].virtual_num + 1;
                        if (sel_joker.virtual_num == 14) sel_joker.virtual_num = -1;
                        tmp2.Add(sel_joker);
                        continue_List[level].Add(tmp2);

                        canContinue = true;
                    }
                }
            }
        }


        List<List<Card>> resList = new List<List<Card>>();

        for (int i = continue_List.Count - 1; i >= 0; i--)
        {
            for (int j = continue_List[i].Count - 1; j >= 0; j--)
            {
                var tmp = continue_List[i][j].ToList();
                resList.Add(tmp);
            }
        }
        return resList;
    }

    public static List<List<Card>> GetAvailableCards_Flush(List<LamiMyCard> myCurrent)
    {
        List<Card> m_tmpCardList = new List<Card>();

        m_tmpCardList.Clear();

        for (int i = 0; i < myCurrent.Count; i++)
        {
            Card card = new Card();
            card.color = myCurrent[i].color;
            card.num = myCurrent[i].num;
            card.MyCardId = i;
            card.virtual_num = card.num;
            if (card.num == 1)
                card.virtual_num = 1;
            m_tmpCardList.Add(card);
        }

        return GetAvailableCards_Flush_With_Card(m_tmpCardList);
    }

    public void OnSelectedCard() //If one card is selected, this function should be called.
    {
        avail_lineList.Clear();

        // Get Selected Cards

        for (int i = 0; i < m_cardList.Count; i++)
        {
            if (m_cardList[i].isSelected)
            {
                sel_cards.Add(m_cardList[i]);
            }
        }

        Sort();

        // Get Available Line Number
        // for (int i = 0; i < parent.panMgr.m_cardLineList.Count; i++)
        // {
        //     if (GetAvilableLineNum(sel_cards, panMgr.m_cardLineList[i])) // condition)
        //     {
        //         avail_lineList.Add(i);
        //     }
        // }
    }

    public void OnPlayButtonClick()
    {
        if (avail_lineList.Count > 0)
        {
            cardLineNumber = avail_lineList[0];
            DealCards();
        }
    }

    public void OnLineClick(int no)
    {
        if (avail_lineList.Contains(no))
        {
            cardLineNumber = no;
            DealCards();
        }
    }

    public void OnTipButtonClick()
    {

    }


    private List<Card> ArrangeWithColor() // Get Arranged CardList
    {
        List<Card> resList = new List<Card>();

        // for (int i = 0; i < m_cardList.Count; i++)
        // {
        //     Card item = new Card();
        //     item.MyCardId = i;
        //     item.number = m_cardList[i].num;
        //     item.color = m_cardList[i].color;
        //     resList.Add(item);
        // }

        // Card[] array = resList.ToArray();
        // resList.Clear();
        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }

        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].color) > Math.Abs(array[j].color))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }


        // for (int i = 0; i < array.Length; i++)
        // {
        //     resList.Add(array[i]);
        // }

        return resList;
    }

    private List<Card> ArrangeWithNumber()
    {
        List<Card> resList = new List<Card>();

        // for (int i = 0; i < m_cardList.Count; i++)
        // {
        //     Card item = new Card();
        //     item.MyCardId = i;
        //     item.number = m_cardList[i].num;
        //     item.color = m_cardList[i].color;
        //     resList.Add(item);
        // }

        // Card[] array = resList.ToArray();
        // resList.Clear();
        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }

        // for (int i = 0; i < array.Length; i++)
        // {
        //     resList.Add(array[i]);
        // }

        return resList;
    }
    public void OnArrangeButtonClick() // Arrange Button Is Clicked
    {
        IsSortClicked = (!IsSortClicked);

        if (IsSortClicked) // If Arrange Button Clicked, Arrange with Number Size 
        {
            ArrangeWithNumber();
        }
        else // If Arrange Button Not Clicked, Arrange with Color Size
        {
            ArrangeWithColor();
        }
    }

    public void DealCards()
    {
        string cards = "";
        for (int i = 0; i < m_cardList.Count; i++)
        {
            if (m_cardList[i].isSelected)
            {
                cards = string.Format("{0}:{1}", m_cardList[i].color, m_cardList[i].num);
                m_cardList.Remove(m_cardList[i]);
            }
        }

        // selected card clear
        sel_cards.Clear();

        string card_data = cardLineNumber + "-" + cards;
        cardLineNumber = -1;
        // Send Message
    }

    // receive my status(init, thinking, ready, giveup, burnt, game), Other Action(buttons, show)
    public void SetMyStatus(int status)
    {
        switch (status)
        {
            case 0: // init
                break;
            case 1: // thinking
                break;
            case 2: // ready
                break;
            case 3: // give up
                break;
            case 4: // burnt
                break;
            case 5: // game
                break;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
