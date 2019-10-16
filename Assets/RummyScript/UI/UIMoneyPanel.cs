﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyPanel : MonoBehaviour
{

    public Image mCoinPic;
    public Text mCoinValue;
    public Image mLeafPic;
    public Text mLeafValue;

    // Start is called before the first frame update
    void Start()
    {
//        mCoinPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.coinPic);
//        mLeafPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.leafPic);
        UpdateValue();
    }

    public void UpdateValue()
    {
        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
        mLeafValue.text = DataController.Inst.userInfo.leafValue.ToString();
    }

    // Update is called once per frame
    public void OnClick()
    {
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }
}
