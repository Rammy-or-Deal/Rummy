using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UICalcDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public List<UIFCalcPlayer> m_calc_player;
    public Text m_CardLineText;

    public Text m_FrontText;
    public Text m_MiddleText;
    public Text m_BackText;
    public Text m_TotalText;


    void Start()
    {

        m_calc_player = new List<UIFCalcPlayer>();
        var list = GetComponentsInChildren<UIFCalcPlayer>();
        foreach (var player in list)
        {
            m_calc_player.Add(player);
        }
        if (m_CardLineText == null)
            m_CardLineText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "ImageText").First();

        LogMgr.Inst.Log("Calc Dialog is started. cardLineText=" + m_CardLineText.text);

        m_FrontText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "FrontText").First();
        m_MiddleText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "MiddleText").First();
        m_BackText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "BackText").First();
        m_TotalText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "TotalText").First();

    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    internal void ShowCards(FortuneUserCardList user, List<Card> showList)
    {
        try
        {
            m_calc_player.Where(x => x.actorNumber == user.actorNumber).First().ShowCards(showList);
        }
        catch { }
    }

    internal void Init(List<UserSeat> m_playerList)
    {
        m_FrontText.text = "";
        m_MiddleText.text = "";
        m_BackText.text = "";
        m_TotalText.text = "";
        
        m_CardLineText.text = "";
        LogMgr.Inst.Log("Calc Dialog Init is called. cardLineText=" + m_CardLineText.text);
        LogMgr.Inst.Log("Calc Dialog Init is called. playerCount=" + m_playerList.Count);
        for (int i = 0; i < m_playerList.Count; i++)
        {
            
            LogMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_playerList[i].isSeat);

            m_calc_player[i].Init(m_playerList[i]);
        }

        //Sample 
        /*
        m_calc_player[3].IsSeat = true;
        List<Card> cards3 = new List<Card>();
        cards3.Add(new Card(4, 8));
        cards3.Add(new Card(2, 2));
        cards3.Add(new Card(7, 5));
        cards3.Add(new Card(3, 3));
        cards3.Add(new Card(9, 1));
        m_calc_player[3].ShowCards(cards3);

        m_calc_player[2].IsSeat = true;
        List<Card> cards2 = new List<Card>();
        cards2.Add(new Card(2, 0));
        cards2.Add(new Card(2, 1));
        cards2.Add(new Card(2, 2));
        cards2.Add(new Card(2, 3));
        cards2.Add(new Card(1, 0));
        m_calc_player[2].ShowCards(cards2);

        List<Card> cards1 = new List<Card>();
        m_calc_player[1].IsSeat = true;
        cards1.Add(new Card(10, 0));
        cards1.Add(new Card(3, 1));
        cards1.Add(new Card(5, 2));
        cards1.Add(new Card(5, 3));
        cards1.Add(new Card(3, 1));
        m_calc_player[1].ShowCards(cards1);
        */
    }

    internal void showLineLabel(int lineNo)
    {
        m_CardLineText.gameObject.SetActive(true);
        switch (lineNo)
        {
            case 0:
                m_CardLineText.text = "Front Hand";
                break;
            case 1:
                m_CardLineText.text = "Middle Hand";
                break;
            case 2:
                m_CardLineText.text = "Back Hand";
                break;
        }
        LogMgr.Inst.Log("Card Line=" + m_CardLineText.text);
    }

    internal async void SendReceiveCoin(int lineNo)
    {
        foreach (var player in m_calc_player.Where(x => x.IsSeat == true))
        {
            player.Coin = 0;
            player.SetCardType();
        }
        //UIFCalcPlayer

        foreach (var srcPlayer in m_calc_player.Where(x => x.IsSeat == true))
        {
            foreach (var tarPlayer in m_calc_player.Where(x => x.IsSeat == true && x.Score > srcPlayer.Score))
            {
                //if (srcPlayer.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber || tarPlayer.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                srcPlayer.SendCoin(tarPlayer, 100);
            }
        }      


        await Task.Delay(2000);
        int curCoin = m_calc_player[0].Coin;
        int myCoin = m_calc_player[0].totalCoin;
        LogMgr.Inst.Log(string.Format("{0} line. CurCoin={1}, TotalCoin={2}", lineNo, curCoin, myCoin));
        await Task.Delay(1000);
        try{
        switch (lineNo)
        {
            case 0:
                m_FrontText.text = string.Format("Front\t: {0}", curCoin);
                m_TotalText.text = string.Format("Total \t: {0}", myCoin);                

                break;
            case 1:
                m_MiddleText.text = string.Format("Middle\t: {0}", curCoin);
                break;
            case 2:
                m_BackText.text = string.Format("Back  \t: {0}", curCoin);
                break;
        }
        }catch{}
    }
}
