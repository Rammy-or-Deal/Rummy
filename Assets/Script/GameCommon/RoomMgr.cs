using System;
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

    public RoomMgr()
    {
        m_roomList = new List<GameRoomInfo>();
    }

    internal void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        m_roomList.Clear();

        foreach (RoomInfo info in roomList)
        {
            GameRoomInfo room = new GameRoomInfo();
            try
            {
                var gameInfo = (string)info.CustomProperties[PhotonFields.RoomInfo];
                room.roomInfoString = gameInfo;
                m_roomList.Add(room);
            }
            catch
            {

            }
        }
        GameMgr.Inst.Log("RoomListUpdate Called. RoomCount = " + roomList.Count, enumLogLevel.RoomManagementLog);
    }

    internal void CreateOrJoinRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        if (m_roomList.Count(x => x.m_gameType == m_gameType) == 0)
            CreateRoom(m_gameType, m_gameTier);
        else
            JoinRoom(m_gameType, m_gameTier);
    }

    public void JoinRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        var room = m_roomList.Where(x => x.m_gameType == m_gameType && x.m_playerCount < x.m_maxPlayer).First();
        PhotonNetwork.JoinRoom(room.m_roomName, null);
        GameMgr.Inst.Log("joined room: " + room.m_roomName, enumLogLevel.RoomManagementLog);
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

        RoomOptions options = new RoomOptions { MaxPlayers = (byte)room.m_maxPlayer };

        options.CustomRoomProperties = new Hashtable { { PhotonFields.RoomInfo, room.roomInfoString } };
        options.CustomRoomPropertiesForLobby = new string[] { PhotonFields.RoomInfo};

        PhotonNetwork.CreateRoom(room.m_roomName, options, TypedLobby.Default);

        GameMgr.Inst.Log("room created " + room.m_roomName, enumLogLevel.RoomManagementLog);
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
        switch(m_gameType)
        {
            case enumGameType.Lami:
                switch(m_gameTier)
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
                switch(m_gameTier)
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
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneString);
    }
}
