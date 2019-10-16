using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    // values
    public GameObject collectionDlg;
    public GameObject avatarDlg;
    public GameObject chatDlg;
    public GameObject emailDlg;
    public GameObject eventDlg;
    public GameObject friendMenu;
    public GameObject spinDlg;
    public GameObject noticeDlg;
    public GameObject rewardDlg;
    public GameObject settingDlg;
    public GameObject shopDlg;
    public GameObject userInfoMenu;
    
    void Start()
    {
//        username.text = "CaiXian";
//        userImage.sprite=Resources.Load<Sprite>("new_avatar/avatar_2");
    }

    
    public void OnClickCollectionsBtn()
    {
        Debug.Log("gift click");
        collectionDlg.SetActive(true);
    }

    public void OnClickCamera()
    {
        Debug.Log("camera click");
        avatarDlg.SetActive(true);
    }

    public void OnClickChatBtn()
    {
        chatDlg.SetActive(true);
    }
    public void OnClickEmailBtn()
    {
        emailDlg.SetActive(true);
    }
    public void OnClickEventBtn()
    {
        eventDlg.SetActive(true);
    }
    public void OnClickFriendBtn()
    {
        friendMenu.SetActive(true);
    }
    public void OnClickSpinBtn()
    {
        spinDlg.SetActive(true);
    }
    public void OnClickRewardBtn()
    {
        rewardDlg.SetActive(true);
    }
    public void OnClickSettingBtn()
    {
        settingDlg.SetActive(true);
    }
    public void OnClickShopBtn()
    {
        shopDlg.SetActive(true);
    }
    public void OnClickUserInfoPanel()
    {
        userInfoMenu.SetActive(true);
    }

}
    