﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIMyCardPanel : MonoBehaviour
{
    //MyCard
    public Dictionary<int, List<Card>> attachList = new Dictionary<int, List<Card>>();
    public List<LamiMyCard> myCards;
    public GameObject[] cursorPoints;
    private int curCursorNum = 0;
    public bool sortedByColor;
    [HideInInspector] public List<LamiMyCard> selectedCards;

    // Start is called before the first frame update
    void Start()
    {
        selectedCards = new List<LamiMyCard>();
        sortedByColor = false;
    }

    public void InitCards(Card[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            LamiMyCard cardEntry = myCards[i];
            cardEntry.num = cards[i].num;
            cardEntry.color = cards[i].color;
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



    public void SetPlayButtonState()
    {
        int count = LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true);    // Get Selected Card




        attachList.Clear();


        if (count == 0)
        {
            InitPanList();
            LamiGameUIManager.Inst.playButton.interactable = false;
        }


        LogMgr.Inst.Log("------------------------------------ Line List---------------------------------- ---", (int)LogLevels.SpecialLog);
        //foreach (var line in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        foreach (var line in LamiMe.Inst.availList)
        {
            string tmp = "";
            foreach (var card in line)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            LogMgr.Inst.Log(tmp, (int)LogLevels.SpecialLog);
        }
        LogMgr.Inst.Log("---- LineEnd ---", (int)LogLevels.SpecialLog);



        var selectedList = LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList();

        LogMgr.Inst.Log("------------------------------------ Selected Line List ------------------------------", (int)LogLevels.SpecialLog);
        //foreach (var line in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        foreach (var line in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        {
            string tmp = "";
            foreach (var card in line)
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

        LogMgr.Inst.Log("selectedCard: " + log, (int)LogLevels.SpecialLog);


        List<List<Card>> m_machedList = new List<List<Card>>();

        // Get all mached lists



        foreach (var org in LamiMe.Inst.availList.Where(x => x.Count == count).ToList())
        {
            bool wrong = true;

            foreach (var card in selectedList)
            {
                wrong = false;
                if (org.Count(x => (x.virtual_num == card.num && x.color == card.color) || (card.num == 15) || (card.num == 1 && x.virtual_num == -1 && x.color == card.color)) == 0)
                {
                    wrong = true;
                    break;
                }
            }
            if (wrong == false)
            {
                foreach (var card in org)
                {
                    wrong = false;
                    if (selectedList.Count(x => (x.num == card.virtual_num && x.color == card.color) || (x.num == 15) || (x.num == 1 && card.virtual_num == -1 && x.color == card.color)) == 0)
                    {
                        wrong = true;
                        break;
                    }
                }
            }

            if (wrong == false)
            {
                List<Card> new_list = new List<Card>();
                new_list.AddRange(org);
                m_machedList.Add(new_list);
            }
        }


        InitPanList();  // Remove all cursors

        // Check if it's matched pan game cards.

        bool canAttach = false;
        LogMgr.Inst.Log("--------------  PAN LINES   -------------------", (int)LogLevels.SpecialLog);
        List<int> cursorList = new List<int>();

        for (int i = 0; i < LamiGameUIManager.Inst.mGameCardPanelList.Count; i++)
        {
            string tmpStr = i + ":";
            foreach (var aLine in LamiGameUIManager.Inst.mGameCardPanelList[i].mGameCardList)
            {
                tmpStr += aLine.num + "(" + aLine.virtual_num + "):" + aLine.color + ",";
            }
            LogMgr.Inst.Log(tmpStr, (int)LogLevels.SpecialLog);

            var line = LamiGameUIManager.Inst.mGameCardPanelList[i].mGameCardList;

            bool isFlush = true;
            int firstColor = line[0].color;
            foreach (var card in line)
            {
                if (card.color != firstColor)
                {
                    isFlush = false;
                    break;
                }
            }

            for (int j = 0; j < m_machedList.Count; j++)
            {

                bool isFlush_created = true;
                int firstNum = m_machedList[j][0].num;

                foreach (var card in m_machedList[j])
                {
                    if (card.num != firstNum)
                    {
                        isFlush_created = false;
                        break;
                    }
                }

                if (line.Count == 0)
                {
                    Debug.Log(i + " st line has no element");
                    break;
                }

                var list = m_machedList[j];
                string tmp = "";
                foreach (var card in list)
                {
                    tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
                }
                LogMgr.Inst.Log(j + " st matched line=" + tmp, (int)LogLevels.SpecialLog);


                Debug.Log("Check - isFlush: " + (isFlush) + ",  isFlush_Created:" + isFlush_created);

                Debug.Log("Check1" + (((m_machedList[j][m_machedList[j].Count - 1].virtual_num == line[0].virtual_num - 1 || // can attach  dealt card to first
                       m_machedList[j][0].virtual_num == line[line.Count - 1].virtual_num + 1) &&   // can attach  dealt card to end
                       isFlush && m_machedList[j][0].color == line[0].color && isFlush_created)));

                Debug.Log("Check2" + (m_machedList[j].Count == 1 && m_machedList[j][0].virtual_num == line[0].virtual_num
                                           && m_machedList[j][0].virtual_num == line[1].virtual_num));

                Debug.Log("Check3" + ((m_machedList[j].Count > 1 && m_machedList[j][0].virtual_num == line[line.Count - 1].virtual_num
                                          && m_machedList[j][0].virtual_num == line[1].virtual_num
                                          && m_machedList[j][1].virtual_num == line[line.Count - 1].virtual_num && !isFlush && !isFlush_created)));

                if (((m_machedList[j][m_machedList[j].Count - 1].virtual_num == line[0].virtual_num - 1 || // can attach  dealt card to first
                       m_machedList[j][0].virtual_num == line[line.Count - 1].virtual_num + 1) &&   // can attach  dealt card to end
                       isFlush && m_machedList[j][0].color == line[0].color && isFlush_created) ||       // can attach to flush line

                    (m_machedList[j].Count == 1 && m_machedList[j][0].virtual_num == line[0].virtual_num
                                           && m_machedList[j][0].virtual_num == line[1].virtual_num) ||

                    (m_machedList[j].Count > 1 && m_machedList[j][0].virtual_num == line[line.Count - 1].virtual_num
                                          && m_machedList[j][0].virtual_num == line[1].virtual_num
                                          && m_machedList[j][1].virtual_num == line[line.Count - 1].virtual_num && !isFlush && !isFlush_created))    // can attach in set list
                {
                    canAttach = true;
                    if (!cursorList.Contains(i))
                        cursorList.Add(i);

                    try
                    {
                        attachList.Add(i, m_machedList[j].ToList());
                    }
                    catch { }
                    LogMgr.Inst.Log("Show Cursor: " + i, (int)LogLevels.PlayerLog2);
                }
            }
        }
        LogMgr.Inst.Log("--------------  PAN LINES END  -------------------", (int)LogLevels.SpecialLog);

        foreach (var cursorPos in cursorList)
        {
            ShowCursorpoint(cursorPos);
        }


        LogMgr.Inst.Log("---- m_machedList ---", (int)LogLevels.SpecialLog);
        for (int i = 0; i < m_machedList.Count; i++)
        {
            var list = m_machedList[i];
            string tmp = "";
            foreach (var card in list)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            LogMgr.Inst.Log(tmp, (int)LogLevels.SpecialLog);
        }
        LogMgr.Inst.Log("---- end ---", (int)LogLevels.SpecialLog);

        if (canAttach == false)
        {
            // Send new line.
            if (m_machedList.Count(x => x.Count >= 3) > 0)
            {
                attachList.Add(-1, m_machedList.Where(x => x.Count >= 3).First());
                canAttach = true;
            }
            //SendDealtCard(-1, m_machedList.Where(x=>x.Count>=3).First());
        }

        LogMgr.Inst.Log("---- attachList ---", (int)LogLevels.SpecialLog);
        for (int i = 0; i < attachList.Count; i++)
        {
            var list = attachList.ElementAt(i).Value;
            string tmp = "";
            foreach (var card in list)
            {
                tmp += card.num + "(" + card.virtual_num + ")" + ",";
            }
            LogMgr.Inst.Log(tmp, (int)LogLevels.SpecialLog);
        }
        LogMgr.Inst.Log("---- end ---", (int)LogLevels.SpecialLog);


        LamiGameUIManager.Inst.playButton.interactable = (attachList.Count > 0);
    }
    public void OnClickLine(int lineNum = -1)
    {
        InitPanList();
        if (attachList.Count > 1 && lineNum == -1) return;


        SendDealtCard(attachList.First().Key, attachList[attachList.First().Key]);

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
    {  // Show cursor by lineNum
        Vector3 pos = LamiGameUIManager.Inst.mGameCardPanelList[lineNum].transform.position;
        int xDiff = 60;
        //        RectTransform rect = (RectTransform) LamiGameUIManager.Inst.mGameCardPanelList[lineNum].transform;
        cursorPoints[curCursorNum].transform.position = new Vector3(pos.x + xDiff, pos.y, pos.z);
        cursorPoints[curCursorNum].SetActive(true);
        LamiGameUIManager.Inst.mGameCardPanelList[lineNum].lineNum = lineNum;
        curCursorNum++;
    }
}