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

    public Card(int num0, int color0)
    {
        num = num0;
        color = color0;
    }
}
public class CardManager : MonoBehaviour
{
    public const int JokerNum = 0;
    public static CardManager Inst;
    public Card[][] playerCard=new Card[4][];
    public int[][] initCard=new int[4][];
    public int maxJoker = 8;

    public CardManager()
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
            initCard[i]=new int[14];
            for (int j = 0; j < 14; j++)
                initCard[i][j] = 2;
        }
        for (int i = 0; i < 4; i++)
        {
            playerCard[i]=new Card[20];
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
        for (int i = 0; i < maxJoker;i++)
        {
            float ran = Random.Range(0f, 1f);
            if (ran < 0.7f)
            {
                int playerId=Random.Range(0, 4);
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
        int color=Random.Range(0, 4);
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
        for (int i=0;i<PhotonNetwork.PlayerList.Length;i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            string cardStr = ConvertCardListToString(playerCard[i]);
            
            Hashtable initialProps = new Hashtable()
            {
                { Common.PLAYER_CARD_List, cardStr }
            };
            p.SetCustomProperties(initialProps);
        }
    }

    public static string ConvertCardListToString(Card[] cardList)
    {
        int[] numList=new int[cardList.Length];
        int[] colList=new int[cardList.Length];
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
        int[] numList = str[0].Split(',').Select(Int32.Parse).ToArray();
        int[] colList=str[1].Split(',').Select(Int32.Parse).ToArray();
        Card[] cardList=new Card[numList.Length];
        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i]=new Card(numList[i],colList[i]);
        }
        return cardList;
    }
    
    public static string ConvertSelectedListToString(List<LamiMyCard> cardList)
    {
        int[] numList=new int[cardList.Count];
        int[] colList=new int[cardList.Count];
        for (int i = 0; i < cardList.Count; i++)
        {
            numList[i] = cardList[i].num;
            colList[i] = cardList[i].color;
        }
        string numString = string.Join(",", numList);
        string colString = string.Join(",", colList);
        return numString + ":" + colString;
    }

    public Card[] ReceivedCardList(int id, Card[] cardList)
    {
        playerCard[id] = cardList;
        Debug.Log("receive:" + id +"  player: "+ playerCard[id]);
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
