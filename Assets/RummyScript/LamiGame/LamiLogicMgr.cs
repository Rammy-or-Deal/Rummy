using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class LamiLogicMgr : MonoBehaviour
    {
        public static LamiLogicMgr Inst;

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

            LogMgr.Log("cachedRoom : " + PunController.Inst.cachedRoomList.Count);

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

        private void Start()
        {
            UIController.Inst.userInfoPanel.gameObject.SetActive(false);
            UIController.Inst.moneyPanel.gameObject.SetActive(false);
            //        ShowPlayers();
            LamiCountdownTimer.Inst.StartTimer();

            StartCoroutine(CreateBot());

        }
        #region Creating Bot

        public IEnumerator CreateBot()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                int botWaitTime = UnityEngine.Random.RandomRange(5, 10);

                while (true)
                {
                    yield return new WaitForSeconds(botWaitTime);
                    LogMgr.Log("Bot Create Command Sent : ", (int)LogLevels.BotLog);
                    LamiPlayerMgr.Inst.MakeOneBot();
                }
            }
        }
        #endregion

        public void OnMessageArrived(int message, Player p = null)
        {
            LogMgr.Log(message + "");

            switch (message)
            {
                case (int)LamiMessages.OnJoinSuccess:
                    LamiPlayerMgr.Inst.OnJoinSuccess();
                    break;
                case (int)LamiMessages.OnUserEnteredRoom_M:
                    if (PhotonNetwork.IsMasterClient)
                        LamiPlayerMgr.Inst.OnUserEnteredRoom_M(p.ActorNumber);
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
                case (int)LamiMessages.OnUserLeave_M:
                    if (PhotonNetwork.IsMasterClient)
                        LamiPlayerMgr.Inst.OnUserLeave(p.ActorNumber);
                    break;
                case (int)LamiMessages.OnStartGame:
                    StopCoroutine(CreateBot());
                    LamiPlayerMgr.Inst.OnStartGame();
                    break;
                case (int)LamiMessages.OnCardDistributed:
                    LamiPlayerMgr.Inst.OnCardDistributed();
                    break;
            }
        }
    }
}