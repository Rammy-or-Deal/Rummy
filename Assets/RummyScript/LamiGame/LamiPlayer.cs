using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;

namespace Assets.RummyScript.LamiGame
{
    public class LamiPlayer
    {
        public int seat_id;
        public LamiUser_Info user_Info;
        public Lami_Card_Additional_Info card_Additional_Info;
        public int status;  // 0:init, 1:thinking, 2:ready, 3:giveup, 4:burnt, 5:game
        public bool isBot = false;
        internal bool canShow = false;

        public LamiPlayerMgr parent;
        public LamiPlayer()
        {

        }
        public void SetUserInfo(string data)
        {
            var tmp = data.Split(':');
            status = int.Parse(tmp[6]);
            user_Info.SetInfo(data);
        }
        public void SetAdditionalCardInfo(string data)
        {
            card_Additional_Info.SetInfo(data);
        }

        internal void SetProperty(int tmpActor)
        {
            canShow = true;
            isBot = true;
            foreach (Player p in PhotonNetwork.PlayerList)  // If this isn't a bot
            {                
                if (tmpActor == p.ActorNumber)
                {
                    string user_info = (string)p.CustomProperties[Common.PLAYER_INFO];
                    status = (int)p.CustomProperties[Common.PLAYER_STATUS];
                    SetUserInfo(user_info);
                    isBot = false;
                    break;
                }                
            }

            if (isBot)  //If this player is bot, pull data from parent's bot information
            {
                for(int i = 0; i < parent.m_botList.Count; i++)
                {
                    if(parent.m_botList[i].id == tmpActor)
                    {
                        status = parent.m_botList[i].status;
                        SetUserInfo(parent.m_botList[i].getBotString());
                    }
                }
            }
        }

        internal void Show()
        {
            if (canShow == true)
            {
                // Show player
            }
            else
            {
                // Hide player
            }
        }
    }
}
