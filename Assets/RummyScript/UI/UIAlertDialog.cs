using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UIAlertDialog : MonoBehaviour
{
    public static UIAlertDialog Inst;
    
    public delegate void DlgReplyEvent();

    public GameObject[] icons;
    public Text text;
    public Text coinVal;
    private DlgReplyEvent _mEvent;
    
    // Start is called before the first frame update

    private void Awake()
    {
        if (!Inst) Inst = this;
    }

    public void Show(Game_Identifier game, DlgReplyEvent mEvent, string mText, int coinVal0 = 0)
    {
        _mEvent = mEvent;
        text.text = mText;
        coinVal.gameObject.SetActive(coinVal0!=0);
        coinVal.text = coinVal0.ToString();
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }
        switch (game)
        {
            case Game_Identifier.Lami:
                icons[0].SetActive(true);                
                break;
            case Game_Identifier.Baccarat:
                break;
            case Game_Identifier.Fortune13:
                icons[1].SetActive(true);
                break;
        }
        gameObject.SetActive(true);
    }

    void Start()
    {
    }

    public void OnClickYesBtn()
    {
        gameObject.SetActive(false);
        if (_mEvent!=null)
            _mEvent();
    }

    public void OnClickNoBtn()
    {
        gameObject.SetActive(false);
    }
}