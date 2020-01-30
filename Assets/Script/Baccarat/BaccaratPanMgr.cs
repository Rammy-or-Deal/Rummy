/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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

    public UIBCardBend[] bend;

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
        // foreach (var player in PhotonNetwork.PlayerList)
        // {
        //     Hashtable prop = new Hashtable{
        //         {Common.PLAYER_BETTING_LOG, ""},
        //         {Common.NOW_BET, ""},
        //     };
        //     player.SetCustomProperties(prop);
        // }

        Hashtable table = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int) enumGameMessage.Baccarat_OnStartNewPan},
            {Common.BACCARAT_CURRENT_TIME, Constants.BaccaratCurrentTime},
            {Common.PLAYER_BETTING_LOG, ""},
            {Common.NOW_BET, ""}
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
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        betPanel.Init();
        cardPanel.Init();
        StartCoroutine(WaitFor1Second());
    }


    public IEnumerator WaitFor1Second()
    {
        int time = -100;
        try
        {
            time = (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CURRENT_TIME];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        m_panTime.text = time + "";
        yield return new WaitForSeconds(1.0f);
        time--;
        if (PhotonNetwork.IsMasterClient && time >= -1)
        {
            if (time >= 0)
            {
                Hashtable table = new Hashtable
                {
                    {PhotonFields.GAME_MESSAGE, (int) enumGameMessage.Baccarat_OnPanTimeUpdate},
                    {Common.BACCARAT_CURRENT_TIME, time}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);

                GameMgr.Inst.botMgr.Deal();
            }
            else
            {
                Hashtable table = new Hashtable
                {
                    {PhotonFields.GAME_MESSAGE, (int) enumGameMessage.Baccarat_OnEndPan},
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
        }
    }

    internal void OnInitUI()
    {
    }

    internal void OnPrizeAwarded()
    {
        message.Show("Congratulations!");
        StartCoroutine(SetPrize());
    }

    IEnumerator SetPrize()
    {
        yield return new WaitForSeconds(3);
        var prize_area = (string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_PRIZE_AREA];
        message.Hide();
        GameMgr.Inst.Log("Prize Area:=" + prize_area, enumLogLevel.BaccaratLogicLog);
        prize_area = prize_area.Trim(',');
        foreach (var area in prize_area.Split(','))
        {
            var areaId = int.Parse(area.Split(':')[0]);
            var prize = int.Parse(area.Split(':')[1]);
            betPanel.pans[areaId].SetPrize(prize);
        }
    }

    internal void OnShowingVictoryArea()
    {
        var areaList = ((string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_VICTORY_AREA]).Split(',')
            .Select(Int32.Parse).ToList();

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

    internal void OnPlayerBet(Vector3 originPos, int moneyId, int areaId)
    {
        //        LogMgr.Inst.Log(string.Format("Player Bet. x={0}, y={1}, moneyId={2}, areaId={3}", moneyId, areaId), (int)LogLevels.PanLog);
        betPanel.OnPlayerBet(originPos, moneyId, areaId);
    }

    Transform[] leftCardOrgPos;
    Transform[] rightCardOrgPos;

    void SaveOrgPosition()
    {
        if (leftCardOrgPos == null) leftCardOrgPos = new Transform[2];
        if (rightCardOrgPos == null) rightCardOrgPos = new Transform[2];
        leftCardOrgPos[0] = cardPanel.leftCards[0].transform;
        leftCardOrgPos[1] = cardPanel.leftCards[1].transform;

        rightCardOrgPos[0] = cardPanel.rightCards[0].transform;
        rightCardOrgPos[1] = cardPanel.rightCards[1].transform;
    }

    internal void OnCatchedCardDistributed()
    {
        InitTeamCard();
        SaveOrgPosition();
        bankerCard.cardString =
            (string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_BANKER];
        playerCard.cardString =
            (string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_PLAYER];
        GameMgr.Inst.Log("BankerCardString=" + bankerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);
        GameMgr.Inst.Log("PlayerCardString=" + playerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);
        
        var maxBettingPlayer =
            (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER];
        AddAnimationForDistributedCard(cardPanel.leftCards, maxBettingPlayer, playerCard.CardList[0],
            playerCard.CardList[1], false);

        var maxBettingBanker =
            (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_BANKER];
        AddAnimationForDistributedCard(cardPanel.rightCards, maxBettingBanker, bankerCard.CardList[0],
            bankerCard.CardList[1], true);
        
        StartCoroutine(CheckAdditionalCards());
    }

    IEnumerator CheckAdditionalCards()
    {
        yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime);
        
        ShowingCatchedCard((int) BaccaratShowingCard_NowTurn.Player);
        ShowingCatchedCard((int) BaccaratShowingCard_NowTurn.Banker);

        if (playerCard.CardList.Count > 2 || bankerCard.CardList.Count > 2)
        {
            StartCoroutine(ShowingAdditionalCard());
        }
        else
        {
            BaccaratBankerMgr.Inst.CalcResult();
        }
    }

    IEnumerator ShowingAdditionalCard()
    {
        yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime / 2);

        if (playerCard.CardList.Count > 2)
        {
            ShowingCatchedCard((int) BaccaratShowingCard_NowTurn.Player_additional);
            yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime / 2);
        }

        if (bankerCard.CardList.Count > 2)
        {
            ShowingCatchedCard((int) BaccaratShowingCard_NowTurn.Banker_additional);
            yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime / 2);
        }

        BaccaratBankerMgr.Inst.CalcResult();
    }

    private void AddAnimationForDistributedCard(UIBCard[] orgCards, int max_better, BaccaratCard card1,
        BaccaratCard card2, bool isBanker)
    {
        BaccaratUserSeat player = null;
        if (BaccaratPlayerMgr.Inst.m_playerList.Count(x =>
                x.isSeat == true && x.m_playerInfo.m_actorNumber == max_better) > 0)
            player = (BaccaratUserSeat) BaccaratPlayerMgr.Inst.m_playerList.First(x =>
                x.isSeat == true && x.m_playerInfo.m_actorNumber == max_better);

        LogMgr.Inst.LogD("max_better:" + max_better, LogLevels.Baccarat_Card);
        LogMgr.Inst.LogD("Card1:" + card1.num + "  Card2:" + card2.num, LogLevels.Baccarat_Card);
        if (player != null)
        {
            MoveDistributed_SmallCards(orgCards, player.cardPos, Constants.BaccaratDistributionTime);
            bool isController = (max_better == PhotonNetwork.LocalPlayer.ActorNumber);
            LogMgr.Inst.LogD("localPlayer actorNumber:" + PhotonNetwork.LocalPlayer.ActorNumber,
                LogLevels.Baccarat_Card);
            int bendId = 0;
            if (isBanker)
                bendId = 1;
            bend[bendId].ShowBigCard(player.cardPos, card1, card2, isController,orgCards);
        }
        else
        {
            LogMgr.Inst.LogD(
                "count:" + BaccaratPlayerMgr.Inst.m_playerList.Count(x =>
                    x.isSeat == true && x.m_playerInfo.m_actorNumber == max_better), LogLevels.Baccarat_Card);
        }
    }

    private void MoveDistributed_SmallCards(UIBCard[] originCards0, Transform[] destination_cardPos, float time)
    {
        Vector3 position;
        position = destination_cardPos[0].position;
        iTween.MoveTo(originCards0[0].gameObject, position, Constants.BaccaratDistributionTime);
        position = destination_cardPos[1].position;
        iTween.MoveTo(originCards0[1].gameObject, position, Constants.BaccaratDistributionTime);
    }

    Coroutine ShowingCardRoutine;

    IEnumerator ShowingCard(int nowTurn)
    {
        yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime);
        Hashtable table = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int) enumGameMessage.Baccarat_OnShowingCatchedCard},
            {Common.BACCARAT_NOW_SHOWING_TURN, nowTurn}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnShowingCatchedCard()
    {
        //BACCARAT_NOW_SHOWING_TURN

        int nowTurn = (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_NOW_SHOWING_TURN];
        GameMgr.Inst.Log("Now showing Card=" + (BaccaratShowingCard_NowTurn) nowTurn);
        // Here, we can add the code to make the users can open the card. player is depended on 
        // {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
        // {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},        
        ShowingCatchedCard(nowTurn);

        if (!PhotonNetwork.IsMasterClient) return;

        int limit = (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_NOW_SHOWING_LIMIT];

        try
        {
            StopCoroutine(ShowingCardRoutine);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        //nowTurn++;
        LogMgr.Inst.Log("New Card command Created. id=" + (BaccaratShowingCard_NowTurn) nowTurn,
            (int) LogLevels.PlayerLog1);

        if (nowTurn == (int) BaccaratShowingCard_NowTurn.Player)
        {
            nowTurn = (int) BaccaratShowingCard_NowTurn.Banker;
            ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
        }
        else if (nowTurn == (int) BaccaratShowingCard_NowTurn.Banker)
        {
            if (playerCard.CardList.Count > 2)
            {
                nowTurn = (int) BaccaratShowingCard_NowTurn.Player_additional;
                ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
            }
            else if (bankerCard.CardList.Count > 2)
            {
                nowTurn = (int) BaccaratShowingCard_NowTurn.Banker_additional;
                ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
            }
            else
            {
                BaccaratBankerMgr.Inst.CalcResult();
            }
        }
        else if (nowTurn == (int) BaccaratShowingCard_NowTurn.Player_additional)
        {
            nowTurn = (int) BaccaratShowingCard_NowTurn.Banker_additional;
            if (bankerCard.CardList.Count > 2)
            {
                nowTurn = (int) BaccaratShowingCard_NowTurn.Banker_additional;
                ShowingCardRoutine = StartCoroutine(ShowingCard(nowTurn));
            }
            else
            {
                BaccaratBankerMgr.Inst.CalcResult();
            }
        }
        else
        {
            BaccaratBankerMgr.Inst.CalcResult();
        }
    }

    private void ShowingCatchedCard(int nowTurn)
    {
        GameMgr.Inst.Log("Card showing command called. id=" + (BaccaratShowingCard_NowTurn) nowTurn,
            enumLogLevel.BaccaratDistributeCardLog);
        GameMgr.Inst.Log("now Playercard=" + playerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);
        GameMgr.Inst.Log("now Bankercard=" + bankerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);

        if (playerCard.CardList.Count == 0) return;
        if (bankerCard.CardList.Count == 0) return;

        switch (nowTurn)
        {
            case (int) BaccaratShowingCard_NowTurn.Player:
                cardPanel.leftCards[0].ShowImage(playerCard.CardList[0].num, playerCard.CardList[0].color);
                cardPanel.leftCards[1].ShowImage(playerCard.CardList[1].num, playerCard.CardList[1].color);
                MoveDistributed_SmallCards(cardPanel.leftCards, leftCardOrgPos, Constants.BaccaratDistributionTime);
                break;
            case (int) BaccaratShowingCard_NowTurn.Banker:
                cardPanel.rightCards[0].ShowImage(bankerCard.CardList[0].num, bankerCard.CardList[0].color);
                cardPanel.rightCards[1].ShowImage(bankerCard.CardList[1].num, bankerCard.CardList[1].color);
                MoveDistributed_SmallCards(cardPanel.rightCards, rightCardOrgPos, Constants.BaccaratDistributionTime);
                break;
            case (int) BaccaratShowingCard_NowTurn.Player_additional:
                if (playerCard.CardList.Count > 2)
                    cardPanel.leftCards[2].ShowImage(playerCard.CardList[2].num, playerCard.CardList[2].color);
                break;
            case (int) BaccaratShowingCard_NowTurn.Banker_additional:
                if (bankerCard.CardList.Count > 2)
                    cardPanel.rightCards[2].ShowImage(bankerCard.CardList[2].num, bankerCard.CardList[2].color);
                break;
        }

        #region Card step by step   ------ not used

        /*
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
        */

        #endregion
    }

    private void InitTeamCard()
    {
        bankerCard.CardList.Clear();
        playerCard.CardList.Clear();
    }
}