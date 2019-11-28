﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class ATTACH_CLASS
{
    public int lineNo = -1;
    public List<Card> list = new List<Card>();
}
public class UIMyCardPanel : MonoBehaviour
{
    //MyCard
    public List<ATTACH_CLASS> m_machedList = new List<ATTACH_CLASS>();
    public List<LamiMyCard> myCards;
    public List<LamiMyCard> originalCards;

    public GameObject[] cursorPoints;
    private int curCursorNum = 0;
    public bool sortedByColor;
    [HideInInspector] public List<LamiMyCard> selectedCards;

    // Start is called before the first frame update
    private void Awake()
    {

        selectedCards = new List<LamiMyCard>();

        sortedByColor = false;
    }

    public void InitCards(Card[] cards)
    {
        if (myCards == null)
            myCards = new List<LamiMyCard>();
        else
            myCards.Clear();

        foreach (var card in originalCards)
        {
            myCards.Add(card);
        }
        for (int i = 0; i < cards.Length; i++)
        {
            myCards[i].num = cards[i].num;
            myCards[i].color = cards[i].color;
            myCards[i].MyCardId = i;
            myCards[i].gameObject.SetActive(true);
            //cardEntry.UpdateValue();
        }

        gameObject.SetActive(true);
    }

    public void DealCards(int lineNo, List<Card> list)
    {
        string cardStr = LamiCardMgr.ConvertSelectedListToString(list);
        cardStr = PhotonNetwork.LocalPlayer.ActorNumber + ":" + cardStr;
        int remainCard = LamiGameUIManager.Inst.myCardPanel.myCards.Count - list.Count;
        Hashtable gameCards = new Hashtable
        {
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
            {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
            {Common.REMAIN_CARD_COUNT, remainCard},
            {Common.GAME_CARD, cardStr},
            {Common.GAME_CARD_PAN, lineNo},
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(gameCards);
        LogMgr.Inst.Log("User dealt card: " + cardStr, (int)LogLevels.PlayerLog2);
        LamiMe.Inst.isFirstTurn = false;
    }

    public void RemoveCards()
    {
        foreach (LamiMyCard card in LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList())
        {
            myCards.Remove(card);
            card.gameObject.SetActive(false);
        }
        selectedCards.Clear();
        LamiGameUIManager.Inst.playButton.interactable = (LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true) > 0);
    }

    public void ArrangeMyCard()
    {
        if (!sortedByColor)
        {
            // Arrange cards by color
            for (int i = 0; i < myCards.Count - 1; i++)
            {
                // first sort by color
                for (int j = i + 1; j < myCards.Count; j++)
                {
                    if (myCards[i].color > myCards[j].color)
                    {
                        int col = myCards[i].color;
                        int num = myCards[i].num;
                        myCards[i].color = myCards[j].color;
                        myCards[i].num = myCards[j].num;
                        myCards[j].color = col;
                        myCards[j].num = num;
                    }
                }
            }
            // sort by number
            for (int col = 0; col < 4; col++)
            {
                for (int i = 0; i < myCards.Count - 1; i++)
                {
                    if (myCards[i].color != col) continue;
                    for (int j = i + 1; j < myCards.Count; j++)
                    {
                        if (myCards[j].color != col) continue;

                        if (myCards[i].num > myCards[j].num)
                        {
                            int color = myCards[i].color;
                            int num = myCards[i].num;
                            myCards[i].color = myCards[j].color;
                            myCards[i].num = myCards[j].num;
                            myCards[j].color = color;
                            myCards[j].num = num;
                        }
                    }
                }
            }
        }
        else
        {
            // Arrange cards by number

            // sort by number

            for (int i = 0; i < myCards.Count - 1; i++)
            {
                for (int j = i + 1; j < myCards.Count; j++)
                {
                    if (myCards[i].num > myCards[j].num)
                    {
                        int color = myCards[i].color;
                        int num = myCards[i].num;
                        myCards[i].color = myCards[j].color;
                        myCards[i].num = myCards[j].num;
                        myCards[j].color = color;
                        myCards[j].num = num;
                    }
                }
            }
            /*
                        for (int num = 0; num < 14; num++)
                        {
                            for (int i = 0; i < myCards.Count - 1; i++)
                            {
                                if(myCards[i].num != num) continue;
                                // first sort by color
                                for (int j = i + 1; j < myCards.Count; j++)
                                {
                                    if(myCards[i].num != num) continue;
                                    if (myCards[i].color < myCards[j].color)
                                    {
                                        int col = myCards[i].color;
                                        int number = myCards[i].num;
                                        myCards[i].color = myCards[j].color;
                                        myCards[i].num = myCards[j].num;
                                        myCards[j].color = col;
                                        myCards[j].num = number;
                                    }
                                }
                            }
                        }
                        */
        }

        // push joker to the last
        for (int i = 0; i < myCards.Count - 1; i++)
        {
            if (myCards[i].num == 0)
            {
                for (int j = i + 1; j < myCards.Count; j++)
                {
                    myCards[j - 1].num = myCards[j].num;
                    myCards[j - 1].color = myCards[j].color;
                }
                myCards[myCards.Count - 1].num = 0;
                myCards[myCards.Count - 1].color = 0;
            }
        }

        for (int i = 0; i < myCards.Count; i++)
        {
            myCards[i].UpdateValue();
        }

        sortedByColor = !sortedByColor;
    }

    public static List<ATTACH_CLASS> GetMatchedList(List<LamiMyCard> selList, List<ATTACH_CLASS> sourceList)
    {
        List<ATTACH_CLASS> resList = new List<ATTACH_CLASS>();

        foreach (var list in sourceList.Where(x => x.list.Count == selList.Count))
        {
            bool isSame = true;
            LogMgr.Inst.ShowLog(list.list, "Match list:=");

            if (list.list.Count(x => x.num == 15) != selList.Count(x => x.num == 15))
            { 
                isSame = false; 
            }
            else
            {
                foreach (var sourceCard in list.list)
                {
                    if (sourceCard.num == 15) continue;
                    if (selList.Count(x => x.num == sourceCard.num && x.color == sourceCard.color) == 0)
                    {
                        isSame = false;
                        break;
                    }
                }

                if (isSame)
                {
                    foreach(var selCard in selList)
                    {
                        if(selCard.num == 15) continue;
                        if(list.list.Count(x=>x.num == selCard.num && x.color == selCard.color) == 0)
                        {
                            isSame = false;
                            break;
                        }
                    }
                }
            }
            if (isSame)
            {
                LogMgr.Inst.Log("Match added", (int)LogLevels.SpecialLog);

                ATTACH_CLASS new_item = new ATTACH_CLASS();
                new_item.lineNo = list.lineNo;
                new_item.list = new List<Card>();
                new_item.list.AddRange(list.list);

                resList.Add(new_item);
            }
        }

        return resList;
    }

    internal void ClearMachedCardList()
    {
        foreach (var list in m_machedList)
        {
            list.list.Clear();
        }
        m_machedList.Clear();
    }

    public void SetPlayButtonState()
    {
        if (LamiPlayerMgr.Inst.GetUserSeat(LamiPlayerMgr.Inst.nowTurn) != 0) return;

        int count = LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true);    // Get Selected Card

        if (count == 3)
        {
            LamiGameUIManager.Inst.shuffleButton.interactable = true;
        }
        else
        {
            LamiGameUIManager.Inst.shuffleButton.interactable = false;
        }

        ClearMachedCardList();


        InitPanList();
        LamiGameUIManager.Inst.playButton.interactable = false;


        LogMgr.Inst.Log("------------------------------------ Line List---------------------------------- ---", (int)LogLevels.SpecialLog);
        //foreach (var line in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        foreach (var line in LamiMe.Inst.availList)
        {
            string tmp = "";
            foreach (var card in line.list)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            LogMgr.Inst.Log(tmp, (int)LogLevels.SpecialLog);
        }
        LogMgr.Inst.Log("---- LineEnd ---", (int)LogLevels.SpecialLog);



        var selectedList = LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList();

        LogMgr.Inst.Log("------------------------------------ Selected Line List ------------------------------", (int)LogLevels.SpecialLog);
        //foreach (var line in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        foreach (var line in LamiMe.Inst.availList.Where(x => x.list.Count == count).ToList())
        {
            string tmp = "";
            foreach (var card in line.list)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            LogMgr.Inst.Log(tmp);
        }
        LogMgr.Inst.Log("---- LineEnd ---", (int)LogLevels.SpecialLog);

        string log = "";
        foreach (var card in selectedList)
        {
            log += card.MyCardId + "/ num=" + card.num + ", color=" + card.color + ", virtual=" + card.virtual_num + "  :  ";
        }

        //LogMgr.Inst.Log("selectedCard: " + log, (int)LogLevels.SpecialLog);
        LogMgr.Inst.Log("selectedCard: " + log, (int)LogLevels.Lami_Second_Working);



        // Get all mached lists

        m_machedList.AddRange(GetMatchedList(selectedList, LamiMe.Inst.availList));
        log = "";
        foreach (var list in m_machedList)
        {
            log += "lineNo:=" + list.lineNo + "/ ";
            foreach (var card in list.list)
            {
                log += card.MyCardId + "/ num=" + card.num + ", color=" + card.color + ", virtual=" + card.virtual_num + "  :  ";
            }

        }
        LogMgr.Inst.Log("Matched List: " + log, (int)LogLevels.Lami_Second_Working);


        InitPanList();  // Remove all cursors

        // Check if it's matched pan game cards.
        List<int> cursorList = new List<int>();
        foreach (var line in m_machedList)
        {
            if (!cursorList.Contains(line.lineNo) && line.lineNo != -1)
                cursorList.Add(line.lineNo);
        }

        foreach (var cursorPos in cursorList)
        {
            ShowCursorpoint(cursorPos);
        }


        // Log
        LogMgr.Inst.Log("---- m_machedList ---", (int)LogLevels.SpecialLog);
        for (int i = 0; i < m_machedList.Count; i++)
        {
            var list = m_machedList[i].list;
            string tmp = "";
            foreach (var card in list)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            LogMgr.Inst.Log(tmp, (int)LogLevels.SpecialLog);
        }
        LogMgr.Inst.Log("---- end ---", (int)LogLevels.SpecialLog);




        LamiGameUIManager.Inst.playButton.interactable = (m_machedList.Count > 0);
    }
    public void OnClickLine(int lineNum = -1)
    {
        InitPanList();
        //if (attachList.Count > 1 && lineNum == -1) return;

        if (m_machedList.Count(x => x.lineNo == lineNum) > 1)
        {

            List<List<Card>> temp = new List<List<Card>>();
            List<int> matchNoList = new List<int>();
            foreach (var list in m_machedList.Where(x => x.lineNo == lineNum))
            {
                if (list.list.Count(x => x.num != 15) > 0)
                {
                    temp.Add(list.list);
                }
                else // If matched cards are only joker, then add pan list, too.
                {
                    if (LamiGameUIManager.Inst.mGameCardPanelList[lineNum].mGameCardList[0].virtual_num == list.list[list.list.Count - 1].virtual_num + 1)
                    {
                        temp.Add(GetCopyList(list.list));
                        temp[temp.Count - 1].AddRange(GetCopyList(LamiGameUIManager.Inst.mGameCardPanelList[lineNum].mGameCardList));
                    }
                    else
                    {
                        temp.Add(GetCopyList(LamiGameUIManager.Inst.mGameCardPanelList[lineNum].mGameCardList));
                        temp[temp.Count - 1].AddRange(GetCopyList(list.list));
                    }
                }
                matchNoList.Add(m_machedList.IndexOf(list));
            }
            LamiGameUIManager.Inst.uiSelectCardList.Show(temp, matchNoList);
        }
        else
        {

            foreach (var attach in m_machedList)
            {
                if (attach.lineNo == lineNum)
                {
                    SendDealtCard(attach.lineNo, attach.list);
                    break;
                }
            }


            for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
            {
                myCards[i].isSelected = false;
                myCards[i].SetUpdate();
            }

            LamiGameUIManager.Inst.playButton.gameObject.SetActive(false);
            LamiGameUIManager.Inst.tipButton.gameObject.SetActive(false);
        }
    }

