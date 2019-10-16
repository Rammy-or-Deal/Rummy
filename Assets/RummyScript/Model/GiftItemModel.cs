using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class GiftItemModel
    {
        public int id;  
        public int itemId;
        
        public string moneyType;
        public int price;
        public int count;
        
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}