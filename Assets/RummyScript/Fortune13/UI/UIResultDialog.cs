﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCloseBtn()
    {
        this.gameObject.SetActive(false);
    }

    internal void Init(List<FortuneUserSeat> playerList)
    {
/*        m_FrontText.text = "";
        m_MiddleText.text = "";
        m_BackText.text = "";
        m_TotalText.text = "";
        myCoin = 0;

        m_CardLineText.text = "";
        LogMgr.Inst.Log("Calc Dialog Init is called. cardLineText=" + m_CardLineText.text);
        LogMgr.Inst.Log("Calc Dialog Init is called. playerCount=" + m_playerList.Count);
  */
        for (int i = 0; i < m_playerList.Count; i++)
        {
            LogMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_playerList[i].IsSeat);
            m_calc_player[i].Init(m_playerList[i]);
        }
    }
}
