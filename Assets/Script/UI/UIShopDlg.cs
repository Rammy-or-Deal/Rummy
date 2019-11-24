using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopDlg : MonoBehaviour
{
    // Start is called before the first frame update
    public UIShopItem[] goldItems;
    public UIShopItem[] leafItems;
    public UIShopFrameItem[] frameItems;
    void Start()
    {
        for (int i = 0; i < goldItems.Length; i++)
        {
            goldItems[i].mPic.sprite = Resources.Load<Sprite>("new_item/shop_item_gold_" + (i + 1).ToString());
            goldItems[i].mPrice.text = "$"+((i + 1) * 0.99);
            goldItems[i].mItemValue.text = (i + 1).ToString() +" × 10k";
        }

        for (int i = 0; i < leafItems.Length; i++)
        {
            leafItems[i].mPic.sprite = Resources.Load<Sprite>("new_item/shop_item_leaf_" + (i + 1).ToString());
            leafItems[i].mPrice.text = "$"+((i + 1) * 0.99);
            leafItems[i].mItemValue.text = ((i + 1) * 10).ToString();
        }
    }

    public void OnClose()
    {
        Debug.Log("close click");
        gameObject.SetActive(false);
    }

    public void OnCollectionClick()
    {
        UIController.Inst.collectionDlg.gameObject.SetActive(true);
    }

    public void OnExchageClick()
    {
        UIController.Inst.exchangeDlg.gameObject.SetActive(true);
    }
}
