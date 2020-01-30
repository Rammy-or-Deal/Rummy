using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SeatMgr : MonoBehaviour
{
    public List<UserSeat> m_playerList;
    [HideInInspector] public Dictionary<int, int> seatNumList;

    public UserSeat GetUserSeatFromList(int actorNum)
    {
        UserSeat seat;
        try
        {
            seat= m_playerList.First(x => x.m_playerInfo.m_actorNumber == actorNum);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            LogPlayerList();
            throw;
        }
        return seat;
    }

    public void LogPlayerList()
    {
        Debug.LogError("m_playerList :"+m_playerList.Count);
        for (int i=0;i<m_playerList.Count;i++)
            Debug.LogError(m_playerList[i].m_playerInfo.m_actorNumber);
    }

    public virtual void OnSeatStringUpdate()
    {
        GameMgr.Inst.Log("SeatMgr->OnSeatStringUpdate() is called.", LogLevel.RoomLog);

        GameMgr.Inst.Log("seatString=" + (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING], LogLevel.RoomLog);
        GameMgr.Inst.Log("playerString=" + (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING], LogLevel.RoomLog);

        // Update seatNumList variable
        var seatInfo = Update_seatNumList((string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);

        // Update User Seat
        PlayerInfoContainer pList = new PlayerInfoContainer();
        string infoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        pList.m_playerInfoListString = infoString;

        GameMgr.Inst.Log("seatList=" + seatInfo.seatString, LogLevel.RoomLog);
        GameMgr.Inst.Log("playerNumList=" + pList.m_playerInfoListString, LogLevel.RoomLog);
        foreach (var player in m_playerList)
        {
            player.isSeat = false;
        }

        foreach (var seat in seatInfo.seatList)
        {
            try
            {
                GameMgr.Inst.Log("now Seat:=" + seat.oneSeatString, LogLevel.RoomLog);
                var user = pList.m_playerList.First(x => x.m_userName == seat.m_userName);
                UpdateUserSeat(seat, user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Code for Fortune
        if (!PhotonNetwork.IsMasterClient) return;
        //if(m_playerList.Count(x=>x.isSeat == false) > 0) return;
        if (pList.m_playerList.Count >= constantContainer.FortuneMinimumPlayer && GameMgr.Inst.m_gameStatus == enumGameStatus.InGamePlay)
            StartFortuneGame();
        #endregion
    }

    public virtual void StartFortuneGame()
    {

    }

    private void UpdateUserSeat(SeatInfo.OneSeat seat, PlayerInfo playerInfo)
    {
        var seatNo = GetUserSeatPos(seat.m_seatNo);
        GameMgr.Inst.Log("NewUserEnteredRoom. ActorNumber=" + seat.m_actorNumber + ", seatNo=" + seatNo, LogLevel.RoomLog);
        m_playerList[seatNo].SetPlayerInfo(playerInfo);
    }

    public virtual void OnPlayerLeftRoom(int actorNumber)
    {
        GameMgr.Inst.Log(actorNumber + " is left. current gameStatus=" + GameMgr.Inst.m_gameStatus);
        try
        {
            if (GameMgr.Inst.m_gameStatus == enumGameStatus.OnGameStarted)
            {
                var player =GetUserSeatFromList(actorNumber);
                player.status = (int)enumPlayerStatus.Rummy_GiveUp;
            }
            else
            {
                var pList = new PlayerInfoContainer();
                pList.GetInfoContainerFromPhoton();
                var p = pList.m_playerList.First(x => x.m_actorNumber == actorNumber);
                pList.m_playerList.Remove(p);

            }
        }
        catch (Exception err)
        {

        }
    }

    private SeatInfo Update_seatNumList(string v)
    {
        if (seatNumList == null) seatNumList = new Dictionary<int, int>();
        GameMgr.Inst.Log("SeatMgr->Update_seatNumList() is called. param:= " + v, LogLevel.RoomLog);        
        SeatInfo seatInfo = new SeatInfo();
        seatInfo.seatString = v;
        seatNumList.Clear();
        foreach (var seat in seatInfo.seatList)
        {
            seatNumList.Add(seat.m_actorNumber, seat.m_seatNo);
        }
        // GameMgr.Inst.Log("SeatMgr->Update_seatNumList() Result:" + string.Join(", ", seatInfo.seatList.Select(x => x.oneSeatString)), enumLogLevel.RoomLog);
        return seatInfo;
    }

    public UserSeat GetUserSeat(int actorNumber)
    {
        int pos = GetUserSeatPos(seatNumList[actorNumber]);
        try
        {
            return m_playerList[pos];    
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            LogPlayerList();
            Debug.LogError(pos);
            throw;
        }
    }

    private int GetUserSeatPos(int id)
    {
        int seatPos;
        if (id == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = 0;
        }
        else if (id > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = id - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
        }
        else
        {
            seatPos = GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + id;
        }
        return seatPos;
    }

    public virtual void AddGold(int playerId, int score)
    {
        GameMgr.Inst.Log("AddGold, seatString=" + (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);
        GameMgr.Inst.Log("AddGold, actor=" + playerId + ", gold=" + score);
        // Update User Seat
        PlayerInfoContainer pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();

        if (pList.m_playerList.Count(x => x.m_actorNumber == playerId) > 0)
        {
            foreach (var player in pList.m_playerList.Where(x => x.m_actorNumber == playerId))
            {
                player.m_coinValue += score;
            }
            GameMgr.Inst.Log("PlayerListString(after result)=" + pList.m_playerInfoListString);


            Hashtable turnProps = new Hashtable
            {
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
        }
        else
        {

        }
    }
}