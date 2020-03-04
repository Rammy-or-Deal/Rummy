using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class SysSkillModel
    {
        public int id;
        public string name;
        public string pic;
        public int max;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}