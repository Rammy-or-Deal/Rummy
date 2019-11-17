using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class SysItemModel
    {
        public int id;
        public string name;
        public string pic;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}