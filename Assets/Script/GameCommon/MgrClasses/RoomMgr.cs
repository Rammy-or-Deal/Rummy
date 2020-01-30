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
                GameMgr.Inst.Log("Created Room: name=" + info.Name + ", Info=" + gameInfo, LogLevel.RoomLog);
                room.roomInfoString = gameInfo;
                m_roomList.Add(room);
            }
            catch (Exception err)
            {
                GameMgr.Inst.Log("Room information isn't correct. Error:= " + err.Message, LogLevel.RoomLog);
            }
        }
        GameMgr.Inst.Log("RoomListUpdate Called. RoomCount = " + roomList.Count, LogLevel.RoomLog);
        GameMgr.Inst.Log("Rooms:=" + string.Join("/", m_roomList.Select(x => x.m_maxPlayer)), LogLevel.RoomLog);
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
            GameMgr.Inst.Log("joined room: " + m_currentRoom.m_roomName, LogLevel.RoomLog);
        }
        catch (Exception err)
        {
            GameMgr.Inst.Log("JoinRoom Failed. So I should create a new room.(Error:" + err.Message + ")", LogLevel.RoomLog);
            CreateRoom(m_gameType, m_gameTier);
        }
    }

    public bool JoinRoom(string roomName)
    {
        if (m_roomList.Count(x => x.m_roomName == roomName) > 0)
        {
            m_currentRoom.roomInfoString = m_roomList.Where(x => x.m_roomName == roomName).First().roomInfoString;
            PhotonNetwork.JoinRoom(m_currentRoom.m_roomName, null);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void CreateRoom_basedRoomInfo(GameRoomInfo tmp_roomInfo)
    {
        GameRoomInfo room = new GameRoomInfo();
        room.m_gameType = tmp_roomInfo.m_gameType;
        room.m_gameTier = tmp_roomInfo.m_gameTier;
        room.m_roomName = tmp_roomInfo.m_roomName;
        room.m_gameFee = tmp_roomInfo.m_gameFee;
        room.m_maxPlayer = tmp_roomInfo.m_maxPlayer;
        room.m_playerCount = tmp_roomInfo.m_playerCount;

        room.m_additionalString = tmp_roomInfo.m_additionalString;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)room.m_maxPlayer;
        options.CustomRoomProperties = new Hashtable { { PhotonFields.RoomInfo, room.roomInfoString } };
        options.CustomRoomPropertiesForLobby = new string[1] { PhotonFields.RoomInfo };

        m_currentRoom.roomInfoString = room.roomInfoString;
        PhotonNetwork.CreateRoom(room.m_roomName, options, TypedLobby.Default);

        GameMgr.Inst.Log("room created " + room.m_roomName, LogLevel.RoomLog);

    }
    public void CreateRoom(enumGameType m_gameType, enumGameTier m_gameTier, string additionalString = "")
    {
        GameRoomInfo room = new GameRoomInfo();
        room.m_gameType = m_gameType;
        room.m_gameTier = m_gameTier;
        room.m_roomName = string.Format("{0}_{1}_{2}", m_gameType, m_gameTier, DateTime.Now.ToString("MMddHHmmss"));
        room.m_gameFee = GetGameFeeOfGame(m_gameType, m_gameTier);
        room.m_maxPlayer = GetMaxPlayerOfGame(m_gameType, m_gameTier);
        room.m_playerCount = 0;
        room.m_additionalString = additionalString;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)room.m_maxPlayer;
        options.CustomRoomProperties = new Hashtable { { PhotonFields.RoomInfo, room.roomInfoString } };
        options.CustomRoomPropertiesForLobby = new string[1] { PhotonFields.RoomInfo };

        m_currentRoom.roomInfoString = room.roomInfoString;
        PhotonNetwork.CreateRoom(room.m_roomName, options, TypedLobby.Default);

        GameMgr.Inst.Log("room created " + room.m_roomName, LogLevel.RoomLog);

    }

    public int GetMaxPlayerOfGame(enumGameType m_gameType, enumGameTier m_gameTier)
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

    public int GetGameFeeOfGame(enumGameType m_gameType, enumGameTier m_gameTier)
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
        catch (Exception e)
        {
            Debug.LogError(e);
        }
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

    internal void CheckUsersAvailability()
    {
        foreach (var p in GameMgr.Inst.seatMgr.m_playerList)
        {
            if (PhotonNetwork.PlayerList.Count(x => x.ActorNumber == p.m_playerInfo.m_actorNumber) == 0)
            {
                OnPlayerLeftRoom_onlyMaster(p.m_playerInfo.m_actorNumber);
            }
        }
    }

    internal void OnLeftRoom()
    {
        GameMgr.Inst.Log(PhotonNetwork.NickName + "/me/Left Room");
        PhotonNetwork.LoadLevel(constantContainer.strLobby);
        m_roomList.Clear();

        GameMgr.Inst.InitStatus();
    }
    internal void OnPlayerLeftRoom_onlyMaster(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (GameMgr.Inst.m_gameType == enumGameType.Lami && GameMgr.Inst.m_gameStatus == enumGameStatus.OnGameStarted) return;

        PlayerInfoContainer pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        SeatInfo seatInfo = new SeatInfo();
        seatInfo.seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING];

        m_currentRoom.m_playerCount--;
        GameMgr.Inst.Log("Before user left room. pList=" + pList.m_playerInfoListString, LogLevel.RoomLog);
        GameMgr.Inst.Log("Before user left room. seatInfo=" + seatInfo.seatString, LogLevel.RoomLog);
        var p = pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First();
        pList.m_playerList.Remove(p);
        var s = seatInfo.seatList.Where(x => x.m_actorNumber == actorNumber).First();
        seatInfo.seatList.Remove(s);

        Hashtable turnProps = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.SEAT_STRING, seatInfo.seatString},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
            {PhotonFields.RoomInfo, m_currentRoom.roomInfoString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);

        GameMgr.Inst.Log("After user left room. pList=" + pList.m_playerInfoListString, LogLevel.RoomLog);
        GameMgr.Inst.Log("After user left room. seatInfo=" + seatInfo.seatString, LogLevel.RoomLog);
    }
    internal void OnUserEnteredRoom_onlyMaster()
    {
        PlayerInfo newPlayer = new PlayerInfo();
        PlayerInfoContainer pList = new PlayerInfoContainer();

        newPlayer.playerInfoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.NEW_PLAYER_INFO];

        GameMgr.Inst.Log("New Player Info: " + newPlayer.playerInfoString, LogLevel.RoomLog);

        int actorNumber = newPlayer.m_actorNumber;

        string seatString = "";
        SeatInfo seatInfo = new SeatInfo();

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PhotonFields.SEAT_STRING))
        {
            seatInfo.AddOneSeat(actorNumber, 0, newPlayer.m_userName);
            pList.m_playerList.Add(newPlayer);
            GameMgr.Inst.Log("I successfully created this room.", LogLevel.RoomLog);
        }
        else
        {
            GameMgr.Inst.Log("On JoinedRoom After", LogLevel.RoomLog);

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
        GameMgr.Inst.Log("current Room String:=" + m_currentRoom.roomInfoString, LogLevel.RoomLog);
        GameMgr.Inst.Log("current Room SeatString:=" + seatString, LogLevel.RoomLog);
        GameMgr.Inst.Log("current Room PlayerListString:=" + pList.m_playerInfoListString, LogLevel.RoomLog);

        Hashtable turnProps = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.SEAT_STRING, seatString},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
            {PhotonFields.RoomInfo, m_currentRoom.roomInfoString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);

        StartCoroutine(SendIamJoined());
    }

    IEnumerator SendIamJoined()
    {
        yield return new WaitForSeconds(1);
        //enumGameMessage.OnJoinSuccess
        //PhotonNetwork.LocalPlayer
    }
}