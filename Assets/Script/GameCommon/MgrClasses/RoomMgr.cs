﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameRoomInfo> m_roomList;
    public GameRoomInfo m_currentRoom;
    private void Awake()
    {
        m_roomList = new List<GameRoomInfo>();
        m_currentRoom = new GameRoomInfo();
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        m_roomList.Clear();

        foreach (RoomInfo info in roomList)
        {
            try
            {
                GameRoomInfo room = new GameRoomInfo();

                var gameInfo = (string)info.CustomProperties[PhotonFields.RoomInfo];
                GameMgr.Inst.Log("Created Room: name=" + info.Name + ", Info=" + gameInfo, enumLogLevel.RoomLog);
                room.roomInfoString = gameInfo;
                m_roomList.Add(room);
            }
            catch (Exception err)
            {
                GameMgr.Inst.Log("Room information isn't correct. Error:= " + err.Message, enumLogLevel.RoomLog);
            }
        }
        GameMgr.Inst.Log("RoomListUpdate Called. RoomCount = " + roomList.Count, enumLogLevel.RoomLog);
        GameMgr.Inst.Log("Rooms:=" + string.Join("/", m_roomList.Select(x => x.m_maxPlayer)), enumLogLevel.RoomLog);
    }

    public void CreateOrJoinRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        if (m_roomList.Count(x => x.m_gameType == m_gameType) == 0)
            CreateRoom(m_gameType, m_gameTier);
        else
            JoinRoom(m_gameType, m_gameTier);
    }

    public void JoinRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        try
        {
            m_currentRoom.roomInfoString = m_roomList.Where(x => x.m_gameType == m_gameType && x.m_playerCount < x.m_maxPlayer).First().roomInfoString;
            PhotonNetwork.JoinRoom(m_currentRoom.m_roomName, null);
            GameMgr.Inst.Log("joined room: " + m_currentRoom.m_roomName, enumLogLevel.RoomLog);
        }
        catch (Exception err)
        {
            GameMgr.Inst.Log("JoinRoom Failed. So I should create a new room.(Error:" + err.Message + ")", enumLogLevel.RoomLog);
            CreateRoom(m_gameType, m_gameTier);
        }
    }

    public void CreateRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        GameRoomInfo room = new GameRoomInfo();
        room.m_gameType = m_gameType;
        room.m_gameTier = m_gameTier;
        room.m_roomName = string.Format("{0}_{1}_{2}", m_gameType, m_gameTier, DateTime.Now.ToString("MMddHHmmss"));
        room.m_gameFee = GetGameFeeOfGame(m_gameType, m_gameTier);
        room.m_maxPlayer = GetMaxPlayerOfGame(m_gameType, m_gameTier);
        room.m_playerCount = 0;
        room.m_additionalString = "";

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)room.m_maxPlayer;
        options.CustomRoomProperties = new Hashtable { { PhotonFields.RoomInfo, room.roomInfoString } };
        options.CustomRoomPropertiesForLobby = new string[1] { PhotonFields.RoomInfo };

        m_currentRoom.roomInfoString = room.roomInfoString;
        PhotonNetwork.CreateRoom(room.m_roomName, options, TypedLobby.Default);

        GameMgr.Inst.Log("room created " + room.m_roomName, enumLogLevel.RoomLog);

    }

    private int GetMaxPlayerOfGame(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        int maxPlayers = 0;
        switch (m_gameType)
        {
            case enumGameType.Lami:
                maxPlayers = 4;
                break;
            case enumGameType.Baccarat:
                maxPlayers = 8;
                break;
            case enumGameType.Fortune13:
                maxPlayers = 4;
                break;
        }
        return maxPlayers;
    }

    private int GetGameFeeOfGame(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        int gameFee = 0;
        switch (m_gameType)
        {
            case enumGameType.Lami:
                switch (m_gameTier)
                {
                    case enumGameTier.LamiNewbie: gameFee = 50; break;
                    case enumGameTier.LamiBeginner: gameFee = 100; break;
                    case enumGameTier.LamiVeteran: gameFee = 200; break;
                    case enumGameTier.LamiIntermediate: gameFee = 300; break;
                    case enumGameTier.LamiAdvanced: gameFee = 500; break;
                    case enumGameTier.LamiMaster: gameFee = 1000; break;
                    default: gameFee = 0; break;
                }
                break;
            case enumGameType.Baccarat:
                gameFee = 0;
                break;
            case enumGameType.Fortune13:
                switch (m_gameTier)
                {
                    case enumGameTier.FortuneNewbie: gameFee = 50; break;
                    case enumGameTier.FortuneBeginner: gameFee = 100; break;
                    case enumGameTier.FortuneVeteran: gameFee = 200; break;
                    case enumGameTier.FortuneIntermediate: gameFee = 300; break;
                    case enumGameTier.FortuneAdvanced: gameFee = 500; break;
                    case enumGameTier.FortuneMaster: gameFee = 1000; break;
                    default: gameFee = 0; break;
                }
                break;
            default:
                gameFee = 0;
                break;
        }
        return gameFee;
    }

    internal void OnJoinedRoom()
    {
        GameMgr.Inst.Log("Joined Room.");
        try
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(false);
        }
        catch { }
        string sceneString = "";
        switch (GameMgr.Inst.m_gameType)
        {
            case enumGameType.Lami:
                sceneString = constantContainer.strScene3Lami;
                break;
            case enumGameType.Baccarat:
                sceneString = constantContainer.strScene3Bacccarat;
                break;
            case enumGameType.Fortune13:
                sceneString = constantContainer.strScene3Fortune;
                break;
            default:
                sceneString = "";
                break;
        }
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneString);
    }

    internal void OnLeftRoom()
    {
        GameMgr.Inst.Log(PhotonNetwork.NickName + "/me/Left Room");
        PhotonNetwork.LoadLevel(constantContainer.strLobby);
        m_roomList.Clear();

        GameMgr.Inst.InitStatus();
    }

    internal void OnUserEnteredRoom_onlyMaster()
    {
        PlayerInfo newPlayer = new PlayerInfo();
        PlayerInfoContainer pList = new PlayerInfoContainer();

        newPlayer.playerInfoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.NEW_PLAYER_INFO];

        GameMgr.Inst.Log("New Player Info: " + newPlayer.playerInfoString, enumLogLevel.RoomLog);

        int actorNumber = newPlayer.m_actorNumber;

        string seatString = "";
        SeatInfo seatInfo = new SeatInfo();

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PhotonFields.SEAT_STRING))
        {
            seatInfo.AddOneSeat(actorNumber, 0, newPlayer.m_userName);
            pList.m_playerList.Add(newPlayer);
            GameMgr.Inst.Log("I successfully created this room.", enumLogLevel.RoomLog);
        }
        else
        {
            GameMgr.Inst.Log("On JoinedRoom After", enumLogLevel.RoomLog);

            seatInfo.seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING];
            pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
            pList.m_playerList.Add(newPlayer);

            if (seatInfo.seatList.Count < m_currentRoom.m_maxPlayer) // If there is seat that the user can play.
            {
                for (int i = 0; i < m_currentRoom.m_maxPlayer; i++)
                {
                    if (seatInfo.seatList.Count(x => x.m_seatNo == i) == 0)
                    {
                        seatInfo.AddOneSeat(actorNumber, i, newPlayer.m_userName);
                        break;
                    }
                }
            }
            else
            {
                if (actorNumber < 0 || seatInfo.seatList.Count(x => x.m_actorNumber < 0) == 0) return;
                // if there's no seat, remove bot                
                var newSeat = seatInfo.seatList.Where(x => x.m_actorNumber < 0).First();
                pList.m_playerList.Remove(pList.m_playerList.Where(x => x.m_userName == newSeat.m_userName).First());
                newSeat.m_actorNumber = actorNumber;
            }
        }

        seatString = seatInfo.seatString;
        m_currentRoom.m_playerCount = seatInfo.seatList.Count();
        GameMgr.Inst.Log("current Room String:=" + m_currentRoom.roomInfoString, enumLogLevel.RoomLog);
        GameMgr.Inst.Log("current Room SeatString:=" + seatString, enumLogLevel.RoomLog);
        GameMgr.Inst.Log("current Room PlayerListString:=" + pList.m_playerInfoListString, enumLogLevel.RoomLog);

        Hashtable turnProps = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.SEAT_STRING, seatString},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
            {PhotonFields.RoomInfo, m_currentRoom.roomInfoString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }
}