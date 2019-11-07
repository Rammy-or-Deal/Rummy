using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Card
{
    public int num;
    public int color;

    public int MyCardId = -1;
    public int virtual_num;
    public List<Card> children;
    public Card parent = null;
    public string cardString{
        get{
            return num+":"+color;
        }
        set{
            var tmp = value.Split(':').Select(Int32.Parse).ToArray();
            num = tmp[0];
            color = tmp[1];
        }
    }
    public Card(int num0, int color0)
    {
        num = num0;
        color = color0;
        virtual_num = num;
    }
    public Card()
    {

    }


    #region For only baccarat
    public List<Card> Children_Set(List<Card> m_orgList)
    {
        children = new List<Card>();
        foreach (var card in m_orgList.Where(x => x.MyCardId > MyCardId && x.num == num && x.num != 15).ToList())
        {
            var tmpCard = new Card();
            tmpCard.num = card.num;
            tmpCard.color = card.color;
            tmpCard.MyCardId = card.MyCardId;
            tmpCard.virtual_num = card.virtual_num;

            tmpCard.parent = this;
            children.Add(tmpCard);
        }
        return children;
    }
    public List<Card> Children_Flush(List<Card> m_orgList)
    {
        children = new List<Card>();
        foreach (var card in m_orgList.Where(x => x.MyCardId != MyCardId && x.num == num + 1 && x.color == color && x.num != 15))
        {
            card.parent = this;
            children.Add(card);
        }
        return children;
    }
    #endregion
}
public class LamiCardMgr : MonoBehaviour
{
    public const int JokerNum = 15;
    public static LamiCardMgr Inst;
    public Card[][] playerCard = new Card[4][];
    public int[][] initCard = new int[4][];
    public int maxJoker = 8;

    public LamiCardMgr()
    {
    }

    private void Awake()
    {
        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
    }

    public void GenerateCard()
    {
        for (int i = 0; i < 4; i++)
        {
            initCard[i] = new int[14];
            for (int j = 0; j < 14; j++)
                initCard[i][j] = 2;
        }
        for (int i = 0; i < 4; i++)
        {
            playerCard[i] = new Card[20];
            for (int j = 0; j < 20; j++)
            {
                playerCard[i][j] = GetRandomCard();
                //                Debug.Log(i + ": player :"+ j +":card :" + playerCard[i][j].color+ "//" + playerCard[i][j].num );
            }
            //            Debug.Log(i + ": player :card :" + playerCard[i]);
        }

        MakeJokerCard();

        SendCardsToPlayers();
    }

    private void MakeJokerCard()
    {
        for (int i = 0; i < maxJoker; i++)
        {
            float ran = Random.Range(0f, 1f);
            if (ran < 0.7f)
            {
                int playerId = Random.Range(0, 4);
                for (int k = 19; k > 19 - maxJoker; k--)
                {
                    if (playerCard[playerId][k].num != JokerNum) //check Joker card
                    {
                        playerCard[playerId][k].num = JokerNum;
                        playerCard[playerId][k].color = JokerNum;
                        break;
                    }
                }
            }
        }
    }

    public Card GetRandomCard()
    {
        int num = Random.Range(1, 14);
        int color = Random.Range(0, 4);
        if (initCard[color][num] > 0)
        {
            initCard[color][num]--;
            return new Card(num, color);
        }
        else
            return GetRandomCard();
    }

    public void SendCardsToPlayers()
    {
        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        var tmp = seatString.Split(',');
        string totalCardString = "";
        for (int i = 0; i < tmp.Length; i++)
        {
            totalCardString += tmp[i].Split(':')[0] + ":";
            string cardStr = ConvertCardListToString(playerCard[i]);
            totalCardString += cardStr + "/";
        }
        // actorNumber:num1,num2,num3...:col1,col2,col3... / actorNumber:num1,num2,num3...:col1,col2,col3...
        totalCardString = totalCardString.Trim('/');
        Hashtable props = new Hashtable()
        {
            { Common.LAMI_MESSAGE, (int)LamiMessages.OnCardDistributed},
            { Common.CARD_LIST_STRING, totalCardString }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        LogMgr.Inst.Log("Prepare CardList: " + totalCardString, (int)LogLevels.PlayerLog2);
    }

    public static string ConvertCardListToString(Card[] cardList)
    {
        // num1,num2,num3...:col1,col2,col3...
        int[] numList = new int[cardList.Length];
        int[] colList = new int[cardList.Length];
        for (int i = 0; i < cardList.Length; i++)
        {
            numList[i] = cardList[i].num;
            colList[i] = cardList[i].color;
        }
        string numString = string.Join(",", numList);
        string colString = string.Join(",", colList);
        return numString + ":" + colString;
    }

    public static Card[] ConvertCardStrToCardList(string cardStr)
    {
        string[] str = cardStr.Split(':');
        int[] numList = str[1].Split(',').Select(Int32.Parse).ToArray();
        int[] colList = str[2].Split(',').Select(Int32.Parse).ToArray();


        Card[] cardList = new Card[numList.Length];
        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i] = new Card(numList[i], colList[i]);
        }

        // add virtual number
        if (str.Length >= 4)
        {
            int[] v_numList = str[3].Split(',').Select(Int32.Parse).ToArray();
            for (int i = 0; i < cardList.Length; i++)
            {
                cardList[i].virtual_num = v_numList[i];
            }
        }
        return cardList;
    }

    internal void OnShuffleRequest(Player player)
    {
        //cardString += card.num + ":" + card.color + ":" + card.MyCardId + ",";
        var cardString = (string)player.CustomProperties[Common.SHUFFLE_CARDS];

        var cardList = cardString.Split(',');
        for(int i = 0; i < cardList.Length; i++)
        {
            try{
            var tmp = cardList[i].Split(':').Select(Int32.Parse).ToArray();
            initCard[tmp[1]][tmp[0]]++;

            var new_card = GetRandomCard();
            tmp[0] = new_card.num;
            tmp[1] = new_card.color;
            cardList[i] = tmp[0]+":"+tmp[1]+":"+tmp[2];
            }catch{}
        }
        cardString = string.Join(",", cardList);

        
        Hashtable props = new Hashtable{
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnShuffleAccept},
            {Common.SHUFFLE_CARDS, cardString}
        };
        player.SetCustomProperties(props);
    }

    public static string ConvertSelectedListToString(List<Card> cardList)
    {
        int[] numList = new int[cardList.Count];
        int[] colList = new int[cardList.Count];
        int[] v_numList = new int[cardList.Count];

        for (int i = 0; i < cardList.Count; i++)
        {
            numList[i] = cardList[i].num;
            colList[i] = cardList[i].color;
            v_numList[i] = cardList[i].virtual_num;
        }
        string numString = string.Join(",", numList);
        string colString = string.Join(",", colList);
        string v_numString = string.Join(",", v_numList);
        return numString + ":" + colString + ":" + v_numString;
    }

    public Card[] ReceivedCardList(int id, Card[] cardList)
    {
        playerCard[id] = cardList;
        Debug.Log("receive:" + id + "  player: " + playerCard[id]);
        return playerCard[id];
    }

    public int GetJokerCount(int player_id)
    {
        int cn = 0;
        for (int i = 0; i < 20; i++)
        {
            if (playerCard[player_id][i].num == JokerNum)
                cn++;
        }
        return cn;
    }

    public int GetACount(int player_id)
    {
        int cn = 0;
        for (int i = 0; i < 20; i++)
        {
            if (playerCard[player_id][i].num == 1)
                cn++;
        }
        return cn;
    }
}
