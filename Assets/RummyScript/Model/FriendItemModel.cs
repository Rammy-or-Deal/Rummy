using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class FriendItemModel
    {
        public int id;  
        public int userId;
        public string onLineStats;
        
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}