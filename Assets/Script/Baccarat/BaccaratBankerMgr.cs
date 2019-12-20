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
using Random = UnityEngine.Random;




public class BaccaratBankerMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratBankerMgr Inst;
    public List<BaccaratCard> cardList = new List<BaccaratCard>();

    public TeamCard bankerCard = new TeamCard();
    public TeamCard playerCard = new TeamCard();

    void Start()
    {
        if (!Inst)
            Inst = this;
        Init();
    }

    public void Init()
    {
        cardList.Clear();
        for (int i = 0; i < Constants.BaccaratCardUnitNumber; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    BaccaratCard card = new BaccaratCard();
                    card.color = k;
                    card.num = j;
                    card.score = card.num;
                    if (card.num > 10)
                        card.score = 0;
                    cardList.Add(card);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void OnEndPan()
    {
        bankerCard.CardList.Clear();
        playerCard.CardList.Clear();
        var limit = MakeRandomCard();

        int dealtMoney_banker = 0;
        int dealtMoney_player = 0;
        int max_betting_banker = GetMaxBettingPlayer(true, ref dealtMoney_banker);
        int max_betting_player = GetMaxBettingPlayer(false, ref dealtMoney_player);
        if (max_betting_banker == max_betting_player)
        {
            if (dealtMoney_banker > dealtMoney_player)
            {
                max_betting_player = -1;
            }
            else
            {
                max_betting_banker = -1;
            }
        }

        Hashtable table = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnCatchedCardDistributed},
            {Common.BACCARAT_CATCHED_CARD_BANKER, bankerCard.cardString},
            {Common.BACCARAT_CATCHED_CARD_PLAYER, playerCard.cardString},
            {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
            {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},
            {Common.BACCARAT_NOW_SHOWING_LIMIT, limit}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);

    }
    int GetMaxBettingPlayer(bool isBanker, ref int money)
    {
        int res = -1;
        var moneySum = 0;
        int area = Constants.BaccaratBankerArea;
        if (!isBanker)
            area = Constants.BaccaratPlayerArea;

        string dealLogString = "";
        try
        {
            dealLogString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_BETTING_LOG];
        }
        catch { }
        dealLogString = dealLogString.Trim('/');
        if (dealLogString == "") return -1;
        var list = dealLogString.Split('/').ToList();
        foreach (var player in GameMgr.Inst.seatMgr.m_playerList)
        {
            int sum = 0;
            try
            {
                sum = list.Where(x => int.Parse(x.Split(':')[2]) == area && int.Parse(x.Split(':')[0]) == player.m_playerInfo.m_actorNumber).Sum(x => BaccaratBankerMgr.Inst.getCoinValue(int.Parse(x.Split(':')[1])));
            }
            catch { }
            if (moneySum < sum)
            {
                res = player.m_playerInfo.m_actorNumber;
                moneySum = sum;
            }
        }
        money = moneySum;
        return res;
    }

    private BaccaratCard GetCard(int no)
    {
        BaccaratCard card = new BaccaratCard();
        card.color = cardList[no].color;
        card.num = cardList[no].num;
        card.score = cardList[no].score;
        cardList.RemoveAt(no);
        return card;
    }
    private int MakeRandomCard()
    {
        int res = (int)BaccaratShowingCard_NowTurn.Banker2;
        // Create Random 2 cards for each team
        for (int i = 0; i < 4; i++)
        {
            int no = (int)Random.Range(0, cardList.Count - 1);
            BaccaratCard card = GetCard(no);

            if (i <= 1)
                bankerCard.CardList.Add(card);
            else
                playerCard.CardList.Add(card);
        }

        // Check if there's lower than 5
        if (bankerCard.score < Constants.BaccaratHighScore && playerCard.score < Constants.BaccaratHighScore)
        {
            if (playerCard.score <= Constants.BaccaratScoreLimit)
            {
                int no = (int)Random.Range(0, cardList.Count - 1);
                BaccaratCard card = GetCard(no);
                playerCard.CardList.Add(card);
                res++;
            }

            if (playerCard.score < Constants.BaccaratHighScore && bankerCard.score <= Constants.BaccaratScoreLimit)
            {
                int no = (int)Random.Range(0, cardList.Count - 1);
                BaccaratCard card = GetCard(no);
                bankerCard.CardList.Add(card);
                res++;
            }
        }

        return res;
    }

    internal void CalcResult()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameMgr.Inst.Log("Wait for new game.");
            StartCoroutine(StartGame());
        }
        var victoryArea = CalcVictoryArea();
        CalcUserPrize(victoryArea);
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(10.0f);

        GameMgr.Inst.Log("New Game Start Message sent");
        Hashtable table = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnInitUI}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);

        // yield return new WaitForSeconds(2.0f);
        // table = new Hashtable{
        //     {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnStartNewPan}
        // };
        // PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }
    List<int> CalcVictoryArea()
    {

        GameMgr.Inst.Log("BankerCardString=" + bankerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);
        GameMgr.Inst.Log("PlayerCardString=" + playerCard.cardString, enumLogLevel.BaccaratDistributeCardLog);
        
        List<int> victoryArea = new List<int>();
        if (playerCard.score > bankerCard.score)
            victoryArea.Add(Constants.BaccaratPlayerArea);
        if (bankerCard.score > playerCard.score)
            victoryArea.Add(Constants.BaccaratBankerArea);
        if (playerCard.score == bankerCard.score)
            victoryArea.Add(Constants.BaccaratDrawArea);
        if (playerCard.CardList[0].num == playerCard.CardList[1].num)
            victoryArea.Add(Constants.BaccaratPPArea);
        if (bankerCard.CardList[0].num == bankerCard.CardList[1].num)
            victoryArea.Add(Constants.BaccaratBPArea);

        string areaString = string.Join(",", victoryArea);

        string exitInfo = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.AdditionalRoomProperty];
        BaccaratRoomInfo info = new BaccaratRoomInfo();
        info.roomString = exitInfo;
        info.status += "," + victoryArea[0];
        info.status = info.status.Trim(',');

        Hashtable table = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnShowingVictoryArea},
            {Common.BACCARAT_VICTORY_AREA, areaString},
            {Common.AdditionalRoomProperty, info.roomString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);

        //Common.AdditionalRoomProperty

        return victoryArea;
    }
    public int getCoinValue(int coinId)
    {
        int res = 0;
        switch (coinId)
        {
            case 0:
                res = 100; break;
            case 1:
                res = 500; break;
            case 2:
                res = 1000; break;
            case 3:
                res = 10000; break;
        }
        return res;
    }
    void CalcUserPrize(List<int> victoryArea)
    {
        string betLog = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_BETTING_LOG];
        PlayerInfoContainer pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();

        GameMgr.Inst.Log("playerListString:=" + pList.m_playerInfoListString, enumLogLevel.BaccaratLogicLog);

        foreach (var player in GameMgr.Inst.seatMgr.m_playerList.Where(x => x.m_playerInfo != null))
        {
            try
            {
                if (betLog == null)
                    continue;
                int prize = 0;
                betLog = betLog.Trim('/');
                string prize_area = "";
                foreach (var area in victoryArea)
                {
                    int prizeTimes = 1;
                    switch (area)
                    {
                        case Constants.BaccaratPlayerArea:
                            prizeTimes = Constants.BaccaratPlayerArea_prize;
                            break;
                        case Constants.BaccaratBankerArea:
                            prizeTimes = Constants.BaccaratBankerArea_prize;
                            break;
                        case Constants.BaccaratDrawArea:
                            prizeTimes = Constants.BaccaratDrawArea_prize;
                            break;
                        case Constants.BaccaratPPArea:
                            prizeTimes = Constants.BaccaratPPArea_prize;
                            break;
                        case Constants.BaccaratBPArea:
                            prizeTimes = Constants.BaccaratBPArea_prize;
                            break;
                    }

                    var betList = betLog.Split('/').Where(x => int.Parse(x.Split(':')[0]) == player.m_playerInfo.m_actorNumber);
                    int moneySum = 0;
                    try
                    {
                        moneySum = betList.Where(x => int.Parse(x.Split(':')[2]) == area).Sum(x => getCoinValue(int.Parse(x.Split(':')[1])));
                    }
                    catch { }
                    prize += moneySum * prizeTimes;
                    if (moneySum > 0)
                    {
                        prize_area += area + ":" + moneySum + ",";
                    }
                }
                // prize_area = prize_area.Trim(',');
                // try
                // {
                GameMgr.Inst.Log("now Player String:=" + player.m_playerInfo.playerInfoString, enumLogLevel.BaccaratLogicLog);
                pList.m_playerList.Where(x => x.m_actorNumber == player.m_playerInfo.m_actorNumber).First().m_coinValue += prize;
                // }
                // catch (Exception err)
                // {
                //     GameMgr.Inst.Log(err.Message);
                // }

                GameMgr.Inst.Log(player.m_playerInfo.m_actorNumber + " is Win. Sending Prize = " + prize);
                if (prize > 0)
                {
                    Hashtable table = new Hashtable{
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnPrizeAwarded},
                    {Common.PLAYER_ID, player.m_playerInfo.m_actorNumber},
                    {Common.BACCARAT_PRIZE, prize},
                    {Common.BACCARAT_PRIZE_AREA, prize_area}
                };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(table);
                }
            }
            catch (Exception err)
            {
                GameMgr.Inst.Log("error in Calc Prize: " + err.Message);                
            }
        }

        Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, enumGameMessage.OnSeatStringUpdate},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}

public class BaccaratCard
{
    public int num;
    public int color;
    public int score;
    public string cardString
    {
        get
        {
            return num + ":" + color;
        }
        set
        {
            var tmpList = value.Split(':');
            num = int.Parse(tmpList[0]);
            color = int.Parse(tmpList[1]);
            score = num;
            if (num > 10)
                score = 0;
        }
    }
}
public class TeamCard
{
    public List<BaccaratCard> CardList = new List<BaccaratCard>();
    public int score
    {
        get
        {
            return CardList.Sum(x => x.score) % 10;
        }
        set { }
    }

    public string cardString
    {
        get
        {
            return string.Join(",", CardList.Select(x => x.cardString).ToList());
        }
        set
        {
            CardList.Clear();
            var tmpList = value.Split(',');
            for (int i = 0; i < tmpList.Length; i++)
            {
                BaccaratCard card = new BaccaratCard();
                card.cardString = tmpList[i];
                CardList.Add(card);
            }
        }
    }

}