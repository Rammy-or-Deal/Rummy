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
        Login();
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
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("join room failed");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameMgr.Inst.Log("room count:=" + roomList.Count);
        if (roomList == null) return;
        GameMgr.Inst.roomMgr.OnRoomListUpdate(roomList);

    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
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

        // try
        // {
            GameMgr.Inst.messageMgr.OnMessageArrived((int)enumGameMessage.OnPlayerLeftRoom_onlyMaster, otherPlayer);
        // }
        // catch (Exception Log)
        // {
        //     Debug.LogError("OnPlayerLeftRoom Error: " + Log.Message);
        // }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable updatedInfo)
    {
        // try
        // {
            object value;
            if (updatedInfo.TryGetValue(PhotonFields.GAME_MESSAGE, out value))
            {
                GameMgr.Inst.messageMgr.OnMessageArrived((int)value, player);
            }
        // }
        // catch (Exception Log)
        // {
        //     Debug.LogError("OnPlayerPropertiesUpdate Error: " + Log.Message);
        // }
    }

    public override void OnRoomPropertiesUpdate(Hashtable updatedInfo)
    {
        // try
        // {
            object value;
            if (updatedInfo.TryGetValue(PhotonFields.GAME_MESSAGE, out value))
            {
                GameMgr.Inst.messageMgr.OnMessageArrived((int)value);
            }
        // }
        // catch (Exception Log)
        // {
        //     Debug.LogError("OnRoomPropertiesUpdate Error: " + Log.Message);
        // }
    }

    #endregion
   
}