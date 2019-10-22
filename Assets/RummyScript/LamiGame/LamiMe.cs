using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;


public class LamiMe : MonoBehaviour
{
    List<List<Card>> flushList;
    int nowFlush = 0;
    public static LamiMe Inst;

    public List<Card> m_tmpCardList = new List<Card>();
    public List<LamiMyCard> m_cardList = new List<LamiMyCard>(); // my cards
    List<LamiMyCard> sel_cards = new List<LamiMyCard>(); // selected cards

    int cardLineNumber;
    private bool IsSortClicked = false;

    List<int> avail_lineList = new List<int>();
    LamiPlayerMgr parent;

    public int status;

    private void Awake()
    {
        if (!Inst)
            Inst = this;
        status = (int)LamiPlayerStatus.Init;
    }

    internal void PublishMe()
    {
        //data : 0:id, 1:name, 2:picUrl, 3:coinValue, 4:skillLevel, 5:frameId, 6:status
        //      format: id:name:picUrl:coinValue:skillLevel:frameId:status

        LogMgr.Inst.Log("Publish Me Called. ", (int)LogLevels.MeLog);

        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                (int)PhotonNetwork.LocalPlayer.ActorNumber,
                DataController.Inst.userInfo.name,
                DataController.Inst.userInfo.pic,
                DataController.Inst.userInfo.coinValue,
                DataController.Inst.userInfo.skillLevel,
                DataController.Inst.userInfo.frameId,
                status
            );

        // Set local player's property.                    
        Hashtable props = new Hashtable
            {
                {Common.PLAYER_STATUS, (int)LamiPlayerStatus.Init},
                {Common.PLAYER_INFO, infoString}
            };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        LogMgr.Inst.Log("My Info stored in photon. " + infoString, (int)LogLevels.MeLog);
        // Send Add New player Message. - OnUserEnteredRoom

