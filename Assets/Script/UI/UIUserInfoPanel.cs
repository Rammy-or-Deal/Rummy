using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUserInfoPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public Image mUserPic;
    public Text mUserName;
    public Text mUserLevel;
    
    // Update is called once per frame
    public void UpdateValue()
    {
        mUserName.text = DataController.Inst.userInfo.name;
        mUserLevel.text = DataController.Inst.userInfo.skill_level;
    }
}
