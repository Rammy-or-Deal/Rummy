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

public class PunController : MonoBehaviourPunCallbacks
{
    static public PunController Inst;

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
        Debug.Log("leave game");

        PhotonNetwork.LeaveRoom();
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

            byte maxPlayers = 10;
            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
            PhotonNetwork.CreateRoom(roomName, options, null);
            Debug.Log("room created " + roomName);
        }
    }

    public void CreateOrJoinLuckyRoom(int tierIdx)
    {
        string roomName = "lucky_" + mTierIdx.ToString();
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

    public void CreateRoom(string roomName)
    {
        roomName = roomName + "_" + DateTime.Now.ToString("MMddHHmmss");

        byte maxPlayers = 4;
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        Debug.Log("room created " + roomName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName, null);
        Debug.Log("join room" + roomName);
    }

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
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
        UIController.Inst.loadingDlg.gameObject.SetActive(false);
        if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
        {
            Debug.Log("Join Success");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("3_PlayLami");
            PhotonNetwork.LoadLevel("3_PlayLami");

            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                    (int)PhotonNetwork.LocalPlayer.ActorNumber,
                    DataController.Inst.userInfo.name,
                    DataController.Inst.userInfo.pic,
                    DataController.Inst.userInfo.coinValue,
                    DataController.Inst.userInfo.skillLevel,
                    DataController.Inst.userInfo.frameId,
                    (int)LamiPlayerStatus.Init
                );

            // Set local player's property.                    
            Hashtable props = new Hashtable
            {
                {Common.PLAYER_STATUS, (int)LamiPlayerStatus.Init},
                {Common.PLAYER_INFO, infoString}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            // Send Add New player Message. - OnUserEnteredRoom

            props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserEnteredRoom_M},
                {Common.NEW_PLAYER_INFO, infoString},
                {Common.NEW_PLAYER_STATUS, (int)LamiPlayerStatus.Init}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
        {
            SceneManager.LoadScene("3_PlayBaccarat");

            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                    (int)PhotonNetwork.LocalPlayer.ActorNumber,
                    DataController.Inst.userInfo.name,
                    DataController.Inst.userInfo.pic,
                    DataController.Inst.userInfo.coinValue,
                    DataController.Inst.userInfo.skillLevel,
                    DataController.Inst.userInfo.frameId,
                    (int)BaccaratPlayerType.Player
                );

            // Save my info to photon
            Hashtable props = new Hashtable
            {
                {Common.PLAYER_INFO, infoString},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // Send Add New player Message. - OnUserEnteredRoom
            props = new Hashtable
            {
                {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnUserEnteredRoom},
                {Common.NEW_PLAYER_INFO, infoString},
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("lucky"))
        {

        }


        //        Hashtable turnProps = new Hashtable();
        //        turnProps[Common.Game_START] = false;
        //        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("join room failed");
        string roomName = "rummy_" + mTierIdx.ToString();
        CreateRoom(roomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        UpdateCachedRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
        cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        Debug.Log(PhotonNetwork.NickName + "/me/Left Room");
        PhotonNetwork.LoadLevel("2_Lobby");
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "   Entered");
        if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
        {
            //LamiGameController.Inst.NewPlayerEnteredRoom(newPlayer);
        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
        {
            //BaccaratGameController.Inst.NewPlayerEnteredRoom(newPlayer);
        }

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "/other/left room");

        if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
        {
            //LamiGameController.Inst.OtherPlayerLeftRoom(otherPlayer);
            LamiMgr.Inst.SendMessage((int)LamiMessages.OnUserLeave_M, otherPlayer);
        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
        {
            BaccaratGameController.Inst.SendMessage((int)BaccaratMessages.OnUserLeave, otherPlayer);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
        }
    }

    public override void OnPlayerPropertiesUpdate(Player otherPlayer, Hashtable hashtable)
    {
        Debug.Log("OnPlayerPropertiesUpdate : " + otherPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
        {
            if (hashtable.ContainsKey(Common.LAMI_MESSAGE))
            {
                LamiMgr.Inst.SendMessage((int)hashtable[Common.LAMI_MESSAGE], otherPlayer);
            }
        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
        {
            if (hashtable.ContainsKey(Common.BACCARAT_MESSAGE))
            {
                BaccaratGameController.Inst.SendMessage((int)hashtable[Common.BACCARAT_MESSAGE], otherPlayer);
            }
        }

    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //Debug.Log("On Room Properties Update called.");
        if (PhotonNetwork.CurrentRoom.Name.Contains("rummy"))
        {
            if (propertiesThatChanged.ContainsKey(Common.LAMI_MESSAGE))
            {
                LamiMgr.Inst.SendMessage((int)propertiesThatChanged[Common.LAMI_MESSAGE]);
            }
        }
        else if (PhotonNetwork.CurrentRoom.Name.Contains("baccarat"))
        {
            if (propertiesThatChanged.ContainsKey(Common.BACCARAT_MESSAGE))
            {
                BaccaratGameController.Inst.SendMessage((int)propertiesThatChanged[Common.BACCARAT_MESSAGE]);
            }
        }
    }

    #endregion

    #region UI CALLBACKS

    #endregion
}