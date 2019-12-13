using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class SeatMgr : MonoBehaviour
{
    public List<UserSeat> m_playerList;
    [HideInInspector] public Dictionary<int, int> seatNumList;

    public virtual void OnSeatStringUpdate()
    {
        GameMgr.Inst.Log("SeatMgr->OnSeatStringUpdate() is called.", enumLogLevel.RoomLog);

        GameMgr.Inst.Log("seatString=" + (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING], enumLogLevel.RoomLog);
        GameMgr.Inst.Log("playerString=" + (string)(string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING], enumLogLevel.RoomLog);

        // Update seatNumList variable
        var seatInfo = Update_seatNumList((string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);        

        // Update User Seat
        PlayerInfoContainer pList = new PlayerInfoContainer();
        string infoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        pList.m_playerInfoListString = infoString;

        GameMgr.Inst.Log("seatList=" + seatInfo.seatString, enumLogLevel.RoomLog);
        GameMgr.Inst.Log("playerNumList=" + pList.m_playerInfoListString, enumLogLevel.RoomLog);
        foreach (var player in m_playerList)
        {
            player.isSeat = false;
        }

        foreach (var seat in seatInfo.seatList)
        {
            GameMgr.Inst.Log("now Seat:=" + seat.oneSeatString, enumLogLevel.RoomLog);
            var user = pList.m_playerList.Where(x => x.m_userName == seat.m_userName).First();
            UpdateUserSeat(seat, user);
        }

        #region Code for Fortune
        if(!PhotonNetwork.IsMasterClient) return;
        //if(m_playerList.Count(x=>x.isSeat == false) > 0) return;
        if(m_playerList.Count >= constantContainer.FortuneMinimumPlayer)
            StartFortuneGame();
        #endregion
    }

    public virtual void StartFortuneGame()
    {
        
    }

    private void UpdateUserSeat(SeatInfo.OneSeat seat, PlayerInfo playerInfo)
    {
        var seatNo = GetUserSeatPos(seat.m_seatNo);
        GameMgr.Inst.Log("NewUserEnteredRoom. ActorNumber=" + seat.m_actorNumber + ", seatNo=" + seatNo, enumLogLevel.RoomLog);
        m_playerList[seatNo].SetPlayerInfo(playerInfo);
    }

    public virtual void OnPlayerLeftRoom(int actorNumber)
    {
        GameMgr.Inst.Log(actorNumber + " is left. current gameStatus=" + GameMgr.Inst.m_gameStatus);
        try
        {
            if (GameMgr.Inst.m_gameStatus == enumGameStatus.OnGameStarted)
            {
                var player = m_playerList.Where(x => x.m_playerInfo.m_actorNumber == actorNumber).First();
                player.status = (int)enumPlayerStatus.Rummy_GiveUp;
            }
            else
            {
                var pList = new PlayerInfoContainer();
                pList.GetInfoContainerFromPhoton();
                var p = pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First();
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
        //GameMgr.Inst.Log("SeatMgr->Update_seatNumList() is called. param:= " + v, enumLogLevel.RoomLog);
        seatNumList.Clear();
        SeatInfo seatInfo = new SeatInfo();
        seatInfo.seatString = v;

        foreach (var seat in seatInfo.seatList)
        {
            seatNumList.Add(seat.m_actorNumber, seat.m_seatNo);
        }
       // GameMgr.Inst.Log("SeatMgr->Update_seatNumList() Result:" + string.Join(", ", seatInfo.seatList.Select(x => x.oneSeatString)), enumLogLevel.RoomLog);
        return seatInfo;
    }

    public UserSeat GetUserSeat(int actorNumber)
    {
        return m_playerList[GetUserSeatPos(seatNumList[actorNumber])];
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
        var seatInfo = Update_seatNumList((string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);
        // Update User Seat
        var userListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        PlayerInfoContainer pList = new PlayerInfoContainer(userListString);

        foreach (var user in GameMgr.Inst.seatMgr.m_playerList.Where(x => x.isSeat == true && x.m_playerInfo.m_actorNumber == playerId))
        {
            user.m_playerInfo.m_coinValue += score;
        }

        Hashtable turnProps = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }
}