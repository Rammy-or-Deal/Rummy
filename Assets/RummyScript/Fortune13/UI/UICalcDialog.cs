using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICalcDialog : MonoBehaviour
{
    // Start is called before the first frame update
    List<UIFCalcPlayer> m_calc_player;
    public Text m_CardLineText;
    void Start()
    {
        
        m_calc_player = new List<UIFCalcPlayer>();
        var list = GetComponentsInChildren<UIFCalcPlayer>();
        foreach(var player in list)
        {
            m_calc_player.Add(player);
        }
        if(m_CardLineText == null)
            m_CardLineText = GetComponentsInChildren<Text>(true).Where(x=>x.gameObject.name == "ImageText").First();

        LogMgr.Inst.Log("Calc Dialog is started. cardLineText=" + m_CardLineText.text);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    internal void ShowCards(FortuneUserCardList user, List<Card> showList)
    {
        try{
            m_calc_player.Where(x=>x.actorNumber == user.actorNumber).First().ShowCards(showList);
        }catch{}
    }

    internal void Init(List<FortuneUserSeat> m_playerList)
    {
        m_CardLineText.text = "";
        LogMgr.Inst.Log("Calc Dialog Init is called. cardLineText=" + m_CardLineText.text);
        LogMgr.Inst.Log("Calc Dialog Init is called. playerCount="+m_playerList.Count);
        for(int i = 0; i < m_playerList.Count; i++)
        {
            LogMgr.Inst.Log("calcPlayer["+i+"].IsSet=" + m_playerList[i].IsSeat);
            m_calc_player[i].Init(m_playerList[i]);
        }
    }
}
