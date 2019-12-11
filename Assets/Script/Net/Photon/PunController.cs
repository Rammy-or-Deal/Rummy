using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class PunController : MonoBehaviourPunCallbacks
{
    static public PunController Inst;
    private TypedLobby _defaultLobby = new TypedLobby("rummyLobby", LobbyType.SqlLobby);
    public Dictionary<string, RoomInfo> cachedRoomList;

    private int mTierIdx;

    public void Awake()
    {
        if (!Inst)
            Inst = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Start()
    {
        Debug.Log("PunController started");
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        cachedRoomList = new Dictionary<string, RoomInfo>();

        Login();
    }

    public void StartLamiTier()
    {
        PhotonNetwork.LoadLevel("2_Lami");
    }

    public void Login()
    {
        string playerName = DataController.Inst.userInfo.name;

        if (!playerName.Equals("") || !PhotonNetwork.IsConnected)
        {
            Debug.Log("!PhotonNetwork.IsConnected");
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void LeaveGame()
    {
        try
        {
            Debug.Log("leave game");
            PhotonNetwork.LeaveRoom();
        }
        catch { }
    }

    public void CreateOrJoinRoom(int tierIdx)
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        mTierIdx = tierIdx;
        string roomName = "rummy_" + mTierIdx.ToString();
        bool isNewRoom = true;

        Debug.Log("cachedRoom : " + cachedRoomList.Count);

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            Debug.Log(info);

            if (info.Name.Contains(roomName))
            {
                isNewRoom = false;
                JoinRoom(info.Name);
            }
            else
            {
                Debug.Log(info.Name + "can't join");
            }
        }

        if (isNewRoom)
        {
            CreateRoom(roomName);
        }
    }

    internal void CreateOrJoinRoom(enumGameType m_gameType, enumGameTier m_gameTier)
    {
        throw new Exception();
    }

    public void CreateBacaratRoom(BaccaratRoomInfo roomInfo)
    {
        string roomName = "baccarat_" + Random.Range(0, 100);
        bool isNameSet = false;
        while (!isNameSet)
        {
            isNameSet = true;
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                if (info.Name == roomName)
                {
                    isNameSet = false;
                    roomName = "baccarat_" + Random.Range(0, 100);
                    break;
                }
            }
        }
        CreateRoom(roomName, 9, roomInfo.roomString);
        Debug.Log("room created:" + roomName + ", property:=" + roomInfo.roomString);
    }

    public void CreateOrJoinBaccaratRoom()
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        string roomName = "baccarat_";
        bool isNewRoom = true;

        Debug.Log("cachedRoom : " + cachedRoomList.Count);

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            Debug.Log(info);

            if (info.Name.Contains(roomName))
            {
                isNewRoom = false;
                JoinRoom(info.Name);
            }
            else
            {
                Debug.Log(info.Name + "can't join");
            }
        }

        if (isNewRoom)
        {
            roomName = roomName + "_" + DateTime.Now.ToString("MMddHHmmss");
            CreateRoom(roomName, 10, "E:A:SDFQW:EAS234:346:Adsfa:WERQ6:623:asd");
            Debug.Log("room created " + roomName);
        }
    }

    public void CreateOrJoinLuckyRoom(int tierIdx)
    {
        string roomName = "fortune_" + tierIdx.ToString();
        bool isNewRoom = true;

        Debug.Log("cachedRoom : " + cachedRoomList.Count);

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            Debug.Log(info);

            if (info.Name.Contains(roomName))
            {
                isNewRoom = false;
                JoinRoom(info.Name);
            }
            else
            {
                Debug.Log(info.Name + "can't join");
            }
        }

        if (isNewRoom)
        {
            roomName = roomName + "_" + DateTime.Now.ToString("MMddHHmmss");

            byte maxPlayers = 4;
            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
            PhotonNetwork.CreateRoom(roomName, options, null);
            Debug.Log("room created " + roomName);
        }
    }

    public void CreateRoom(string roomName, byte maxPlayers = 4, string AdditionalRoomProperty = "")
    {
        roomName = roomName + "_" + DateTime.Now.ToString("MMddHHmmss");

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        if (AdditionalRoomProperty != "")
        {
            options.CustomRoomProperties = new Hashtable { { Common.AdditionalRoomProperty, AdditionalRoomProperty } };
            options.CustomRoomPropertiesForLobby = new string[] { Common.AdditionalRoomProperty, Common.BaccaratRoomPlayers };
        }
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        Debug.Log("room created " + roomName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName, null);
        Debug.Log("join room" + roomName);
    }
    public Dictionary<string, string> baccaratRoomList = new Dictionary<string, string>();
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        baccaratRoomList.Clear();
        foreach (RoomInfo info in roomList)
        {
            try
            {
                Debug.Log("RoomProeprty:=" + (string)info.CustomProperties[Common.AdditionalRoomProperty]);
                baccaratRoomList.Add(info.Name, (string)info.CustomProperties[Common.AdditionalRoomProperty]);
            }
            catch { }

            Debug.Log("roomList" + info);
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList || info.PlayerCount == info.MaxPlayers)
            {
                Debug.Log(
                    "!info.IsOpen || !info.IsVisible || info.RemovedFromList || info.PlayerCount == info.MaxPlayers");
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    Debug.Log("cachedRoomList.Remove" + info.Name);
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;

                Debug.Log("ContainsKey(info.Name)" + info);
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
                Debug.Log("else" + info);
            }
        }

        //        Debug.Log("update cached RoomList:" + cachedRoomList.Count);
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        Debug.Log("onConnectedMaster LobbyPun");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        UIController.Inst.loadingDlg.gameObject.SetActive(false);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby LobbyPun");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        UIController.Inst.loadingDlg.gameObject.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        GameMgr.Inst.roomMgr.OnJoinedRoom();

        #region Old Code
        /*
            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                (int)PhotonNetwork.LocalPlayer.ActorNumber,
                DataController.Inst.userInfo.name,
                DataController.Inst.userInfo.pic,
                DataController.Inst.userInfo.coinValue,
                DataController.Inst.userInfo.skillLevel,
                DataController.Inst.userInfo.frameId,
                (int)enumPlayerStatus.Rummy_Init
            );
            string gameScene = "3_PlayLami";

            UIController.Inst.loadingDlg.gameObject.SetActive(false);
            if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
            {
                // Set local player's property.                    
                Hashtable props = new Hashtable
                {
                    {Common.PLAYER_STATUS, (int)enumPlayerStatus.Rummy_Init},
                    {Common.PLAYER_INFO, infoString}
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                // Send Add New player Message. - OnUserEnteredRoom

                props = new Hashtable
                {
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserEnteredRoom_M},
                    {Common.NEW_PLAYER_INFO, infoString},
                    {Common.NEW_PLAYER_STATUS, (int)enumPlayerStatus.Rummy_Init}
                };

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            }
            else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
            {
                gameScene = "3_PlayBaccarat";
                // Save my info to photon
                Hashtable props = new Hashtable
                {
                    {Common.PLAYER_INFO, infoString},
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                // Send Add New player Message. - OnUserEnteredRoom
                props = new Hashtable
                {
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnUserEnteredRoom},
                    {Common.NEW_PLAYER_INFO, infoString},
                };

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            }
            else if (PhotonNetwork.CurrentRoom.Name.Contains("fortune"))
            {
                gameScene = "3_PlayFortune13";
                // Save my info to photon
                Hashtable props = new Hashtable
                {
                    {Common.FORTUNE_MESSAGE, (int)RoomManagementMessages.OnUserEnteredRoom_M},
                    {Common.PLAYER_INFO, infoString},
                    {Common.PLAYER_STATUS, (int)FortunePlayerStatus.Init}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
            */
        #endregion

        // if (PhotonNetwork.IsMasterClient)
        //     PhotonNetwork.LoadLevel(gameScene);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("join room failed");
        string roomName = "rummy_" + mTierIdx.ToString();
        //CreateRoom(roomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        //UpdateCachedRoomList(roomList);
        GameMgr.Inst.Log("room count:=" + roomList.Count);
        if (roomList == null) return;
        GameMgr.Inst.roomMgr.OnRoomListUpdate(roomList);

    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
        cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        GameMgr.Inst.roomMgr.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        try
        {
            GameMgr.Inst.messageMgr.OnMessageArrived((int)enumGameMessage.OnPlayerLeftRoom_onlyMaster, otherPlayer);
        }
        catch (Exception Log)
        {
            Debug.LogError("OnPlayerLeftRoom Error: " + Log.Message);
        }

        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable updatedInfo)
    {       
        try
        {
            GameMgr.Inst.messageMgr.OnMessageArrived((int)updatedInfo[PhotonFields.GAME_MESSAGE], player);
        }
        catch (Exception Log)
        {
            Debug.LogError("OnPlayerPropertiesUpdate Error: " + Log.Message);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable updatedInfo)
    {
        try
        {
            GameMgr.Inst.messageMgr.OnMessageArrived((int)updatedInfo[PhotonFields.GAME_MESSAGE]);
        }
        catch (Exception Log)
        {
            Debug.LogError("OnRoomPropertiesUpdate Error: " + Log.Message);
        }
    }

    #endregion

    #region UI CALLBACKS

    #endregion
}