    public static List<Card> GetCopyList(List<Card> list)
    {
        List<Card> new_list = new List<Card>();
        foreach (var card in list)
        {
            Card new_card = new Card(card.num, card.color);
            new_card.MyCardId = card.MyCardId;
            new_card.virtual_num = card.virtual_num;
            new_list.Add(new_card);
        }
        return new_list;
    }

    public void OnClickCardList(int listNum = -1)
    {
        LogMgr.Inst.Log("Matched List(" + listNum + ") clicked", (int)LogLevels.Lami_Second_Working);
        InitPanList();
        if (listNum == -1) return;

        SendDealtCard(m_machedList[listNum].lineNo, m_machedList[listNum].list);

        for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
        {
            myCards[i].isSelected = false;
            myCards[i].SetUpdate();
        }

        LamiGameUIManager.Inst.playButton.gameObject.SetActive(false);
        LamiGameUIManager.Inst.tipButton.gameObject.SetActive(false);
    }

    public void SendDealtCard(int v, List<Card> list)
    {
        //RemoveCards();
        DealCards(v, list);
        RemoveSentCard(list);
        LamiGameUIManager.Inst.uiSelectCardList.Hide();
    }

    private void RemoveSentCard(List<Card> list)
    {
        foreach (var card in myCards.Where(x => x.isSelected == true).ToList())
        {
            //var tmp = myCards.Where(x => (x.num == card.num && x.color == card.color) || (x.num==15 &&  card.num==15)).First();
            //myCards.Remove(tmp);
            //tmp.gameObject.SetActive(false);

            myCards.Remove(card);
            card.gameObject.SetActive(false);
        }
        selectedCards.Clear();
        LamiGameUIManager.Inst.playButton.interactable = false;
    }

    public void InitPanList() ///// Remove all cursors
    {
        for (int i = 0; i < curCursorNum; i++)
        {
            cursorPoints[i].SetActive(false);
        }
        LamiGameUIManager.Inst.InitLineNumbers();
        curCursorNum = 0;
    }

    private void ShowCursorpoint(int lineNum)
    {
        LogMgr.Inst.Log("cursor num:=" + lineNum + ", count:=" + cursorPoints.Length, (int)LogLevels.LamiFinishLog);
        // Show cursor by lineNum
        Vector3 pos = LamiGameUIManager.Inst.mGameCardPanelList[lineNum].transform.position;
        int xDiff = 75;
        //        RectTransform rect = (RectTransform) LamiGameUIManager.Inst.mGameCardPanelList[lineNum].transform;
        cursorPoints[curCursorNum].transform.position = new Vector3(pos.x + xDiff, pos.y - 15, pos.z);
        cursorPoints[curCursorNum].SetActive(true);
        LamiGameUIManager.Inst.mGameCardPanelList[lineNum].lineNum = lineNum;
        curCursorNum++;
    }
    public void Init_Clear()
    {
        sortedByColor = false;
        foreach (var card in myCards)
        {
            card.gameObject.SetActive(false);
        }
    }
}