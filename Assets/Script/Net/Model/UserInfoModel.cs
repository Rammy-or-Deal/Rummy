﻿using System;
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
        public string pic;
        
//        public string remark;
        
//        public string starPic;
//        public string starValue;
        
        public int coin_value;
        public int leaf_value;
        
        public string skill_level;
        
        public int frame_id;
        public string udid;
        
        

        public UserInfoModel()
        {   
            coin_value = 2500;
            leaf_value = 300;
            frame_id = 3;
        }

        public void Init(string prefixName="Guest")
        {
            name = prefixName+"["+Random.Range(1000,9999).ToString() + "]";
            pic = "new_avatar/avatar_" + Random.Range(1,26).ToString();
            coin_value = Random.Range(50000,99999);
            leaf_value = Random.Range(100,999);
            skill_level = Constant.skillLevelList[Random.Range(0, 6)];
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}