        props = new Hashtable
            {
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserEnteredRoom_M},
                {Common.NEW_PLAYER_INFO, infoString},
                {Common.NEW_PLAYER_STATUS, status}
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        LogMgr.Inst.Log("Tell I am entered. " + infoString, (int)LogLevels.RoomLog1);

    }
    public void SendMyStatus()
    {
        Hashtable props = new Hashtable{
                {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserReady},
                {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
                {Common.PLAYER_STATUS, status}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }
    public void OnReadyButtonClicked()
    {
        status = (int)LamiPlayerStatus.Ready;
        SendMyStatus();
    }

    public void SetMyCards(string data)
    {
        Card[] cards = LamiCardMgr.ConvertCardStrToCardList((string)data);

        LamiGameUIManager.Inst.myCardPanel.InitCards(cards);
        LamiGameUIManager.Inst.InitButtonsFirst();

        // Set I am ready to Start
        Hashtable props = new Hashtable{
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserReadyToStart_M},
            {Common.PLAYER_STATUS, (int)LamiPlayerStatus.ReadyToStart}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        LamiGameUIManager.Inst.myCardPanel.ArrangeMyCard();
    }
    public void SelectTipCard_Flush()
    {
        if (flushList.Count > nowFlush)
        {
            for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
            {
                LamiGameUIManager.Inst.myCardPanel.myCards[i].isSelected = false;
            }

            string log = "";
            for (int i = 0; i < flushList[nowFlush].Count; i++)
            {
                log += flushList[nowFlush][i].num;
                if (flushList[nowFlush][i].num == 15)
                    log += "(" + flushList[nowFlush][i].virtual_num + ")";
                log += ",";
                LamiGameUIManager.Inst.myCardPanel.myCards[flushList[nowFlush][i].MyCardId].isSelected = true;
            }

            for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
            {
                LamiGameUIManager.Inst.myCardPanel.myCards[i].SetUpdate();
            }

            LogMgr.Inst.Log("now tip " + log, (int)LogLevels.PlayerLog2);
        }
        LogMgr.Inst.Log("now tip turn = " + nowFlush + " / total= " + flushList.Count, (int)LogLevels.PlayerLog2);
        nowFlush++;
        nowFlush = nowFlush % flushList.Count;
    }
    public void Init_FlashList()
    {
        try
        {
            for (int i = 0; i < flushList.Count; i++)
            {
                flushList[i].Clear();
            }
            flushList.Clear();
        }
        catch { }
        nowFlush = 0;

        flushList = GetAvailableCards_Flush();
    }
    internal void SetMyTurn(bool isMyTurn)
    {
        if (isMyTurn)
        {
            LogMgr.Inst.Log("This is my turn: " + PhotonNetwork.LocalPlayer.ActorNumber, (int)LogLevels.PlayerLog2);

            LamiGameUIManager.Inst.playButton.gameObject.SetActive(true);
            //LamiGameUIManager.Inst.playButton.interactable = false;
            LamiGameUIManager.Inst.tipButton.gameObject.SetActive(true);
            //GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(true);
            //LamiCountdownTimer.Inst.StartTurnTimer();

            // Get all available cards
            // Get available Flush List
            Init_FlashList();

        }
        else
        {
            LamiGameUIManager.Inst.playButton.gameObject.SetActive(false);
            //LamiGameUIManager.Inst.playButton.interactable = false;
            LamiGameUIManager.Inst.tipButton.gameObject.SetActive(false);
            //GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(true);
            //            LamiCountdownTimer.Inst.StartTurnTimer();
        }
    }
    /************************* */



    //get selected cards are SET
    private bool IsSelSet()
    {
        int first_num = -1;
        bool isSet = false; // true: SET     false: FLUSH
        for (int i = 0; i < sel_cards.Count; i++)
        {
            if (first_num == -1) // Get the first number
            {
                first_num = Math.Abs(sel_cards[i].num);
            }
            else
            {
                isSet = true;
                if (first_num != Math.Abs(sel_cards[i].num)
                ) // If first_num <> sel_cards[i] then, this line is not SET. it's FLUSH.
                    isSet = false;
                break;
            }
        }

        return isSet;
    }

    //get selected cards are FLUSH
    private bool IsSelFlush()
    {
        int first_num = -1;
        int first_col = -1; // true: FLUSH
        bool isFlush = false;
        for (int i = 0; i < sel_cards.Count; i++)
        {
            if (first_num == -1) // Get the first number
            {
                first_num = Math.Abs(sel_cards[i].num) - 1;
                first_col = sel_cards[i].color;
            }
            else
            {
                isFlush = true;
                first_num++;
                if (first_num != Math.Abs(sel_cards[i].num) || first_col != sel_cards[i].color
                ) // If first_num <> sel_cards[i] then, this line is not FLUSH.
                    isFlush = false;
                break;
            }
        }

        return isFlush;
    }

    private void Sort()
    {
        // Sort cards
        // If this line is SET, we will not sort.
        if (IsSelSet()) return;

        LamiMyCard[] array = sel_cards.ToArray();
        sel_cards.Clear();
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (Math.Abs(array[i].num) > Math.Abs(array[j].num))
                {
                    //SwitchCard(array[i], array[j]);
                }
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            sel_cards.Add(array[i]);
        }
    }

    public List<List<Card>> GetAvailableCards_Flush()
    {
        m_tmpCardList.Clear();

        for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
        {
            Card card = new Card();
            card.color = LamiGameUIManager.Inst.myCardPanel.myCards[i].color;
            card.num = LamiGameUIManager.Inst.myCardPanel.myCards[i].num;
            card.MyCardId = i;
            card.virtual_num = card.num;
            m_tmpCardList.Add(card);
        }

        List<List<List<int>>> continue_intList = new List<List<List<int>>>();
        List<List<List<Card>>> continue_List = new List<List<List<Card>>>();

        //init
        List<List<int>> t0 = new List<List<int>>();
        List<int> t1 = new List<int>();
        t1.Add(m_tmpCardList[0].num);
        t0.Add(t1);
        continue_intList.Add(t0);

        List<List<Card>> t00 = new List<List<Card>>();
        List<Card> t11 = new List<Card>();
        t11.Add(m_tmpCardList[0]);
        t00.Add(t11);
        continue_List.Add(t00);

        for (int i = 0; i < m_tmpCardList.Count; i++)
        {
            if (m_tmpCardList[i].num == 15) continue;
            bool canInsert = true;
            for (int j = 0; j < continue_intList[0].Count; j++)
            {
                if (continue_intList[0][j].Contains(m_tmpCardList[i].num))
                {
                    canInsert = false;
                }
            }
            if (canInsert)
            {
                List<int> tt1 = new List<int>();
                tt1.Add(m_tmpCardList[i].num);

                continue_intList[0].Add(tt1);


                List<Card> tt11 = new List<Card>();
                tt11.Add(m_tmpCardList[i]);
                continue_List[0].Add(tt11);
            }
        }

        // Inserting Joker
        int count = continue_intList[0].Count;
        for (int j = 0; j < count; j++)
        {
            for (int i = 0; i < m_tmpCardList.Where(x => x.num == 15).Count(); i++)
            {
                List<int> tt1 = continue_intList[0][j].ToList();
                int firstValue = continue_intList[0][j][continue_intList[0][j].Count - 1];
                firstValue = firstValue - (i + 1);

                if (firstValue > 0)
                {
                    List<int> insert_intList = new List<int>();
                    for (int k = 0; k < i + 1; k++)
                    {
                        insert_intList.Add(firstValue + k);
                    }

                    tt1.InsertRange(0, insert_intList);
                    continue_intList[0].Add(tt1);

                    List<Card> tt11 = continue_List[0][j].ToList();
                    List<Card> insert_List = new List<Card>();
                    for (int k = 0; k < i + 1; k++)
                    {
                        var j_card = m_tmpCardList.Where(x => x.num == 15).ToList()[k];
                        j_card.virtual_num = firstValue + k;
                        insert_List.Add(j_card);
                    }
                    tt11.InsertRange(0, insert_List);
                    continue_List[0].Add(tt11);
                }

            }
        }


        bool canContinue = true;
        int level = 0;
        while (canContinue)
        {
            canContinue = false;
            level++;
            List<List<int>> first_int = new List<List<int>>();
            List<List<Card>> first_tmpCard = new List<List<Card>>();
            continue_intList.Add(first_int);
            continue_List.Add(first_tmpCard);
            count = continue_List[level - 1].Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < m_tmpCardList.Count; j++)
                {
                    if (!continue_intList[level - 1][i].Contains(m_tmpCardList[j].num) &&
                        (continue_intList[level - 1][i].Max() + 1 == m_tmpCardList[j].num || (continue_intList[level - 1][i].Max() + 1 == 14 && m_tmpCardList[j].num == 1))
                        && continue_List[level - 1][i][0].color == m_tmpCardList[j].color)
                    {
                        var tmp1 = continue_intList[level - 1][i].ToList();
                        tmp1.Add(m_tmpCardList[j].num);
                        continue_intList[level].Add(tmp1);

                        var tmp2 = continue_List[level - 1][i].ToList();
                        tmp2.Add(m_tmpCardList[j]);
                        continue_List[level].Add(tmp2);
                        canContinue = true;
                        break;
                    }
                }

                if (continue_List[level - 1][i].Count(x => x.num == 15) < m_tmpCardList.Count(x => x.num == 15))
                {
                    var tmp2 = continue_List[level - 1][i].ToList();
                    Card sel_joker = new Card();
                    bool isSelectJoker = false;
                    for (int tt = 0; tt < m_tmpCardList.Count && !isSelectJoker; tt++)
                    {
                        if (m_tmpCardList[tt].num == 15) continue;
                        for (int kk = 0; kk < tmp2.Count && !isSelectJoker; kk++)
                        {
                            if (tmp2[kk].MyCardId != m_tmpCardList[tt].MyCardId)
                            {
                                sel_joker.color = m_tmpCardList[tt].color;
                                sel_joker.num = m_tmpCardList[tt].num;
                                sel_joker.MyCardId = m_tmpCardList[tt].MyCardId;
                                isSelectJoker = true;
                            }
                        }
                    }

                    if (isSelectJoker)
                    {
                        var tmp1 = continue_intList[level - 1][i].ToList();
                        tmp1.Add(tmp1.Max() + 1);
                        continue_intList[level].Add(tmp1);
                        sel_joker.virtual_num = tmp1.Max();
                        tmp2.Add(sel_joker);
                        continue_List[level].Add(tmp2);

                        canContinue = true;

                        break;
                    }
                }
            }
        }


        List<List<Card>> resList = new List<List<Card>>();

        for (int i = continue_List.Count - 1; i >= 0; i--)
        {
            for (int j = continue_List[i].Count - 1; j >= 0; j--)
            {
                var tmp = continue_List[i][j].ToList();
                resList.Add(tmp);
            }
        }

        resList.Sort((a, b) => b.Count - a.Count);
        return resList;
    }

    public void OnSelectedCard() //If one card is selected, this function should be called.
    {
        avail_lineList.Clear();

        // Get Selected Cards

        for (int i = 0; i < m_cardList.Count; i++)
        {
            if (m_cardList[i].isSelected)
            {
                sel_cards.Add(m_cardList[i]);
            }
        }

        Sort();

        // Get Available Line Number
        // for (int i = 0; i < parent.panMgr.m_cardLineList.Count; i++)
        // {
        //     if (GetAvilableLineNum(sel_cards, panMgr.m_cardLineList[i])) // condition)
        //     {
        //         avail_lineList.Add(i);
        //     }
        // }
    }

    public bool GetAvilableLineNum(List<LamiMyCard> sel_cards, LamiCardLine cardLine)
    {

        // selected card count is only "1"
        if (sel_cards.Count == 1)
        {
            if (sel_cards[0].color == 0) return true; // If selected card is Joker, return True

            if (cardLine.isSet) // card line is SET
            {
                //If Any one Number is Same, return True
                if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].num)) return true;
            }
            else // card Line is FLUSH
            {
                // Color is Same Or Joker
                if (cardLine.m_cardList[0].color == sel_cards[0].color || sel_cards[0].color == 0)
                {
                    // If the last Number of CardLine is "-1" than Selected card Or the first Number of CardLine is "+1"  , return True 
                    if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].num) + 1 ||
                        cardLine.m_cardList[cardLine.m_cardList.Count - 1].number ==
                        Math.Abs(sel_cards[0].num) - 1)
                        return true;
                    // If the last number of CardLine is "K" and Selected card is "A", return True;
                    if (cardLine.m_cardList[cardLine.m_cardList.Count - 1].number == 13 &&
                        Math.Abs(sel_cards[0].num) == 1) return true;
                }
                else
                    return false;
            }
        }
        else // selected card count is more than "2"
        {
            // If Colors are same

            // If CardLine and SelectedCards are SET
            if (cardLine.isSet && IsSelSet() && cardLine.m_cardList[0].color == sel_cards[0].color &&
                cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].num)) return true;

            //If CardLine and SelectedCards are FLUSH and Colors are same
            if (!cardLine.isSet && IsSelFlush() && cardLine.m_cardList[0].color == sel_cards[0].color)
            {
                if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[sel_cards.Count - 1].num) + 1 ||
                    cardLine.m_cardList[cardLine.m_cardList.Count - 1].number ==
                    Math.Abs(sel_cards[0].num) - 1)
                    return true;
            }
        }

        return false;
    }



    public void OnPlayButtonClick()
    {
        if (avail_lineList.Count > 0)
        {
            cardLineNumber = avail_lineList[0];
            DealCards();
        }
    }

    public void OnLineClick(int no)
    {
        if (avail_lineList.Contains(no))
        {
            cardLineNumber = no;
            DealCards();
        }
    }

    public void OnTipButtonClick()
    {

    }

    public void GetAvaiableCards()  // Get the cards we can deal.
    {

        /*
                // Get Continuous Cards (without JOKER)
                List<List<tmpCard>> continue_List = new List<List<tmpCard>>();
                for (int c = 0; c < 4; c++)
                {
                    for (int i = 1; i <= 13; i++)
                    {
                        if (m_cardList[i].color != c)
                            continue;
                        List<tmpCard> list = GetContinueCard(i, c);

                        if (list.Count > 0)
                            continue_List.Add(list);
                        if (list.Count > 1)
                            i += list.Count;
                    }
                }

                // Get Same Cards
                List<List<tmpCard>> same_List = new List<List<tmpCard>>();

                for (int j = 0; j < m_cardList.Count - 1; j++)
                {
                    List<tmpCard> list = new List<tmpCard>();
                    tmpCard card = new tmpCard();
                    card.MyCardId = j;
                    card.color = m_cardList[j].color;
                    card.number = m_cardList[j].num;
                    list.Add(card);


                    for (int i = j + 1; i < m_cardList.Count; i++)
                    {
                        if (m_cardList[i].num == m_cardList[j].num)
                        {
                            tmpCard card1 = new tmpCard();
                            card1.MyCardId = i;
                            card1.color = m_cardList[i].color;
                            card1.number = m_cardList[i].num;
                            list.Add(card1);
                        }
                    }

                    bool alreadyIn = true;
                    for (int k = 0; k < same_List.Count; k++)
                    {
                        if (same_List[k][0].number == list[0].number)
                        {
                            alreadyIn = false;
                        }
                    }
                    if (alreadyIn)
                        same_List.Add(list);
                }
        */
        // 
    }



    private List<Card> GetContinueCard(int first, int c)
    {

        List<Card> res = new List<Card>();
        // for (int i = first; i <= 13; i++)
        // {
        //     bool isFound = false;
        //     for (int j = 0; j < m_cardList.Count; j++)
        //     {
        //         if (m_cardList[j].color == c && m_cardList[j].num == i)
        //         {
        //             Card card = new Card();
        //             card.MyCardId = j;
        //             card.number = i;
        //             card.color = c;
        //             res.Add(card);
        //             isFound = true;
        //             break;
        //         }
        //     }
        //     if (isFound != true)
        //     {
        //         break;
        //     }
        // }
        return res;
    }

    private List<Card> ArrangeWithColor() // Get Arranged CardList
    {
        List<Card> resList = new List<Card>();

        // for (int i = 0; i < m_cardList.Count; i++)
        // {
        //     Card item = new Card();
        //     item.MyCardId = i;
        //     item.number = m_cardList[i].num;
        //     item.color = m_cardList[i].color;
        //     resList.Add(item);
        // }

        // Card[] array = resList.ToArray();
        // resList.Clear();
        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }

        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].color) > Math.Abs(array[j].color))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }


        // for (int i = 0; i < array.Length; i++)
        // {
        //     resList.Add(array[i]);
        // }

        return resList;
    }

    private List<Card> ArrangeWithNumber()
    {
        List<Card> resList = new List<Card>();

        // for (int i = 0; i < m_cardList.Count; i++)
        // {
        //     Card item = new Card();
        //     item.MyCardId = i;
        //     item.number = m_cardList[i].num;
        //     item.color = m_cardList[i].color;
        //     resList.Add(item);
        // }

        // Card[] array = resList.ToArray();
        // resList.Clear();
        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < array.Length; j++)
        //     {
        //         if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
        //         {
        //             int MyCardId, number, color;
        //             MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
        //             array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
        //             array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
        //         }
        //     }
        // }

        // for (int i = 0; i < array.Length; i++)
        // {
        //     resList.Add(array[i]);
        // }

        return resList;
    }
    public void OnArrangeButtonClick() // Arrange Button Is Clicked
    {
        IsSortClicked = (!IsSortClicked);

        if (IsSortClicked) // If Arrange Button Clicked, Arrange with Number Size 
        {
            ArrangeWithNumber();
        }
        else // If Arrange Button Not Clicked, Arrange with Color Size
        {
            ArrangeWithColor();
        }
    }

    public void DealCards()
    {
        string cards = "";
        for (int i = 0; i < m_cardList.Count; i++)
        {
            if (m_cardList[i].isSelected)
            {
                cards = string.Format("{0}:{1}", m_cardList[i].color, m_cardList[i].num);
                m_cardList.Remove(m_cardList[i]);
            }
        }

        // selected card clear
        sel_cards.Clear();

        string card_data = cardLineNumber + "-" + cards;
        cardLineNumber = -1;
        // Send Message
    }

    // receive my status(init, thinking, ready, giveup, burnt, game), Other Action(buttons, show)
    public void SetMyStatus(int status)
    {
        switch (status)
        {
            case 0: // init
                break;
            case 1: // thinking
                break;
            case 2: // ready
                break;
            case 3: // give up
                break;
            case 4: // burnt
                break;
            case 5: // game
                break;
        }
    }


}
