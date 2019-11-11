using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultDialog : MonoBehaviour
{
    public UIFResultPlayer[] players;
    public Text frontTxt;
    public Text middleTxt;
    public Text backTxt;
    public Text totalTxt;
    public Text tableTaxTxt;
    
    void Start()
    {
        
    }

    public void OnCloseBtn()
    {
        this.gameObject.SetActive(false);
    }

}
