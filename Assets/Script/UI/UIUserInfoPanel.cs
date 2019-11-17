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
    
    void Start()
    {
//        mUserPic.sprite = Resources.Load<Sprite>("new_avatar/avatar_4");
//        mUserName.text = "Chai";
//        mUserLevel.text = "Novice";
        UpdateValue();
    }

    // Update is called once per frame
    public void UpdateValue()
    {
        mUserPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
        mUserName.text = DataController.Inst.userInfo.name;
        mUserLevel.text = DataController.Inst.userInfo.skillLevel;
    }
}
