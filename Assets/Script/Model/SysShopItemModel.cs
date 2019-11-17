using System;
using UnityEngine.UI;

namespace RummyScript.Model
{
    [Serializable]
    public class SysShopItemModel

    {
    public int id;
    public string pic;
    public int value;
    public float price;
    

    public override string ToString()
    {
        return UnityEngine.JsonUtility.ToJson(this, true);
    }
    }
}

