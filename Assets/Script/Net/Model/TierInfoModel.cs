using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class TierInfoModel
    {
        public int id;
        public int firstPlace;
        public int secondPlace;
        public int thirdPlace;
        public int lastPlace;
        public int game;
        public int ace;
        public int joker;
        public int min;
        public int max;
        public TierInfoModel()
        {
        }

        public void Init()
        {
        }
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}