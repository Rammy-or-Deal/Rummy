﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class SeatMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public List<UserSeat> m_playerList;
    [HideInInspector] public Dictionary<int, int> seatNumList;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnSeatStringUpdate()
    {
        GameMgr.Inst.Log("SeatMgr->OnSeatStringUpdate() is called.", enumLogLevel.RoomLog);

        // Update seatNumList variable
        var seatInfo = Update_seatNumList((string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);
        GameMgr.Inst.Log("seatNumList=" + string.Join(", ", seatNumList), enumLogLevel.RoomLog);

        // Update User Seat
        var userListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        PlayerInfoContainer pList = new PlayerInfoContainer(userListString);
        foreach (var seat in seatInfo.seatList)
        {
            var user = pList.m_playerList.Where(x => x.m_userName == seat.m_userName).First();
            UpdateUserSeat(seat, user);
        }
    }


    private void UpdateUserSeat(SeatInfo.OneSeat seat, PlayerInfo playerInfo)
    {
        var seatNo = GetUserSeat(seat.m_seatNo);
        GameMgr.Inst.Log("NewUserEnteredRoom. ActorNumber=" + seat.m_actorNumber + ", seatNo=" + seatNo, enumLogLevel.RoomLog);
        m_playerList[seatNo].SetPlayerInfo(playerInfo);
    }

    private SeatInfo Update_seatNumList(string v)
    {
        if (seatNumList == null) seatNumList = new Dictionary<int, int>();
        GameMgr.Inst.Log("SeatMgr->Update_seatNumList() is called. param:= " + v, enumLogLevel.RoomLog);
        seatNumList.Clear();
        SeatInfo seatInfo = new SeatInfo();
        seatInfo.seatString = v;

        foreach (var seat in seatInfo.seatList)
        {
            seatNumList.Add(seat.m_actorNumber, seat.m_seatNo);
        }
        GameMgr.Inst.Log("SeatMgr->Update_seatNumList() Result:" + string.Join(", ", seatInfo.seatList.Select(x => x.oneSeatString)), enumLogLevel.RoomLog);
        return seatInfo;
    }

    private int GetUserSeat(int no)
    {
        int seatPos;
        if (no == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = 0;
        }
        else if (no > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = no - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
        }
        else
        {
            seatPos = GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + no;
        }

        return seatPos;
    }

    public virtual void AddGold(int playerId, int score)
    {
        var seatInfo = Update_seatNumList((string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING]);
        // Update User Seat
        var userListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        PlayerInfoContainer pList = new PlayerInfoContainer(userListString);

        foreach (var seat in seatInfo.seatList.Where(x=>x.m_actorNumber == playerId))
        {
            var user = pList.m_playerList.Where(x => x.m_userName == seat.m_userName).First();
            user.m_coinValue += score;
        }

        Hashtable turnProps = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }
}