using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class RequestItemModel
    {
        public int id;  
        public int friendId;
        
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}