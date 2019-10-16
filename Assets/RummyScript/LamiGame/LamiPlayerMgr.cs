using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class LamiPlayerMgr
    {
        public LamiMe m_lamiMe;
        public LamiPlayer[] m_playerList;
        public List<LamiGameBot> m_botList = new List<LamiGameBot>();
        public Dictionary<int, int> seatNumList;
        public LamiMgr parent;
        string master_seatString = "";
        public LamiPlayerMgr(LamiMgr parent)
        {
            this.parent = parent;
            seatNumList = new Dictionary<int, int>();
        }

        internal void OnUserEnteredRoom_M()
        {
            string newPlayerInfo = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NEW_PLAYER_INFO];
            int ActorNumber = int.Parse(newPlayerInfo.Split(':')[0]);
            
            string seatString = "";
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(Common.SEAT_STRING))    // If it's new room
            {
                seatString = ActorNumber + ":" + 0;
                Debug.Log("User Created the room.");
            }
            else    // If it's remained room
            {
                Debug.Log("OnJoinedRoom After");
                seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
                var userSeatList = seatString.Split(',');

                if (userSeatList.Length < 4) // If there is seat that the user can play.
                {
                    int[] seatNoList = new int[userSeatList.Length];
                    for (int i = 0; i < userSeatList.Length; i++)
                        seatNoList[i] = int.Parse(userSeatList[i].Split(':')[1]);

                    int seatNo = -1;
                    for (int i = 0; i <= 3; i++)
                    {
                        if (!seatNoList.Contains(i))
                        {
                            seatNo = i;
                            break;
                        }
                    }
                    seatString += "," + ActorNumber + ":" + seatNo;
                }
                else    // if there's no seat, remove bot
                {
                    int seatNo = -1;
                    int removedBot = 0;
                    for (int i = 0; i < userSeatList.Length; i++)
                    {
                        int tmpActor = int.Parse(userSeatList[i].Split(':')[0]);
                        int tmpSeat = int.Parse(userSeatList[i].Split(':')[1]);

                        if (tmpActor < 0)    // if actornumber < 0, this is a bot. 
                        {
                            seatNo = tmpSeat;
                            removedBot = tmpActor;
                            tmpActor = ActorNumber;
                            userSeatList[i] = tmpActor + ":" + tmpSeat;
                            break;
                        }
                    }

                    if (removedBot != 0)    // If the bot is removed, send bot remove messages
                    {
                        Debug.Log("Bot removed: " + removedBot);
                        Hashtable botChangeString = new Hashtable
                        {
                            {Common.LAMI_MESSAGE, (int)LamiMessages.OnRemovedBot},
                            {Common.REMOVED_BOT_ID, removedBot}
                        };
                        PhotonNetwork.CurrentRoom.SetCustomProperties(botChangeString);
                    }

                    seatString = "";
                    for (int i = 0; i < userSeatList.Length; i++)
                    {
                        seatString += userSeatList[i] + ",";
                    }
                    seatString = seatString.Trim(',');
                }
            }

            // Send RoomUpdate Messages to all players.
            Debug.Log("SeatString updated: " + seatString);
            Hashtable turnProps = new Hashtable
                {
                    {Common.LAMI_MESSAGE, (int)LamiMessages.OnRoomSeatUpdate},
                    {Common.SEAT_STRING, seatString},
                };
            master_seatString = seatString;
            PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
        }

        internal void OnJoinSuccess()
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(false);
            PhotonNetwork.LoadLevel("3_PlayLami");
            Debug.Log("Joined Room and Lami Play started");
            m_lamiMe = new LamiMe(this);
            m_lamiMe.SendMyInfo();
        }
        internal void OnRoomSeatUpdate()
        {
            string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];

            // Prepare seatNumList      - this is to remove unneeded for statement
            if (seatString != "")
                seatNumList.Clear();
            var tmp = seatString.Split(',');
            for (int i = 0; i < tmp.Length; i++)
            {
                seatNumList.Add(int.Parse(tmp[i].Split(':')[0]), int.Parse(tmp[i].Split(':')[1]));
            }

            // Get seat no from seat string
            for (int i = 0; i < m_playerList.Length; i++)
                m_playerList[i].canShow = false;

            for (int i = 0; i < tmp.Length; i++)
            {
                int tmpActor = int.Parse(tmp[i].Split(':')[0]);
                int tmpSeat = int.Parse(tmp[i].Split(':')[1]);
                m_playerList[GetUserSeat(tmpSeat)].SetProperty(tmpActor);
            }

            // Show/Hide players;
            for (int i = 0; i < m_playerList.Length; i++)
                m_playerList[i].Show();
        }

        private int GetUserSeat(int seatNo_in_seatString)
        {

            int seatPos;
            if (seatNo_in_seatString == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
            {
                seatPos = 0;
            }
            else if (seatNo_in_seatString > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
            {
                seatPos = seatNo_in_seatString - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
            }
            else
            {
                seatPos = 4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNo_in_seatString;
            }

            return seatPos;
        }

        #region  Bot Section
        internal void OnBotInfoChanged()
        {
            string botListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BOT_LIST_STRING];

            var tmp = botListString.Split(',');
            m_botList.Clear();
            for (int i = 0; i < tmp.Length; i++)
            {
                LamiGameBot bot = new LamiGameBot(this);
                bot.SetBotInfo(tmp[i]);
                m_botList.Add(bot);

                LogMgr.Log("Bot Created : " + bot.getBotString(), (int)LogLevels.BotLog);
            }
        }
        internal void OnRemovedBot()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int removedBotId = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.REMOVED_BOT_ID];
                m_botList = m_botList.Where(x => x.id != removedBotId).ToList();
                SendBotListString();
            }
        }
        void SendBotListString()
        {
            string botString = "";
            for (int i = 0; i < m_botList.Count; i++)
            {
                botString += m_botList[i].getBotString() + ",";
            }
            botString = botString.Trim(',');

            Hashtable botChangeString = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnBotInfoChanged},
                {Common.BOT_LIST_STRING, botString}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(botChangeString);
        }
        void MakeOneBot()
        {
            // Check if there's empty seat
            string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
            var tmp = seatString.Split(',');
                    // if all seats are not empty, return.
            if(tmp.Length >= 4) return;

            // Create a bot
            LamiGameBot mBot = new LamiGameBot(this);
            mBot.Init();
            bool isIdSet = false;

            while(isIdSet == false)
            {
                isIdSet = true;
                for(int i = 0; i < m_botList.Count; i++)
                {
                    if(mBot.id == m_botList[i].id)
                        isIdSet = false;
                }
                if(isIdSet == false)
                {
                    mBot.id = -(UnityEngine.Random.Range(1000, 9999));
                }
            }

            mBot.SendMyInfo();
            
            m_botList.Add(mBot);            

            LogMgr.Log("Bot Created : " + mBot.getBotString(), (int)LogLevels.BotLog);
            SendBotListString();
        }

        #endregion
    }
}