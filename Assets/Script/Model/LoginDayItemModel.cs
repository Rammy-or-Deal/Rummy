using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class LoginDayItemModel
    {
        public int id;  
        public string pic;
        public string stats;
        public int price;
        
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}

