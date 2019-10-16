using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICollectionDialog : MonoBehaviour
{
    public UICollectionFrameItem[] frameItems;
    public UIGiftItem[] giftItems;
    private Sprite _mCoinPic;
    private Sprite _mLeafPic;
    // Start is called before the first frame update
    void Start()
    {
        
        _mCoinPic = Resources.Load<Sprite>("new_symbol/coin_m");
        _mLeafPic = Resources.Load<Sprite>("new_symbol/leaf");
        
        for (int i = 0; i < frameItems.Length; i++)
        {
            frameItems[i].frame.sprite = Resources.Load<Sprite>("new_frame/frame_legend");
            frameItems[i].name.text = "Default" + i;
            frameItems[i].pic.sprite = Resources.Load<Sprite>("new_key/key_btn_" + frameItems[i].stats);
        }
        
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
        obj.SetActive(false);
    }
}
