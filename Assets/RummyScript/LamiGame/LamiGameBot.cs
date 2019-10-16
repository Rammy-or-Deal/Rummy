using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Assets.RummyScript.LamiGame
{
    public class LamiGameBot
    {
        List<tmpCard> m_cardList = new List<tmpCard>(); // my cards
        List<tmpCard> remained = new List<tmpCard>(); // selected cards
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

        LamiPlayerMgr parent;

        public string[] skillLevelList = new string[] { "Novice", "Expert", "Hero", "Elite", "King", "Master" };

        public LamiGameBot(LamiPlayerMgr parent)
        {
            this.parent = parent;
        }
        public void Init()
        {            
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
            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                    id,
                    name,
                    pic,
                    coinValue,
                    skillLevel,
                    frameId
                    );
            return infoString;
        }

        internal void SetBotInfo(string v)
        {
            var tmp = v.Split(':');
            id = int.Parse(tmp[0]);
            name = tmp[1];
            pic = tmp[2];
            coinValue = int.Parse(tmp[3]);
            skillLevel = tmp[4];
            frameId = int.Parse(tmp[5]);
        }


        /*************************************************** */
    }
}