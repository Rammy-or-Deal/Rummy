/*
this script is attached to ammo, and stores data to be used by the PickUpItemScript
*/


using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

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

        int max_betting_banker = GetMaxBettingPlayer(true);
        int max_betting_player = GetMaxBettingPlayer(false);

        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnCatchedCardDistributed},
            {Common.BACCARAT_CATCHED_CARD_BANKER, bankerCard.cardString},
            {Common.BACCARAT_CATCHED_CARD_PLAYER, playerCard.cardString},
            {Common.BACCARAT_MAX_BETTING_PLAYER_BANKER, max_betting_banker},
            {Common.BACCARAT_MAX_BETTING_PLAYER_PLAYER, max_betting_player},
            {Common.BACCARAT_NOW_SHOWING_LIMIT, limit}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }
    int GetMaxBettingPlayer(bool isBanker)
    {
        int res = -1;
        var moneySum = 0;
        int area = Constants.BaccaratBankerArea;
        if (!isBanker)
            area = Constants.BaccaratPlayerArea;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var tmp = (string)player.CustomProperties[Common.PLAYER_BETTING_LOG];
            tmp = tmp.Trim('/');
            if (tmp == "") continue;

            var list = tmp.Split('/').ToList();
            var sum = 0;
            sum = list.Where(x => int.Parse(x.Split(':')[1]) == area).Sum(x => int.Parse(x.Split(':')[1]));
            if (moneySum < sum)
            {
                res = player.ActorNumber;
                moneySum = sum;
            }
        }

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
        CalcVictoryArea();
    }
    void CalcVictoryArea()
    {
        List<int> victoryArea = new List<int>();
        if(playerCard.score > bankerCard.score)
            victoryArea.Add(Constants.BaccaratPlayerArea);
        if(bankerCard.score > playerCard.score)
            victoryArea.Add(Constants.BaccaratBankerArea);
        if(playerCard.score == bankerCard.score)
            victoryArea.Add(Constants.BaccaratDrawArea);
        if(playerCard.CardList[0].num == playerCard.CardList[1].num)
            victoryArea.Add(Constants.BaccaratPPArea);
        if(bankerCard.CardList[0].num == bankerCard.CardList[1].num)
            victoryArea.Add(Constants.BaccaratBPArea);

        string areaString = string.Join(",", victoryArea);

        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnShowingVictoryArea},
            {Common.BACCARAT_VICTORY_AREA, areaString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }
}
