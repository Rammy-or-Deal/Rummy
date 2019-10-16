using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGiftItem : MonoBehaviour
{
    public int id;
    public Image mMoneyPic;
    public Text mMoneyValue;
    public Image mSymbolPic;
    public Text mSymbolCount;
    public string mMoneyType = "coin";
    // Start is called before the first frame update
    
    public void OnClickBuy()
    {
        Debug.Log(id+" item clicked ");
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }
}
