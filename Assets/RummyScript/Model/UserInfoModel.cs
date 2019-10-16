using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RummyScript.Model
{
    [Serializable]
    public class UserInfoModel
    {
        public int id;
        public string name;
        public string pic;
        
        public string winRate;
//        public string remark;
        
//        public string starPic;
//        public string starValue;
        
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
        
        public string[ ] skillLevelList = new string[ ]{"Novice", "Expert", "Hero", "Elite", "King", "Master"} ;

        public UserInfoModel()
        {
            
            
            winRate = "12/20";
            
            
            leafPic = "new_symbol/leaf";
            coinValue = 2500;
            leafValue = 300;

            announce = "Announce Text";
            message = "There is no message";
            email = "There is no email";
            coinPic = "new_symbol/coin";
            skillLevel = "expert";
            frameId = 3;

        }

        public void Init()
        {
            name = "Guest"+"["+Random.Range(1000,9999).ToString() + "]";
            pic = "new_avatar/avatar_" + Random.Range(1,26).ToString();

            coinValue = Random.Range(1000,9999);
            leafValue = Random.Range(100,999);
            skillLevel = skillLevelList[Random.Range(0, 6)];
        }

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}