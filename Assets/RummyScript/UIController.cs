using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Inst;

    public UIShopDlg shopDlg;
    
    public UIChatDialog chatDlg;
    public UIEmailDialog emailDlg;
    public UIEventDialog eventDlg;
    public UIFriendMenu friendMenu;
    public UILuckySpinDialog spinDlg;
    public UINoticeDialog noticeDlg;
    public UIRewardDialog rewardDlg;
    public UISettingDialog settingDlg;
    public UIChallengeDialog challengeDlg;
    public UIExchangeDialog exchangeDlg;
    public UIUserInfoMenu userInfoMenu;
    public UIMoneyPanel moneyPanel;
    public UIUserInfoPanel userInfoPanel;
    public UIAvatarDialog avatarDlg;
    public UIAlertDialog alertDlg;
    public UILoadingDialog loadingDlg;
    public UICollectionDialog collectionDlg;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Inst)
            Inst= this;
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
