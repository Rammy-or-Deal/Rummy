using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NoticeType
{
    Reward,
    ShopGoldItem,
    ShopLeafItem,
    ShopExchangeItem,
    ShopGiftItem,
    ShopFrameItem,
    CollectionFrameItem,
    CollectionGiftItem,
    LoginDayItem,
    ChatSend,
    None
}
public class UINoticeDialog : MonoBehaviour
{
    public Text mView;
    // Start is called before the first frame update
    void Start()
    {
        mView.text = "If you watch video 30s, you can earn double!";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    public void OnClose(GameObject obj)
    {
        Debug.Log("close click");
        obj.SetActive(false);
        mView.text = "If you watch video 30s, you can earn double!";
    }
    public void OnClickConfirm()
    {
        mView.text = "You earn 1000 Gold !";
    }
    public void OnClickCancecl()
    {
        mView.text = "You earn 500 Gold !";
    }
    
    private string GetNoticeTypeString(NoticeType notice)
    {
        string msg = "";
        switch (notice)
        {
            
            case NoticeType.Reward:
                msg =  "If you watch video 30s, you can earn double!";
                break;
            case NoticeType.ShopGoldItem:
                msg =  "Do you want to buy Gold ?";
                break;
            case NoticeType.ShopLeafItem:
                msg =  "Do you want to buy Leaf ?";
                break;
            case NoticeType.ShopExchangeItem:
                msg =  "Do you want to Exchange ?";
                break;
            case NoticeType.ShopGiftItem:
                msg =  "Do you want to buy Gift ?";
                break;
            case NoticeType.ShopFrameItem:
                msg =  "Do you want to buy Frame ?";
                break;
        }
        
        return msg;
    }
}
