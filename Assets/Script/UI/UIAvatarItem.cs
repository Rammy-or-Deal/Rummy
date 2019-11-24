using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatarItem : MonoBehaviour
{
    public int id;
    public Image pic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AvatarClick()
    {
        DataController.Inst.userInfo.pic = "new_avatar/avatar_" + (id + 1).ToString();
        UIController.Inst.userInfoPanel.mUserPic.sprite = Resources.Load<Sprite>("new_avatar/avatar_" + (id + 1));
        UIController.Inst.userInfoMenu.UpdateUserInfoMenu();
        UIController.Inst.avatarDlg.gameObject.SetActive(false);
    } 
}
