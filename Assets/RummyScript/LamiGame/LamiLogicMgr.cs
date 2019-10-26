using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LamiLogicMgr : MonoBehaviour
{
    public static LamiLogicMgr Inst;
    public bool isStart = false;

    private void Awake()
    {
        if (!Inst)
            Inst = this;
    }

    #region Join Or Create Room
    public void OnJoinRoomClicked(int tierIdx = 0)
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        int mTierIdx = tierIdx;
        string roomName = "rummy_" + mTierIdx.ToString();
        bool isNewRoom = true;

        LogMgr.Inst.Log("cachedRoom : " + PunController.Inst.cachedRoomList.Count);

        foreach (RoomInfo info in PunController.Inst.cachedRoomList.Values)
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

    private void CreateRoom(string roomName)
    {
        roomName = roomName + "_" + DateTime.Now.ToString("MMddHHmmss");
        byte maxPlayers = 4;
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        Debug.Log("room created " + roomName);
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName, null);
        Debug.Log("join room" + roomName);
    }

    #endregion


    #region Creating Bot
    private void Start()
    {
        UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        UIController.Inst.moneyPanel.gameObject.SetActive(false);
        //        ShowPlayers();
        LamiCountdownTimer.Inst.StartTimer();
        isStart = false;
        StartCoroutine(CreateBot());
        
    }

    public IEnumerator CreateBot()
    {
        //int botWaitTime = UnityEngine.Random.Range(7, 10);
        int botWaitTime = 2;
        if(Constants.LamiBuildMethod == BuildMethod.Product)
            botWaitTime = UnityEngine.Random.Range(7, 10);
            
        while (!isStart)
        {
            yield return new WaitForSeconds(botWaitTime);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                LogMgr.Inst.Log("Bot Create Command Sent : ", (int) LogLevels.BotLog);
                LamiPlayerMgr.Inst.MakeOneBot();
            }
        }
    }
    #endregion

    public void OnMessageArrived(int message, Player p = null)
    {
        LogMgr.Inst.Log("Message Arrived: " + (LamiMessages)message);

        switch (message)
        {
            case (int)LamiMessages.OnJoinSuccess:
                LamiPlayerMgr.Inst.OnJoinSuccess();
                break;
            case (int)LamiMessages.OnUserEnteredRoom_M:
                if (PhotonNetwork.IsMasterClient)
                    LamiPlayerMgr.Inst.OnUserEnteredRoom_M();
                break;
            case (int)LamiMessages.OnRoomSeatUpdate:
                LamiPlayerMgr.Inst.OnRoomSeatUpdate();
                break;
            case (int)LamiMessages.OnRemovedBot:
                LamiPlayerMgr.Inst.OnRemovedBot();
                break;
            case (int)LamiMessages.OnBotInfoChanged:
                LamiPlayerMgr.Inst.OnBotInfoChanged();
                break;
            case (int)LamiMessages.OnUserReady:
                LamiPlayerMgr.Inst.OnUserReady(p.ActorNumber);
                break;
            case (int)LamiMessages.OnUserReady_BOT:
                int id = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BOT_ID];
                LamiPlayerMgr.Inst.OnUserReady(id);
                break;
            case (int)LamiMessages.OnUserLeave_M:
                if (PhotonNetwork.IsMasterClient)
                    LamiPlayerMgr.Inst.OnUserLeave_M(p.ActorNumber);                    
                break;
            case (int)LamiMessages.OnStartGame:
                StopCoroutine(CreateBot());
                LamiPlayerMgr.Inst.OnStartGame();
                isStart = true;
                break;
            case (int)LamiMessages.OnCardDistributed:
                LamiPlayerMgr.Inst.OnCardDistributed();
                break;
            case (int)LamiMessages.OnUserReadyToStart_M:
                if (PhotonNetwork.IsMasterClient)
                    LamiPlayerMgr.Inst.OnUserReadyToStart_M();
                break;
            case (int)LamiMessages.OnDealCard:
                LamiPlayerMgr.Inst.OnDealCard();
                LamiPanMgr.Inst.OnDealCard();
                break;
            case (int)LamiMessages.OnUserTurnChanged:
                LamiPlayerMgr.Inst.OnUserTurnChanged();
                break;
            case (int)LamiMessages.OnPlayerStatusChanged:
                LamiPlayerMgr.Inst.OnPlayerStatusChanged();
                break;
            case (int)LamiMessages.OnGameFinished:
                LamiPlayerMgr.Inst.OnGameFinished();
                break;
        }
    }
}