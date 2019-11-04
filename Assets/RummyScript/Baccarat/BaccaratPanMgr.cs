using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Unity.Collections;
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
    public UIBMessage message;

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

        //StartCoroutine(TestMessage());
    }
    /*
        IEnumerator TestMessage()
        {
            yield return new WaitForSeconds	(1);
            message	.Show	("Baccarat Betting Ends");
            yield return new WaitForSeconds	(1);
            message.Hide	();
        }
    */
    internal void StartNewPan()
    {

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

    public async void OnEndPan()
    {
        message.Show("Betting Finished.");
        await Task.Delay(3000);
        message.Hide();
    }

    internal async void OnStartNewPan()
    {
        message.Show("Baccarat Betting Started.");
        await Task.Delay(3000);
        message.Hide();

        try
        {
            m_panClock.gameObject.SetActive(true);
        }
        catch { }
        betPanel.Init();
        cardPanel.Init();
        StartCoroutine(WaitFor1Second());
    }

    public IEnumerator WaitFor1Second()
    {
        int time = -100;
        try
        {
            time = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CURRENT_TIME];
        }
        catch
        {
            
        }
        m_panTime.text = time + "";
        yield return new WaitForSeconds(1.0f);
        time--;
        if (PhotonNetwork.IsMasterClient && time >= -1)
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

    internal void OnInitUI()
    {

    }

    internal async void OnPrizeAwarded()
    {
        var prize_area = (string)PhotonNetwork.LocalPlayer.CustomProperties[Common.BACCARAT_PRIZE_AREA];

        message.Show("Congratulations!");
        await Task.Delay(3000);
        message.Hide();
        foreach (var area in prize_area.Split(','))
        {
            var areaId = int.Parse(area.Split(':')[0]);
            var prize = int.Parse(area.Split(':')[1]);
            betPanel.pans[areaId].SetPrize(prize);
        }
    }

    internal void OnShowingVictoryArea()
    {
        var areaList = ((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_VICTORY_AREA]).Split(',').Select(Int32.Parse).ToList();

        foreach (var area in areaList)
        {
            betPanel.pans[area].winObj.gameObject.SetActive(true);
        }

        // Showing Historical chart.
        UpdateHistoryPan(areaList[0]);
    }

    private void UpdateHistoryPan(int v)
    {
        UIBHistory.Inst.AddCell(v);
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
        var max_betting_banker = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_BANKER];
        var max_betting_player = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER];

        MoveDistributedCardToPlayer(max_betting_banker, max_betting_player);

        LogMgr.Inst.Log("Card Distributed. banker:=" + bankerCard.cardString + ",  player:=" + playerCard.cardString, (int)LogLevels.PlayerLog1);

        // Here, we can add the code to make the users can open the card. player is depended on 
        // {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
        // {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},

        if (!PhotonNetwork.IsMasterClient) return;
        ShowingCardRoutine = StartCoroutine(ShowingCard((int)BaccaratShowingCard_NowTurn.Player1));
    }



    private void MoveDistributedCardToPlayer(int max_betting_banker, int max_betting_player)
    {
        BaccaratUserSeat banker = null;
        try
        {
            banker = BaccaratPlayerMgr.Inst.m_playerList.Where(x => x.id == max_betting_banker).First();
        }
        catch { }
        BaccaratUserSeat player = null;
        try
        {
            player = BaccaratPlayerMgr.Inst.m_playerList.Where(x => x.id == max_betting_player).First();
        }
        catch { }

        // move cards to banker's seat

        if (banker != null)
        {
            MoveDistributed_SmallCards(cardPanel.leftCards, banker.cardPos, Constants.BaccaratDistributionTime);

            if (max_betting_banker == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                MoveDistributed_BigCards(cardPanel.leftCards, banker.cardPos, Constants.BaccaratDistributionTime);
            }
        }

        // move cards to player's seat.
        if (player != null)
        {
            MoveDistributed_SmallCards(cardPanel.rightCards, player.cardPos, Constants.BaccaratDistributionTime);

            if (max_betting_banker == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                MoveDistributed_BigCards(cardPanel.leftCards, banker.cardPos, Constants.BaccaratDistributionTime);
            }
        }
    }

    private void MoveDistributed_BigCards(UIBCard[] originCards, Transform[] destination_cardPos, float time)
    {
        UIBCardBend.Inst.OnClickShowBigCard();
    }

    private void MoveDistributed_SmallCards(UIBCard[] originCards, Transform[] destination_cardPos, float time)
    {
        Vector3 position;
        position = destination_cardPos[0].gameObject.transform.position;
        iTween.MoveTo(originCards[0].gameObject, position, time);
        position = destination_cardPos[1].gameObject.transform.position;
        iTween.MoveTo(originCards[1].gameObject, position, time);
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