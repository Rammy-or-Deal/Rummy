using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class UIResultDialog : MonoBehaviour
{
    public UIFResultPlayer[] players;
    public Text frontTxt;
    public Text middleTxt;
    public Text backTxt;
    public Text totalTxt;
    public Text tableTaxTxt;
    public Text luckyTxt;
    public Text RestartMessageText;
    void Start()
    {
        RestartMessageText = GetComponentsInChildren<Text>(true).Where(x => x.gameObject.name == "RestartMessageTex").First();
    }

    public void OnCloseBtn()
    {
        try
        {
            StopCoroutine(exitEvent);
        }
        catch { }
        this.gameObject.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
    public void OnRestartBtn()
    {
        try
        {
            StopCoroutine(exitEvent);
        }
        catch { }
        this.gameObject.SetActive(false);
    }
    internal void Init(List<UserSeat> m_playerList)
    {
        frontTxt.text = "";
        middleTxt.text = "";
        backTxt.text = "";
        totalTxt.text = "";
        tableTaxTxt.text = "";

        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (m_playerList[i].isSeat == false) continue;
            LogMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_playerList[i].isSeat);
            players[i].Init(m_playerList[i]);
        }

    }

    internal void SetProperty(UICalcDialog calcDlg)
    {
        var m_calc_player = calcDlg.m_calc_player;

        int luckyCoin = AddLuckyBonus(calcDlg);
        if (luckyCoin != 0)
        {
            luckyTxt.gameObject.SetActive(true);
            luckyTxt.text =  "Lucky :" + luckyCoin.ToString();
        }
        else
        {
            luckyTxt.gameObject.SetActive(false);
        }

        for (int i = 0; i < m_calc_player.Count; i++)
        {            
            players[i].SetProperty(m_calc_player[i]);
            GameMgr.Inst.Log("calcPlayer[" + i + "].IsSet=" + m_calc_player[i].IsSeat+", gold="+m_calc_player[i].totalCoin, enumLogLevel.FortuneLuckyLog);
            if (PhotonNetwork.IsMasterClient)
                GameMgr.Inst.seatMgr.AddGold(m_calc_player[i].actorNumber, m_calc_player[i].totalCoin);
        }

        frontTxt.text = calcDlg.m_FrontText.text;
        middleTxt.text = calcDlg.m_MiddleText.text;
        backTxt.text = calcDlg.m_BackText.text;

        if (m_calc_player[0].totalCoin > 0)
        {
            totalTxt.text = "Total :" + (m_calc_player[0].totalCoin * 0.9);
            tableTaxTxt.text = "TableTax: " + (m_calc_player[0].totalCoin * 0.1);
            totalTxt.color = Color.green;
        }
        else
        {
            tableTaxTxt.text = "TableTax: 0";
            tableTaxTxt.color = Color.yellow;
            totalTxt.text = "Total :" + (m_calc_player[0].totalCoin);
            totalTxt.color = Color.red;
        }
    }

    private int AddLuckyBonus(UICalcDialog calcDlg)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        if (pList.m_playerList.Count(x => x.m_status == enumPlayerStatus.Fortune_Lucky) == 0) return 0;
        var m_calc_player = calcDlg.m_calc_player;
        var m_lucky_player = FortuneUIController.Inst.luckyDlg.players;
        int myCoin = 0;
        foreach (var player in pList.m_playerList)
        {
            if (m_calc_player.Count(x => x.actorNumber == player.m_actorNumber) == 0) continue;
            if (m_lucky_player.Count(x => x.actorNumber == player.m_actorNumber) == 0) continue;
            var uiCalcPlayer = m_calc_player.Where(x => x.actorNumber == player.m_actorNumber).First();
            var uiLuckyPlayer = m_lucky_player.Where(x => x.actorNumber == player.m_actorNumber).First();
            GameMgr.Inst.Log("Origianl Coin actor="+player.m_actorNumber+", coin:=" + uiCalcPlayer.totalCoin, enumLogLevel.FortuneLuckyLog);
            uiCalcPlayer.totalCoin += int.Parse(uiLuckyPlayer.resultText.text);            
            GameMgr.Inst.Log("Penalty from Lucky. actor="+player.m_actorNumber+", penalty:=" + int.Parse(uiLuckyPlayer.resultText.text), enumLogLevel.FortuneLuckyLog);
            GameMgr.Inst.Log("Result Coin actor="+player.m_actorNumber+", coin:=" + uiCalcPlayer.totalCoin, enumLogLevel.FortuneLuckyLog);
            if (player.m_actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                myCoin = int.Parse(uiLuckyPlayer.resultText.text);
            }
        }
        return myCoin;
    }

    Coroutine exitEvent;
    IEnumerator ShowTimeForExit(int waitTime)
    {
        while (waitTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            RestartMessageText.text = string.Format("Will leave in {0}sec", waitTime);
            waitTime--;
        }
        //OnRestartBtn();
        OnCloseBtn();
    }

    internal void ShowResult()
    {
        this.gameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnFinishedGame}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        exitEvent = StartCoroutine(ShowTimeForExit(Constants.FortuneWaitTimeForLeave));
    }
}