using System;
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

    internal void Init(List<FortuneUserSeat> m_playerList)
    {
        frontTxt.text = "";
        middleTxt.text = "";
        backTxt.text = "";
        totalTxt.text = "";
        tableTaxTxt.text = "";

        for (int i = 0; i < m_playerList.Count; i++)
        {
            LogMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_playerList[i].IsSeat);
            players[i].Init(m_playerList[i]);
        }

    }

    internal void SetProperty(UICalcDialog calcDlg)
    {
        var m_calc_player = calcDlg.m_calc_player;

        for (int i = 0; i < m_calc_player.Count; i++)
        {
            LogMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_calc_player[i].IsSeat);
            players[i].SetProperty(m_calc_player[i]);
        }

        frontTxt.text = calcDlg.m_FrontText.text;
        middleTxt.text = calcDlg.m_MiddleText.text;
        backTxt.text = calcDlg.m_BackText.text;
        if (m_calc_player[0].totalCoin > 0)
        {
            totalTxt.text = "Total :" + (m_calc_player[0].totalCoin * 0.9);
            tableTaxTxt.text = "Table Tax(10%):" + (m_calc_player[0].totalCoin * 0.1);            
            totalTxt.color = Color.green;
        }
        else
        {
            tableTaxTxt.text = "Table Tax(10%): 0";
            tableTaxTxt.color = Color.yellow;
            totalTxt.text = "Total :" + (m_calc_player[0].totalCoin);
            totalTxt.color =  Color.red;
        }
    }
}