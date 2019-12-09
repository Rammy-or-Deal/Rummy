﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
public class RummyCardMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public Card[][] playerCard = new Card[4][];
    public int[][] initCard = new int[4][];
    public int maxJoker = constantContainer.Rummy_MaxJokerCount;
    public const int JokerNum = 15;
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
        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.SEAT_STRING];
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
            { PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnCardDistributed},
            { PhotonFields.CARD_LIST_STRING, totalCardString }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        GameMgr.Inst.Log("Prepare CardList: " + totalCardString, enumLogLevel.RummyCardMgrLog);
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
}