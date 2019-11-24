using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class FrameItemModel
    {
        public int id;  
        public int itemId;
        public string moneyType; 
        public int price;
        public string stats;
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}