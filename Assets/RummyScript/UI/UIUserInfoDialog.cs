using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUserInfoDialog : MonoBehaviour
{
    public UIGiftItem[] giftItems;
    
    public Image mFramePic;
    public Text mUserName;
    public Image mUserPic;
    public Text mWinRate;
    public Text mRemark;

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
        
        _mCoinPic = Resources.Load<Sprite>("new_symbol/coin_m");
        _mLeafPic = Resources.Load<Sprite>("new_symbol/leaf");
        
        mFramePic.sprite = Resources.Load<Sprite>("new_frame/frame_legend");
        mUserPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
        mUserName.text = "Chai[54545]";
        mWinRate.text = "50%(100/200)";
        mRemark.text = "All is well";
        mSkillPic.sprite = Resources.Load<Sprite>("new_skill/skill_4");
        mSkillLevel.text = "Legend" + " × " + "3";
        mSkillValue.text = "200/1000";
        mStarPic.sprite = Resources.Load<Sprite>("new_symbol/star");
        mStarValue.text = "1000";
        mCoinPic.sprite = Resources.Load<Sprite>("new_symbol/coin");
        mCoinValue.text = "20000";
        mLeafPic.sprite = _mLeafPic;
        mLeafValue.text = "300";
        
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

    // Update is called once per frame
    void Update()
    {
        
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

    public void OnClickGiftBtn(GameObject obj)
    {
        Debug.Log("gift click");
        obj.SetActive(true);
    }
}
