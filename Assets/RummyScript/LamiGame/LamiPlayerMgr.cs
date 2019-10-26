using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class LamiPlayerMgr : MonoBehaviour
{
    public string totalCardString = "";
    public string totalRemainString = "";
    public string totalPayString = "";
    public int nowTurn = -1;
    public LamiUserSeat[] m_playerList;

    public List<LamiGameBot> m_botList = new List<LamiGameBot>();
    public Dictionary<int, int> seatNumList;
    string master_seatString = "";

    public static LamiPlayerMgr Inst;
    private void Awake()
    {
        if (!Inst)
        {
            Inst = this;
            seatNumList = new Dictionary<int, int>();
        }
    }

    #region Room Management functions
    internal void OnUserEnteredRoom_M()
    {
        string newPlayerInfo = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NEW_PLAYER_INFO];
        LogMgr.Inst.Log("New Player Info: " + newPlayerInfo, (int)LogLevels.MasterLog);

        int ActorNumber = int.Parse(newPlayerInfo.Split(':')[0]);
        string seatString = "";
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(Common.SEAT_STRING))    // If it's new room
        {
            seatString = ActorNumber + ":" + 0;
            Debug.Log("User Created the room.");
        }
        else    // If it's remained room
        {
            Debug.Log("OnJoinedRoom After");
            seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
            var userSeatList = seatString.Split(',');
            for (int i = 0; i < userSeatList.Length; i++)
            {
                if (int.Parse(userSeatList[i].Split(':')[0]) == ActorNumber)
                {
                    break;
                }
            }

            if (userSeatList.Length < 4) // If there is seat that the user can play.
            {
                int[] seatNoList = new int[userSeatList.Length];
                for (int i = 0; i < userSeatList.Length; i++)
                    seatNoList[i] = int.Parse(userSeatList[i].Split(':')[1]);

                int seatNo = -1;
                for (int i = 0; i <= 3; i++)
                {
                    if (!seatNoList.Contains(i))
                    {
                        seatNo = i;
                        break;
                    }
                }
                seatString += "," + ActorNumber + ":" + seatNo;
            }
            else    // if there's no seat, remove bot
            {
                int seatNo = -1;
                int removedBot = 0;
                for (int i = 0; i < userSeatList.Length; i++)
                {
                    int tmpActor = int.Parse(userSeatList[i].Split(':')[0]);
                    int tmpSeat = int.Parse(userSeatList[i].Split(':')[1]);

                    if (tmpActor < 0)    // if actornumber < 0, this is a bot. 
                    {
                        seatNo = tmpSeat;
                        removedBot = tmpActor;
                        tmpActor = ActorNumber;
                        userSeatList[i] = tmpActor + ":" + tmpSeat;
                        break;
                    }
                }

                if (removedBot != 0)    // If the bot is removed, send bot remove messages
                {
                    Debug.Log("Bot removed: " + removedBot);
                    Hashtable botChangeString = new Hashtable
                        {
                            {Common.LAMI_MESSAGE, (int)LamiMessages.OnRemovedBot},
                            {Common.REMOVED_BOT_ID, removedBot}
                        };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(botChangeString);
                }

                seatString = "";
                for (int i = 0; i < userSeatList.Length; i++)
                {
                    seatString += userSeatList[i] + ",";
                }
                seatString = seatString.Trim(',');
            }
        }

        // Send RoomUpdate Messages to all players.
        Debug.Log("SeatString updated: " + seatString);
        Hashtable turnProps = new Hashtable
                {
                    {Common.LAMI_MESSAGE, (int)LamiMessages.OnRoomSeatUpdate},
                    {Common.SEAT_STRING, seatString},
                };
        master_seatString = seatString;
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    internal void OnCardDistributed()
    {
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

    internal void OnGameFinished()
    {
        foreach (var seat in m_playerList)
        {
            try
            {
                seat.cardList.Clear();
            }
            catch { }
            seat.cardListUpdate(totalCardString, totalPayString);
        }
        LamiGameUIManager.Inst.finishDlg.gameObject.SetActive(true);
    }

    internal void OnPlayerStatusChanged()
    {
        string seat_string = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        LogMgr.Inst.Log("Seat String:=" + seat_string);
        string tmpStr = "";
        for (int i = 0; i < m_playerList.Length; i++)
        {
            tmpStr += m_playerList[i].id + "(" + m_playerList[i].status + "), ";
        }
        LogMgr.Inst.Log("Current String:=" + tmpStr);



        int player_id = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        int status = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_STATUS];
        LogMgr.Inst.Log("Current Request String:=" + player_id + ", " + status);

        for (int i = 0; i < m_playerList.Length; i++)
        {
            if (m_playerList[i].id == player_id)
            {
                m_playerList[i].status = status;
                m_playerList[i].Show();
            }
        }

        for (int i = 0; i < m_botList.Count; i++)
        {
            if (m_playerList[i].id == player_id && m_playerList[i].isBot)
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
        //     {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
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

        for (int i = 0; i < m_playerList.Length; i++)
        {
            if (m_playerList[i].id == actor)
            {
                m_playerList[i].mCardNum.text = remained + "";

                string[] str = cardString.Split(':');
                m_playerList[i].OnUserDealt(str[1]);
                nowTurn = i;
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
        var numList = cardString.Split(':')[1].Split(',').Select(Int32.Parse).ToArray();
        var colList = cardString.Split(':')[2].Split(',').Select(Int32.Parse).ToArray();
        for(int i = 0; i < numList.Length; i++)
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
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnGameFinished},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            //LamiGameUIManager.Inst.finishDlg.gameObject.SetActive(true);
        }
        else
        {
            Hashtable props = new Hashtable{
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserTurnChanged},
                {Common.NOW_TURN, nowTurn}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
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

        int actor = m_playerList[turn].id;
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
        for (int i = 0; i < m_playerList.Length; i++)
        {
            if (m_playerList[i].id == actor)
            {
                m_playerList[i].mClock.SetActive(true);
                LamiCountdownTimer.Inst.turnTime = m_playerList[i].mClockTime;
            }
            else
            {
                m_playerList[i].mClock.SetActive(false);
            }
        }
        LamiCountdownTimer.Inst.StartTurnTimer();
        #endregion

        if (!PhotonNetwork.IsMasterClient) return;   // If this isn't master, return.

        if (actor < 0 && turn >= 0)
        {
            for (int i = 0; i < m_botList.Count; i++)
                if (m_botList[i].id == actor)
                    m_botList[i].SetMyTurn();

            // turn = (turn + 1) % 4;
            // Hashtable props = new Hashtable{
            //     {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserTurnChanged},
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
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserTurnChanged},
            {Common.NOW_TURN, turn}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        LogMgr.Inst.Log("First Turn is determined. - " + turn, (int)LogLevels.RoomLog2);
    }

    internal void OnUserLeave_M(int actorNumber)
    {
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
                    {Common.LAMI_MESSAGE, (int)LamiMessages.OnRoomSeatUpdate},
                    {Common.SEAT_STRING, seatString},
                };
        master_seatString = seatString;
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    internal void OnJoinSuccess()
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(false);
        //PhotonNetwork.LoadLevel("3_PlayLami");

        LamiMe.Inst.PublishMe();
    }
    internal void OnRoomSeatUpdate()
    {
        //OnBotInfoChanged();

        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];

        LogMgr.Inst.Log("OnRoomSeatUpdate: " + seatString, (int)LogLevels.PlayerLog1);
        // Prepare seatNumList      - this is to remove unneeded for statement
        //if (seatString != "")
        seatNumList.Clear();
        var tmp = seatString.Split(',');
        for (int i = 0; i < tmp.Length; i++)
        {
            seatNumList.Add(int.Parse(tmp[i].Split(':')[0]), int.Parse(tmp[i].Split(':')[1]));
        }
        LogMgr.Inst.Log("seatNumList: " + seatNumList.ToStringFull(), (int)LogLevels.PlayerLog1);

        // Get seat no from seat string
        for (int i = 0; i < m_playerList.Length; i++)
            m_playerList[i].canShow = false;

        for (int i = 0; i < tmp.Length; i++)
        {
            int tmpActor = int.Parse(tmp[i].Split(':')[0]);
            int tmpSeat = int.Parse(tmp[i].Split(':')[1]);

            m_playerList[GetUserSeat(tmpSeat)].SetProperty(tmpActor);
        }

        // Show/Hide players;
        for (int i = 0; i < m_playerList.Length; i++)
            m_playerList[i].Show();
    }

    #endregion
    internal void OnStartGame()
    {
        for (int i = 0; i < m_playerList.Length; i++)
        {
            m_playerList[i].status = (int)LamiPlayerStatus.Init;
            m_playerList[i].Show();
        }
    }
    internal void OnUserReady(int actornumber)
    {
        LogMgr.Inst.Log(actornumber + " Clicked Ready button.", (int)LogLevels.PlayerLog1);

        for (int i = 0; i < m_playerList.Length; i++)
        {
            if (m_playerList[i].id == actornumber)
            {
                m_playerList[i].status = (int)LamiPlayerStatus.Ready;
                m_playerList[i].canShow = true;
                m_playerList[i].Show();
                break;
            }
        }


        //Check if all players are ready

        int readyUsers = 0;
        for (int i = 0; i < m_playerList.Length; i++)
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
                        {Common.LAMI_MESSAGE, (int)LamiMessages.OnStartGame}
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
    internal void OnBotInfoChanged()
    {
        string botListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BOT_LIST_STRING];
        LogMgr.Inst.Log(botListString, (int)LogLevels.BotLog);
        var tmp = botListString.Split(',');
        m_botList.Clear();
        if (botListString == "") return;
        for (int i = 0; i < tmp.Length; i++)
        {
            LamiGameBot bot = new LamiGameBot(this);
            bot.SetBotInfo(tmp[i]);
            m_botList.Add(bot);

            LogMgr.Inst.Log("Bot Created : " + bot.getBotString(), (int)LogLevels.BotLog);
        }
    }
    internal void OnRemovedBot()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int removedBotId = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.REMOVED_BOT_ID];
            m_botList = m_botList.Where(x => x.id != removedBotId).ToList();
            SendBotListString();
        }
    }
    void SendBotListString()
    {
        string botString = "";
        for (int i = 0; i < m_botList.Count; i++)
        {
            botString += m_botList[i].getBotString() + ",";
        }
        botString = botString.Trim(',');

        Hashtable botChangeString = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnBotInfoChanged},
                {Common.BOT_LIST_STRING, botString}
            };
        PhotonNetwork.CurrentRoom.SetCustomProperties(botChangeString);
    }

    public void MakeOneBot()
    {
        // Check if there's empty seat
        string seatString = "";
        try
        {
            seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        }
        catch
        {

        }
        if (seatString == "") return;

        LogMgr.Inst.Log("Check Users before making bot. " + seatString, (int)LogLevels.BotLog);

        List<string> tmp = new List<string>();
        try{
        tmp.AddRange(seatString.Trim(',').Split(',').ToList());
        }catch{}
        // if all seats are not empty, return.
        if (tmp.Count >= 4 || tmp.Count == 0) return;

        // Create a bot
        LamiGameBot mBot = new LamiGameBot(this);
        mBot.Init();
        bool isIdSet = false;

        while (isIdSet == false)
        {
            isIdSet = true;
            for (int i = 0; i < m_botList.Count; i++)
            {
                if (mBot.id == m_botList[i].id)
                    isIdSet = false;
            }
            if (isIdSet == true)
            {
                mBot.id = -(UnityEngine.Random.Range(1000, 9999));
            }
        }

        m_botList.Add(mBot);
        LogMgr.Inst.Log("Bot Created : " + mBot.getBotString(), (int)LogLevels.BotLog);
        SendBotListString();

        mBot.PublishMe();
    }

    #endregion

    private int GetUserSeat(int seatNo_in_seatString)
    {

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
            userSeat = m_playerList[0];
        }
        else if (seatNumList[p.ActorNumber] > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            userSeat = m_playerList[seatNumList[p.ActorNumber] - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber]];
        }
        else
        {
            userSeat = m_playerList[4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNumList[p.ActorNumber]];
        }

        return userSeat;
    }
}
