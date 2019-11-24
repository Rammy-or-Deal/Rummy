using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class SysExchangeItemModel
    
    {
        public int id;
        public string pic;
        public string moneyType;
        public int value;
        public int price;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}