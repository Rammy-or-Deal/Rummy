using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
public class LamiPlayerMgr : SeatMgr
{
    public string totalCardString = "";
    public string totalRemainString = "";
    public string totalPayString = "";
    public int nowTurn = -1;

    public List<LamiGameBot> m_botList = new List<LamiGameBot>();
    string master_seatString = "";

    public static LamiPlayerMgr Inst;
    private void Start()
    {
        if (!Inst)
        {
            Inst = this;
            GameMgr.Inst.seatMgr = this;
            seatNumList = new Dictionary<int, int>();
        }
    }

    #region Room Management functions

    public override void OnSeatStringUpdate()
    {
        base.OnSeatStringUpdate();
        if (!PhotonNetwork.IsMasterClient) return;

        GameMgr.Inst.Log("Check if all players are ready.", enumLogLevel.RummySeatMgrLog);
        // Update User Seat

        var userListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        PlayerInfoContainer pList = new PlayerInfoContainer(userListString);

        GameMgr.Inst.Log("userListString = " + userListString + ",  seatNumList=" + string.Join(",", seatNumList), enumLogLevel.RummySeatMgrLog);

        bool isAllReady = true;
        foreach (var seat in seatNumList)
        {

            var user = pList.m_playerList.Where(x => x.m_actorNumber == seat.Key).First();
            if (user.m_status != enumPlayerStatus.Rummy_Ready)
            {
                GameMgr.Inst.Log(user.m_userName + " isn't ready.", enumLogLevel.RummySeatMgrLog);
                isAllReady = false;
                break;
            }
            else
            {
                GameMgr.Inst.Log(user.m_userName + " is ready.", enumLogLevel.RummySeatMgrLog);
            }
        }
        Debug.Log(seatNumList.Count + " = " + GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer + "  / " + isAllReady);
        
        if (isAllReady && seatNumList.Count == GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer)
        {
            GameMgr.Inst.Log("All Users are ready.", enumLogLevel.RoomLog);

            LamiCardMgr.Inst.GenerateCard();

            /*
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnGameStarted}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            */
        }
    }    

    internal void OnCardDistributed()
    {
        CreateBotsFromPhoton();
        var cardListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.CARD_LIST_STRING];
        totalCardString = cardListString;
        totalRemainString = cardListString;
        totalPayString = "";

        LogMgr.Inst.Log("Card Distributed: " + cardListString, (int)LogLevels.PlayerLog2);

        var tmp = cardListString.Split('/');

