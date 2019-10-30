using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class BaccaratPanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratPanMgr Inst;
    public Text m_panTime;
    public GameObject m_panClock;
    public UIBBetPanel betPanel;
    public UIBCardPanel cardPanel;

    public TeamCard bankerCard = new TeamCard();
    public TeamCard playerCard = new TeamCard();
    void Start()
    {
        if (!Inst)
            Inst = this;
        if (PhotonNetwork.IsMasterClient)
        {
            StartNewPan();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal async void StartNewPan()
    {
        await Task.Delay(1000);
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Hashtable prop = new Hashtable{
                {Common.PLAYER_BETTING_LOG, ""},
                {Common.NOW_BET, ""},
            };
            player.SetCustomProperties(prop);
        }

        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnStartNewPan},
            {Common.BACCARAT_CURRENT_TIME, Constants.BaccaratCurrentTime}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnStartNewPan()
    {
        m_panClock.gameObject.SetActive(true);
        StartCoroutine(WaitFor1Second());
    }

    public IEnumerator WaitFor1Second()
    {
        int time = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CURRENT_TIME];
        m_panTime.text = time + "";
        yield return new WaitForSeconds(1.0f);
        time--;
        if (PhotonNetwork.IsMasterClient)
        {
            if (time >= 0)
            {
                Hashtable table = new Hashtable{
                    {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnPanTimeUpdate},
                    {Common.BACCARAT_CURRENT_TIME, time}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
            else
            {
                Hashtable table = new Hashtable{
                    {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnEndPan},
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
        }
    }

    internal void OnShowingVictoryArea()
    {
        var areaList = ((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_VICTORY_AREA]).Split(',').Select(Int32.Parse).ToList();

        foreach (var area in areaList)
        {
            betPanel.pans[area].winObj.gameObject.SetActive(true);
        }
    }

    internal void OnPanTimeUpdate()
    {
        m_panClock.gameObject.SetActive(true);
        StartCoroutine(WaitFor1Second());
    }

    internal void OnPlayerBet(float x, float y, int moneyId, int areaId)
    {
        LogMgr.Inst.Log(string.Format("Player Bet. x={0}, y={1}, moneyId={2}, areaId={3}", x, y, moneyId, areaId), (int)LogLevels.PanLog);
        betPanel.OnPlayerBet(x, y, moneyId, areaId);
    }

    internal void OnCatchedCardDistributed()
    {
        InitTeamCard();

        bankerCard.cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_BANKER];
        playerCard.cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_PLAYER];

        LogMgr.Inst.Log("Card Distributed. banker:=" + bankerCard.cardString + ",  player:=" + playerCard.cardString, (int)LogLevels.PlayerLog1);

        // Here, we can add the code to make the users can open the card. player is depended on 
        // {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
        // {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},

        if (!PhotonNetwork.IsMasterClient) return;
        ShowingCardRoutine = StartCoroutine(ShowingCard((int)BaccaratShowingCard_NowTurn.Player1));
    }
    Coroutine ShowingCardRoutine;
    IEnumerator ShowingCard(int nowTurn)
    {
        yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime);
        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnShowingCatchedCard},
            {Common.BACCARAT_NOW_SHOWING_TURN, nowTurn}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnShowingCatchedCard()
    {
        //BACCARAT_NOW_SHOWING_TURN
        int nowTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_NOW_SHOWING_TURN];

        // Here, we can add the code to make the users can open the card. player is depended on 
        // {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
        // {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},        
        ShowingCatchedCard(nowTurn);

        if (!PhotonNetwork.IsMasterClient) return;

        int limit = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_NOW_SHOWING_LIMIT];

        try
        {
            StopCoroutine(ShowingCardRoutine);
        }
        catch { }

        nowTurn++;
        LogMgr.Inst.Log("New Card command Created. id=" + (BaccaratShowingCard_NowTurn)nowTurn, (int)LogLevels.PlayerLog1);

        if (nowTurn > (int)BaccaratShowingCard_NowTurn.Banker2)
        {
            if (nowTurn == (int)BaccaratShowingCard_NowTurn.Player3)
                if (playerCard.CardList.Count > 2)
                    ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
                else
                    nowTurn++;

            if (nowTurn == (int)BaccaratShowingCard_NowTurn.Banker3)
                if (bankerCard.CardList.Count > 2)
                    ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
                else
                    BaccaratBankerMgr.Inst.CalcResult();
                    
            if (nowTurn > (int)BaccaratShowingCard_NowTurn.Banker3)
                BaccaratBankerMgr.Inst.CalcResult();
        }
        else
        {
            ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
        }
        //if (nowTurn == (int)BaccaratShowingCard_NowTurn.Player3 && playerCard.CardList.Count > 2)
    }

    private void ShowingCatchedCard(int nowTurn)
    {
        LogMgr.Inst.Log("Card showing command called. id=" + (BaccaratShowingCard_NowTurn)nowTurn, (int)LogLevels.PlayerLog1);
        switch (nowTurn)
        {
            case (int)BaccaratShowingCard_NowTurn.Player1:
                cardPanel.leftCards[0].ShowImage(playerCard.CardList[0].num, playerCard.CardList[0].color);
                break;
            case (int)BaccaratShowingCard_NowTurn.Player2:
                cardPanel.leftCards[1].ShowImage(playerCard.CardList[1].num, playerCard.CardList[1].color);
                break;
            case (int)BaccaratShowingCard_NowTurn.Player3:
                if (playerCard.CardList.Count > 2)
                    cardPanel.leftCards[2].ShowImage(playerCard.CardList[2].num, playerCard.CardList[2].color);
                break;
            case (int)BaccaratShowingCard_NowTurn.Banker1:
                cardPanel.rightCards[0].ShowImage(bankerCard.CardList[0].num, bankerCard.CardList[0].color);
                break;
            case (int)BaccaratShowingCard_NowTurn.Banker2:
                cardPanel.rightCards[1].ShowImage(bankerCard.CardList[1].num, bankerCard.CardList[1].color);
                break;
            case (int)BaccaratShowingCard_NowTurn.Banker3:
                if (bankerCard.CardList.Count > 2)
                    cardPanel.rightCards[2].ShowImage(bankerCard.CardList[2].num, bankerCard.CardList[2].color);
                break;
        }

    }

    private void InitTeamCard()
    {
        bankerCard.CardList.Clear();
        playerCard.CardList.Clear();
    }
}