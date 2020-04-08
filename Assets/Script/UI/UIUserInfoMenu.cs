using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUserInfoMenu : MonoBehaviour
{
    public UIGiftItem[] giftItems;
    public GameObject seasonalDlg;
    public GameObject gameStatsDlg;
    public GameObject userInfoDlg;
    
    public Image mFramePic;
    public Text mUserName;
    public Image mUserPic;
    public Text mWinRate;
//    public Text mRemark;

    public Image mSkillPic;
    public Text mSkillLevel;
    public Text mSkillValue;

    public Image mStarPic;
    public Text mStarValue;
    public Image mCoinPic;
    public Text mCoinValue;
    public Image mLeafPic;
    public Text mLeafValue;

    private Sprite _mCoinPic;
    private Sprite _mLeafPic;

    // Start is called before the first frame update
    void Start()
    {
        
        _mCoinPic = Resources.Load<Sprite>("new_symbol/coin");
        _mLeafPic = Resources.Load<Sprite>("new_symbol/leaf");
        
        mFramePic.sprite = Resources.Load<Sprite>("new_frame/frame_legend");
        mWinRate.text = "50%(100/200)";

        
        for (int i = 0; i<giftItems.Length; i++)
        {
            Sprite symbolPic = Resources.Load<Sprite>("new_symbol/gift_" + (i + 1).ToString());
            if (i < (giftItems.Length)/2)
            {
                giftItems[i].mMoneyType = "coin";
                giftItems[i].mMoneyPic.sprite = _mCoinPic;
                giftItems[i].mMoneyValue.text = ((i + 1) * 100).ToString();
            }
            else
            {
                giftItems[i].mMoneyType = "leaf";
                giftItems[i].mMoneyPic.sprite = _mLeafPic;
                giftItems[i].mMoneyValue.text = ((i + 1) * 5).ToString();
            }

            giftItems[i].mSymbolPic.sprite = symbolPic;
            giftItems[i].mSymbolCount.text = "×" + " " + (i * 3).ToString();

        }

    }

    public void UpdateUserInfoMenu()
    {
        mUserName.text = DataController.Inst.userInfo.name;
        mSkillLevel.text = DataController.Inst.userInfo.skill_level;
    }
    
    public void OnClose(GameObject obj)
    {
        Debug.Log("close click");
        obj.SetActive(false);
    }

    public void OnClickSeasonBtn(GameObject obj)
    {
        Debug.Log("season click");
        obj.SetActive(true);
    }

    public void OnClickGameStatsBtn(GameObject obj)
    {
        Debug.Log("gamestats click");
        obj.SetActive(true);
    }
    public void OnClickCollectionBtn(GameObject obj)
    {
        Debug.Log("OnClickCollectionBtn click");
        obj.SetActive(true);
    }

    public void OnClickCameraBtn(GameObject obj)
    {
        Debug.Log("OnClickChangeAvatar click");
        obj.SetActive(true);
    }

}
