using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RummyScript.Model
{
    [Serializable]
    public class UserInfoModel
    {
        public int id;
        public string facebook_id;
        public string name;
        public Sprite sprite;
        public string pic;
        
        public string winRate;
//        public string remark;
        
//        public string starPic;
//        public string starValue;
        
        public string coinPic;
        public int coin_value;
        
        public string leafPic;
        public int leaf_value;

        public string announce;
        public string message;
        public string email;
        
        public int giftItemId;
        public int giftItemCount;
        
        public int skillId;
        public int skillValue;
        public string skill_level;
        
        public int frame_id;
        public int friendItemId;
        public int requestId;
        public string udid;
        
        

        public UserInfoModel()
        {                        
            winRate = "12/20";                        
            leafPic = "new_symbol/leaf";
            coin_value = 2500;
            leaf_value = 300;
            announce = "Announce Text";
            message = "There is no message";
            email = "There is no email";
            coinPic = "new_symbol/coin";
            frame_id = 3;
        }

        public void Init(string prefixName="Guest")
        {
            name = prefixName+"["+Random.Range(1000,9999).ToString() + "]";
            pic = "new_avatar/avatar_" + Random.Range(1,26).ToString();
            sprite= Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
            
            coin_value = Random.Range(50000,99999);
            leaf_value = Random.Range(100,999);
            skill_level = constantContainer.skillLevelList[Random.Range(0, 6)];
        }

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}