        for (int i = 0; i < tmp.Length; i++)
        {
            int tmpActor = int.Parse(tmp[i].Split(':')[0]);
            if (tmpActor == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                LamiMe.Inst.SetMyCards(tmp[i]);
            }
            if (tmpActor < 0)
            {
                for (int j = 0; j < m_botList.Count; j++)
                {
                    if (m_botList[j].id == tmpActor)
                    {
                        m_botList[j].SetMyCards(tmp[i]);
                        break;
                    }
                }
            }
        }
    }

    internal void OffAutoPlayer()
    {
        int playerId = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        foreach (LamiUserSeat player in m_playerList)
        {
            if (player.m_playerInfo.m_actorNumber == playerId)
            {
                player.isAuto = false;
            }
            player.Show();
        }
    }

    internal void OnAutoPlayer()
    {
        int playerId = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        foreach (LamiUserSeat player in m_playerList)
        {
            if (player.m_playerInfo.m_actorNumber == playerId)
            {
                player.isAuto = true;
            }
            player.Show();
        }
    }

    internal void OnGameFinished()
    {
        foreach (LamiUserSeat seat in m_playerList)
        {
            try
            {
                seat.cardList.Clear();
            }
            catch { }
            seat.cardListUpdate(totalCardString, totalPayString);
            seat.calcScore();
        }
        ShowFinishDlg();
    }

    internal void OnPlayerStatusChanged()
    {
        string seat_string = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        LogMgr.Inst.Log("Seat String:=" + seat_string);
        string tmpStr = "";
        for (int i = 0; i < m_playerList.Count; i++)
        {
            tmpStr += ((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber + "(" + m_playerList[i].status + "), ";
        }
        LogMgr.Inst.Log("Current String:=" + tmpStr);



        int player_id = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        int status = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_STATUS];
        LogMgr.Inst.Log("Current Request String:=" + player_id + ", " + status);

        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber == player_id)
            {
                ((LamiUserSeat)m_playerList[i]).status = status;
                ((LamiUserSeat)m_playerList[i]).Show();
            }
        }

        for (int i = 0; i < m_botList.Count; i++)
        {
            if (((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber == player_id && ((LamiUserSeat)m_playerList[i]).isBot)
            {
                m_botList[i].status = status;
                //m_botList[i].PublishMe();
            }
        }

        if (!PhotonNetwork.IsMasterClient) return;
        TurnChange();
    }
    internal void OnDealCard()
    {
        //         Hashtable gameCards = new Hashtable
        // {   
        //     {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnDealCard},
        //     {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
        //     {Common.REMAIN_CARD_COUNT, remainCard},
        //     {Common.GAME_CARD, cardStr},
        //     {Common.GAME_CARD_PAN, 0},
        // };

        // Change players list
        int actor = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        int remained = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.REMAIN_CARD_COUNT];

        string cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.GAME_CARD];
        UpdateRemainCards(cardString);

        LogMgr.Inst.Log("User Dealt card -  actor=" + actor + ", remained=" + remained + ", nowTurn=" + nowTurn, (int)LogLevels.RoomLog2);

        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber == actor)
            {
                m_playerList[i].mCardNum.text = remained + "";

                string[] str = cardString.Split(':');
                ((LamiUserSeat)m_playerList[i]).OnUserDealt(str[1]);
                nowTurn = i;
            }
        }


        for (int i = 0; i < m_botList.Count; i++)
        {
            if (actor == m_botList[i].id)
            {
                m_botList[i].OnUserDealt(cardString);
            }
        }

        if (PhotonNetwork.IsMasterClient && nowTurn >= 0)
        {
            TurnChange();
        }
    }

    private void UpdateRemainCards(string cardString)
    {

        int actor = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        Debug.Log("CardString:=" + cardString + "   actor=" + actor);

        var numList = cardString.Split(':')[1].Split(',').Select(Int32.Parse).ToArray();
        var colList = cardString.Split(':')[2].Split(',').Select(Int32.Parse).ToArray();
        for (int i = 0; i < numList.Length; i++)
        {
            totalPayString += actor + ":" + numList[i] + ":" + colList[i] + "/";
        }


        //totalPayString = totalPayString.Trim('/');

        //Debug.Log(totalRemainString);
    }

    public void TurnChange()
    {
        int first = nowTurn;
        nowTurn = (nowTurn + 1) % 4;

        while ((m_playerList[GetUserSeat(nowTurn)].status == (int)LamiPlayerStatus.GiveUp ||
            m_playerList[GetUserSeat(nowTurn)].status == (int)LamiPlayerStatus.Burnt) && first != nowTurn)
        {
            nowTurn = (nowTurn + 1) % 4;
        }
        if (first == nowTurn &&
            (m_playerList[GetUserSeat(nowTurn)].status == (int)LamiPlayerStatus.GiveUp || m_playerList[GetUserSeat(nowTurn)].status == (int)LamiPlayerStatus.Burnt))
        {
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnGameFinished},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            //LamiGameUIManager.Inst.finishDlg.gameObject.SetActive(true);


            StartCoroutine(SendRestartEvent());
        }
        else
        {
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserTurnChanged},
                {Common.NOW_TURN, nowTurn}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    IEnumerator SendRestartEvent()
    {
        yield return new WaitForSeconds(3);
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnGameRestart},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    internal void OnUserTurnChanged()
    {
        int turn = -1;
        try
        {
            turn = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.NOW_TURN];
        }
        catch { }
        LogMgr.Inst.Log("UserTurnChanged: turn = " + turn, (int)LogLevels.RoomLog2);
        nowTurn = turn;
        if (turn < 0) return;
        turn = GetUserSeat(turn);

        int actor = ((LamiUserSeat)m_playerList[turn]).m_playerInfo.m_actorNumber;
        LogMgr.Inst.Log("UserTurnChanged: Changed turn=" + turn + "  , Actor = " + actor + "   /myID=" + PhotonNetwork.LocalPlayer.ActorNumber, (int)LogLevels.RoomLog2);

        if (actor == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            LamiMe.Inst.SetMyTurn(true);
        }
        else
        {
            LamiMe.Inst.SetMyTurn(false);
        }
        // if this player is bot


        #region showing timer
        LamiCountdownTimer.Inst.StopTurnTimer();
        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber == actor)
            {
                ((LamiUserSeat)m_playerList[i]).mClock.SetActive(true);
                LamiCountdownTimer.Inst.turnTime = m_playerList[i].mClockTime;
            }
            else
            {
                ((LamiUserSeat)m_playerList[i]).mClock.SetActive(false);
            }
        }
        LamiCountdownTimer.Inst.StartTurnTimer(actor == PhotonNetwork.LocalPlayer.ActorNumber);
        #endregion

        if (!PhotonNetwork.IsMasterClient) return;   // If this isn't master, return.

        if (actor < 0 && turn >= 0)
        {
            for (int i = 0; i < m_botList.Count; i++)
                if (m_botList[i].id == actor)
                    m_botList[i].SetMyTurn();

            // turn = (turn + 1) % 4;
            // Hashtable props = new Hashtable{
            //     {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserTurnChanged},
            //     {Common.NOW_TURN, turn}
            // };
            // PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    internal void OnUserReadyToStart_M()
    {
        bool AllReady = true;

        // Check if all players are ready.
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if ((int)p.CustomProperties[Common.PLAYER_STATUS] != (int)LamiPlayerStatus.ReadyToStart)
            {
                AllReady = false;
            }
        }
        if (AllReady != true) return;

        // If all players are ready, Set the turn
        int turn = UnityEngine.Random.Range(0, 4);
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserTurnChanged},
            {Common.NOW_TURN, turn}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        LogMgr.Inst.Log("First Turn is determined. - " + turn, (int)LogLevels.RoomLog2);
    }

    internal void OnUserLeave_M(int actorNumber)
    {
        /*
        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        var tmp = seatString.Split(',');
        seatString = "";

        for (int i = 0; i < tmp.Length; i++)
        {
            int tmpActor = int.Parse(tmp[i].Split(':')[0]);
            if (actorNumber != tmpActor)
            {
                seatString += tmp[i] + ",";
            }
        }
        seatString = seatString.Trim(',');
        // Send RoomUpdate Messages to all players.
        Debug.Log("SeatString updated: " + seatString);
        Hashtable turnProps = new Hashtable
                {
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnRoomSeatUpdate},
                    {Common.SEAT_STRING, seatString},
                };
        master_seatString = seatString;
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
        */
    }

    internal void OnRoomSeatUpdate()
    {
        //OnBotInfoChanged();

        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];

        //LogMgr.Inst.Log("OnRoomSeatUpdate: " + seatString, (int)LogLevels.PlayerLog1);
        //Debug.Log("OnRoomSeatUpdate: " + seatString);
        // Prepare seatNumList      - this is to remove unneeded for statement
        //if (seatString != "")
        seatNumList.Clear();
        var tmp = seatString.Split(',');
        for (int i = 0; i < tmp.Length; i++)
        {
            seatNumList.Add(int.Parse(tmp[i].Split(':')[0]), int.Parse(tmp[i].Split(':')[1]));
        }
        //LogMgr.Inst.Log("seatNumList: " + seatNumList.ToStringFull(), (int)LogLevels.PlayerLog1);
        //Debug.Log("seatNumList: " + seatNumList.ToStringFull());

        // Get seat no from seat string
        for (int i = 0; i < m_playerList.Count; i++)
            m_playerList[i].canShow = false;

        for (int i = 0; i < tmp.Length; i++)
        {
            int tmpActor = int.Parse(tmp[i].Split(':')[0]);
            int tmpSeat = int.Parse(tmp[i].Split(':')[1]);

            ((LamiUserSeat)m_playerList[GetUserSeat(tmpSeat)]).SetProperty(tmpActor);
        }

        // Show/Hide players;
        for (int i = 0; i < m_playerList.Count; i++)
            ((LamiUserSeat)m_playerList[i]).Show();
    }

    #endregion
    internal void OnStartGame()
    {
        for (int i = 0; i < m_playerList.Count; i++)
        {
            m_playerList[i].status = (int)LamiPlayerStatus.Init;
            ((LamiUserSeat)m_playerList[i]).Show();
        }
    }
    internal void OnUserReady(int actornumber)
    {
        LogMgr.Inst.Log(actornumber + " Clicked Ready button.", (int)LogLevels.PlayerLog1);

        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (((LamiUserSeat)m_playerList[i]).m_playerInfo.m_actorNumber == actornumber)
            {
                m_playerList[i].status = (int)LamiPlayerStatus.Ready;
                m_playerList[i].canShow = true;
                ((LamiUserSeat)m_playerList[i]).Show();
                break;
            }
        }


        //Check if all players are ready

        int readyUsers = 0;
        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (m_playerList[i].canShow && m_playerList[i].status == (int)LamiPlayerStatus.Ready)
            {
                readyUsers++;
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (readyUsers == 4) // if All users are ready and there are 4 users, send StartGame message
            {
                // Send Game Start Message
                Hashtable props = new Hashtable{
                        {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnStartGame}
                    };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                LogMgr.Inst.Log("All players are ready.", (int)LogLevels.RoomLog1);
                // Distribute all cards
                LamiCardMgr.Inst.GenerateCard();

                // Close Game room
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
        }
    }

    #region  Bot Section
    internal void CreateBotsFromPhoton()
    {
        PlayerInfoContainer pList = new PlayerInfoContainer();
        string playerListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];

        //LogMgr.Inst.Log(botListString, (int)LogLevels.BotLog);
        pList.m_playerInfoListString = playerListString;

        foreach(var player in m_playerList.Where(x=>x.m_playerInfo.m_actorNumber < 0))
        {
            LamiGameBot bot = new LamiGameBot();
            var p = pList.m_playerList.Where(x=>x.m_actorNumber == player.m_playerInfo.m_actorNumber).First();
            GameMgr.Inst.Log("bot info:=" + p.playerInfoString, enumLogLevel.BotLog);
            bot.SetBotInfo(p.playerInfoString);

            m_botList.Add(bot);
            LogMgr.Inst.Log("Bot Created : " + bot.getBotString(), (int)LogLevels.BotLog);
        }

    }

    public string getBotString()
    {
        string botString = "";
        for (int i = 0; i < m_botList.Count; i++)
        {
            botString += m_botList[i].getBotString() + ",";
        }
        botString = botString.Trim(',');
        return botString;
    }

    #endregion

    public int GetUserSeat(int seatNo_in_seatString)
    {
        Debug.Log("GetUserSeat, seatNo_in_seatString:=" + seatNo_in_seatString);
        int seatPos;
        if (seatNo_in_seatString == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = 0;
        }
        else if (seatNo_in_seatString > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = seatNo_in_seatString - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
        }
        else
        {
            seatPos = 4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNo_in_seatString;
        }

        return seatPos;
    }

    public LamiUserSeat GetUserSeat(Player p)
    {
        LamiUserSeat userSeat;

        Debug.Log("GetUserSeat; seat_id:" + PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID]);
        Debug.Log("GetUserSeat; seatNumList:" + seatNumList.ToStringFull());

        if (seatNumList[p.ActorNumber] == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            Debug.Log(p.NickName + "///" + p.ActorNumber);
            userSeat = ((LamiUserSeat)m_playerList[0]);
        }
        else if (seatNumList[p.ActorNumber] > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            userSeat = ((LamiUserSeat)m_playerList[seatNumList[p.ActorNumber] - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber]]);
        }
        else
        {
            userSeat = ((LamiUserSeat)m_playerList[4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNumList[p.ActorNumber]]);
        }

        return userSeat;
    }

    public void ShowFinishDlg()
    {
        LamiGameUIManager.Inst.finishDlg.SetData();

        StartCoroutine(WaitSecondds());
    }

    IEnumerator WaitSecondds()
    {
        yield return new WaitForSeconds(3);
        LamiGameUIManager.Inst.finishDlg.gameObject.SetActive(true);
    }

}
