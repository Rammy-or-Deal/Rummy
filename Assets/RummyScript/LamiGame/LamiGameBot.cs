using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class LamiGameBot
    {
        Card[] m_cardList; // my cards
        Card [] remained; // selected cards
        public int id;
        public string name = "";
        public string pic;
        public string winRate;
        public string coinPic;
        public int coinValue;
        public string leafPic;
        public int leafValue;
        public string announce;
        public string message;
        public string email;
        public int giftItemId;
        public int giftItemCount;
        public int skillId;
        public int skillValue;
        public string skillLevel;
        public int frameId;
        public int friendItemId;
        public int requestId;
        public int status;
        LamiPlayerMgr parent;

        public string[] skillLevelList = new string[] { "Novice", "Expert", "Hero", "Elite", "King", "Master" };

        public LamiGameBot(LamiPlayerMgr parent)
        {
            this.parent = parent;
        }
        public void Init()
        {
            status = (int)LamiPlayerStatus.Ready;
            id = -(Random.Range(1000, 9999));
            name = "Guest" + "[" + Random.Range(1000, 9999).ToString() + "]";
            pic = "new_avatar/avatar_" + Random.Range(1, 26).ToString();

            coinValue = Random.Range(1000, 9999);
            leafValue = Random.Range(100, 999);
            skillLevel = skillLevelList[Random.Range(0, 6)];

            winRate = "12/20";
            leafPic = "new_symbol/leaf";

            announce = "Announce Text";
            message = "There is no message";
            email = "There is no email";
            coinPic = "new_symbol/coin";
            skillLevel = "expert";
            frameId = 3;
        }
        internal string getBotString()
        {
            //data : 0:id, 1:name, 2:picUrl, 3:coinValue, 4:skillLevel, 5:frameId
            //      format: id:name:picUrl:coinValue:skillLevel:frameId:status
            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                    id,
                    name,
                    pic,
                    coinValue,
                    skillLevel,
                    frameId,
                    status
                    );
            return infoString;
        }

        internal void SetBotInfo(string v)
        {
            LogMgr.Inst.Log("Bot String: " + v, (int)LogLevels.BotLog);

            var tmp = v.Split(':');
            id = int.Parse(tmp[0]);
            name = tmp[1];
            pic = tmp[2];
            coinValue = int.Parse(tmp[3]);
            skillLevel = tmp[4];
            frameId = int.Parse(tmp[5]);
            status = int.Parse(tmp[6]);
        }
        public void PublishMe()
        {
            string infoString = "";            
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                    id,
                    name,
                    pic,
                    coinValue,
                    skillLevel,
                    frameId,
                    status
                    );

                // Send Add New player Message. - OnUserEnteredRoom
            Hashtable props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserEnteredRoom_M},
                {Common.NEW_PLAYER_INFO, infoString},
                {Common.NEW_PLAYER_STATUS, status},
                {Common.IS_BOT, true}
            };
    
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);            

            ReadyMessageSend();
        }
        async void ReadyMessageSend()
        {
            await Task.Delay(1000);
            status = (int)LamiPlayerStatus.Ready;

            Hashtable props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserReady_BOT},
                {Common.BOT_ID, id},
                {Common.BOT_STATUS, status},
            };
    
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        internal void SetMyCards(string cardString)
        {
            m_cardList = LamiCardMgr.ConvertCardStrToCardList(cardString);
        }
        /*************************************************** */
    }
}