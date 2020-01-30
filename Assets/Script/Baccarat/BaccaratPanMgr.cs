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
    }

    internal void StartNewPan()
    {
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
        GameMgr.Inst.Log("Prize Area:=" + prize_area, LogLevel.BaccaratLogicLog);
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
    
    internal void OnCatchedCardDistributed()
    {
        InitTeamCard();
        
        bankerCard.cardString =
            (string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_BANKER];
        playerCard.cardString =
            (string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CATCHED_CARD_PLAYER];
        GameMgr.Inst.Log("BankerCardString=" + bankerCard.cardString, LogLevel.BaccaratDistributeCardLog);
        GameMgr.Inst.Log("PlayerCardString=" + playerCard.cardString, LogLevel.BaccaratDistributeCardLog);
        
        StartCoroutine(CardSqueezing());
    }

    IEnumerator CardSqueezing()
    {
        var maxBettingPlayer =
            (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER];
        DoCardSqueezing(maxBettingPlayer, playerCard.CardList[0],
            playerCard.CardList[1], 0);

        var maxBettingBanker =
            (int) PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_MAX_BETTING_PLAYER_BANKER];
        DoCardSqueezing(maxBettingBanker, bankerCard.CardList[0],
            bankerCard.CardList[1], 1);
        
        yield return new WaitForSeconds(Constants.BaccaratShowingCard_waitTime);
        
        ShowingCatchedCard(BaccaratShowingCard_NowTurn.Top);

        if (playerCard.CardList.Count > 2 || bankerCard.CardList.Count > 2) //additional cards
        {
            if (playerCard.CardList.Count > 2)
                DoCardSqueezing(maxBettingPlayer, playerCard.CardList[0],
                    playerCard.CardList[1], 0);
            if (bankerCard.CardList.Count > 2)
                ShowingCatchedCard(BaccaratShowingCard_NowTurn.Banker_additional);
            
            yield return new WaitForSeconds(Constants.B3rdWaitTime);

            if (playerCard.CardList.Count > 2)
                ShowingCatchedCard(BaccaratShowingCard_NowTurn.Player_additional);
            if (bankerCard.CardList.Count > 2)
                ShowingCatchedCard(BaccaratShowingCard_NowTurn.Banker_additional);
            
            yield return new WaitForSeconds(Constants.B3rdWaitTime);
        }
        BaccaratBankerMgr.Inst.CalcResult(); //finished one game
    }
    
    private void DoCardSqueezing(int maxBetter, BaccaratCard card1,
        BaccaratCard card2, int bendId,bool isExtra=false)
    {
        LogMgr.Inst.LogD("max_better:" + maxBetter, LogLevels.Baccarat_Card);
        if (maxBetter == -1)
            return;
        BaccaratUserSeat player = (BaccaratUserSeat) BaccaratPlayerMgr.Inst.GetUserSeatFromList(maxBetter);
        
        if (player != null)
        {
            if (isExtra)
                iTween.MoveTo(cardPanel.cards[bendId][2].gameObject, player.cardPos[0].parent.position, Constants.BTweenTime);
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    iTween.MoveTo(cardPanel.cards[bendId][i].gameObject, player.cardPos[i].position, Constants.BTweenTime);
                }    
            }
            
            
            bool isController = (maxBetter == PhotonNetwork.LocalPlayer.ActorNumber);
            bend[bendId].ShowBigCard(player.cardPos, card1, card2, isController,cardPanel.cards[bendId]);
        }
    }

    
    private void ShowingCatchedCard(BaccaratShowingCard_NowTurn nowTurn)
    {
        if (playerCard.CardList.Count == 0) return;
        if (bankerCard.CardList.Count == 0) return;

        switch (nowTurn)
        {
            case BaccaratShowingCard_NowTurn.Top:
                cardPanel.leftCards[0].ShowImage(playerCard.CardList[0].num, playerCard.CardList[0].color);
                cardPanel.leftCards[1].ShowImage(playerCard.CardList[1].num, playerCard.CardList[1].color);
                
                cardPanel.rightCards[0].ShowImage(bankerCard.CardList[0].num, bankerCard.CardList[0].color);
                cardPanel.rightCards[1].ShowImage(bankerCard.CardList[1].num, bankerCard.CardList[1].color);
                
                cardPanel.TweenOriginalPos();
                break;
            case BaccaratShowingCard_NowTurn.Player_additional:
                if (playerCard.CardList.Count > 2)
                    cardPanel.leftCards[2].ShowImage(playerCard.CardList[2].num, playerCard.CardList[2].color);
                break;
            case BaccaratShowingCard_NowTurn.Banker_additional:
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