using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIMyCardPanel : MonoBehaviour
{
    //MyCard
    public List<LamiMyCard> myCards;
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

    public void DealCards()
    {

        //        foreach (LamiMyCard card in selectedCards)
        //        {
        //            LamiGameUIManager.Inst.AddGameCard(card);
        //        }


        string cardStr = LamiCardMgr.ConvertSelectedListToString(LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList());
        cardStr = PhotonNetwork.LocalPlayer.ActorNumber + ":" + cardStr;
        int remainCard = LamiGameUIManager.Inst.myCardPanel.myCards.Count - LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true);
        Hashtable gameCards = new Hashtable
        {
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
            {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
            {Common.REMAIN_CARD_COUNT, remainCard},
            {Common.GAME_CARD, cardStr},
            {Common.GAME_CARD_PAN, 0},
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(gameCards);
        LogMgr.Inst.Log("User dealt card: " + cardStr, (int)LogLevels.PlayerLog2);
        RemoveCards();
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
        int count = LamiGameUIManager.Inst.myCardPanel.myCards.Count(x => x.isSelected == true);
        var selectedList = LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList();
        string log = "";
        foreach (var card in selectedList)
        {
            log += card.MyCardId + "/ num=" + card.num + ", color=" + card.color + ", virtual=" + card.virtual_num + "  :  ";
        }

        LogMgr.Inst.Log("selectedCard: " + log, (int)LogLevels.PlayerLog2);
        bool isCorrect = false;

        List<List<Card>> m_machedList = new List<List<Card>>();

        // Get all mached lists
        foreach (var org in LamiMe.Inst.flushList.Where(x => x.Count == count).ToList())
        {
            bool wrong = true;

            foreach (var card in selectedList)
            {
                wrong = false;
                if (org.Count(x => x.num == card.num && x.color == card.color) == 0)
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
                    if (selectedList.Count(x => x.num == card.num && x.color == card.color) == 0)
                    {
                        wrong = true;
                        break;
                    }
                }
            }

            if (wrong == false)
            {
                isCorrect = true;
                m_machedList.Add(org);
            }
        }

        InitPanList();  // Remove all cursors

        // Check if it's matched pan game cards.
        bool canAttach = false;
        for (int i = 0; i < LamiGameUIManager.Inst.mGameCardPanelList.Count; i++)
        {
            var line = LamiGameUIManager.Inst.mGameCardPanelList[i];
            for (int j = 0; j < m_machedList.Count; j++)
            {
                if (m_machedList[j][m_machedList.Count - 1].virtual_num == line.mGameCardList[0].virtual_num || // can attach  dealt card to first
                    m_machedList[j][0].virtual_num == line.mGameCardList[line.mGameCardList.Count - 1].virtual_num)     // can attach  dealt card to end
                {
                    canAttach = true;
                    ShowCursorpoint(i);
                    LogMgr.Inst.Log("Show Cursor: " + i, (int)LogLevels.PlayerLog2);
                }
            }
        }

        if(canAttach == false)
        {
            // Send new line.
            
        }

        LamiGameUIManager.Inst.playButton.interactable = isCorrect;
    }

    private void InitPanList() ///// Remove all cursors
    {
        throw new NotImplementedException();
    }

    private void ShowCursorpoint(int lineNum){  // Show cursor by lineNum
        throw new NotImplementedException();
    }
}