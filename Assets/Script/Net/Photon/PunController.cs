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
        
    }

    public void Login()
    {
        if (!PhotonNetwork.IsConnected)
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(true);
            Debug.Log("!PhotonNetwork.IsConnected");
            PhotonNetwork.LocalPlayer.NickName = DataController.Inst.userInfo.name;
            PhotonNetwork.ConnectUsingSettings();
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
        GameMgr.Inst.messageMgr.OnMessageArrived((int) enumGameMessage.OnPlayerLeftRoom_onlyMaster, otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable updatedInfo)
    {
        object value;
        if (updatedInfo.TryGetValue(PhotonFields.GAME_MESSAGE, out value))
        {
            GameMgr.Inst.messageMgr.OnMessageArrived((int) value, player);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable updatedInfo)
    {
        object value;
        if (updatedInfo.TryGetValue(PhotonFields.GAME_MESSAGE, out value))
        {
            GameMgr.Inst.messageMgr.OnMessageArrived((int) value);
        }
    }

    #endregion
}