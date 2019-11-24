using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    public int id;

    public Text mItemValue;
    public Image mPic;
    public Text mPrice;
    public Image mMoneyPic;
    
    // Start is called before the first frame update
    void Start()
    {
            
    }

    public void OnClickBuy()
    {
        Debug.Log(id+" item clicked ");
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }
    
}
