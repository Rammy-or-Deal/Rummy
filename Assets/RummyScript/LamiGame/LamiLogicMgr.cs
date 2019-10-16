using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class LamiLogicMgr :MsgMgr
    {
        public LamiLogicMgr(LamiMgr parent) : base(parent)
        {
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

        public void OnMessageArrived(int message)
        {
            switch (message)
            {
                case (int)LamiMessages.OnJoinSuccess:
                    parent.playerMgr.OnJoinSuccess();
                    break;
                case (int)LamiMessages.OnUserEnteredRoom_M:
                    if (PhotonNetwork.IsMasterClient)
                        parent.playerMgr.OnUserEnteredRoom_M();
                    break;
                case (int)LamiMessages.OnRoomSeatUpdate:
                    parent.playerMgr.OnRoomSeatUpdate();
                    break;
                case (int)LamiMessages.OnBotInfoChanged:
                    parent.playerMgr.OnBotInfoChanged();
                    break;
            }
        }
    }
}