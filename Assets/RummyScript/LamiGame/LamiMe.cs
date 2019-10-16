using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class tmpCard
    {   // This is temp class for MyCard
        public int MyCardId { get; set; }
        public int number { get; set; }
        public int color { get; set; }
    }
    public class LamiMe
    {
        List<LamiMyCard> m_cardList = new List<LamiMyCard>(); // my cards
        List<LamiMyCard> sel_cards = new List<LamiMyCard>(); // selected cards

        int cardLineNumber;
        private bool IsSortClicked = false;

        List<int> avail_lineList = new List<int>();
        LamiPlayerMgr parent;

        int status = (int)LamiPlayerStatus.Init;
        public LamiMe(LamiPlayerMgr parent)
        {
            this.parent = parent;
            status = (int)LamiPlayerStatus.Init;
        }

        internal void SendMyInfo()
        {
            string infoString = "";
            infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                    0,
                    DataController.Inst.userInfo.name,
                    DataController.Inst.userInfo.pic,
                    DataController.Inst.userInfo.coinValue,
                    DataController.Inst.userInfo.skillLevel,
                    DataController.Inst.userInfo.frameId
                    );
            Hashtable props = new Hashtable
            {
                {Common.PLAYER_STATUS, (int)LamiPlayerStatus.Init},
                {Common.PLAYER_INFO, infoString}
            };
    
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        /************************* */

        public void SetMyCards(string data)
        {
            var cards = data.Split(',');
            // data format: color:number,color:number,...
            for (int i = 0; i < cards.Length; i++)
            {
                var tmp = cards[i].Split(':');
                LamiMyCard card = new LamiMyCard(int.Parse(tmp[0]), int.Parse(tmp[1]));
                m_cardList.Add(card);
            }
        }

        //get selected cards are SET
        private bool IsSelSet()
        {
            int first_num = -1;
            bool isSet = false; // true: SET     false: FLUSH
            for (int i = 0; i < sel_cards.Count; i++)
            {
                if (first_num == -1) // Get the first number
                {
                    first_num = Math.Abs(sel_cards[i].number);
                }
                else
                {
                    isSet = true;
                    if (first_num != Math.Abs(sel_cards[i].number)
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
                    first_num = Math.Abs(sel_cards[i].number) - 1;
                    first_col = sel_cards[i].color;
                }
                else
                {
                    isFlush = true;
                    first_num++;
                    if (first_num != Math.Abs(sel_cards[i].number) || first_col != sel_cards[i].color
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
                    if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
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
                    if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].number)) return true;
                }
                else // card Line is FLUSH
                {
                    // Color is Same Or Joker
                    if (cardLine.m_cardList[0].color == sel_cards[0].color || sel_cards[0].color == 0)
                    {
                        // If the last Number of CardLine is "-1" than Selected card Or the first Number of CardLine is "+1"  , return True 
                        if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].number) + 1 ||
                            cardLine.m_cardList[cardLine.m_cardList.Count - 1].number ==
                            Math.Abs(sel_cards[0].number) - 1)
                            return true;
                        // If the last number of CardLine is "K" and Selected card is "A", return True;
                        if (cardLine.m_cardList[cardLine.m_cardList.Count - 1].number == 13 &&
                            Math.Abs(sel_cards[0].number) == 1) return true;
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
                    cardLine.m_cardList[0].number == Math.Abs(sel_cards[0].number)) return true;

                //If CardLine and SelectedCards are FLUSH and Colors are same
                if (!cardLine.isSet && IsSelFlush() && cardLine.m_cardList[0].color == sel_cards[0].color)
                {
                    if (cardLine.m_cardList[0].number == Math.Abs(sel_cards[sel_cards.Count - 1].number) + 1 ||
                        cardLine.m_cardList[cardLine.m_cardList.Count - 1].number ==
                        Math.Abs(sel_cards[0].number) - 1)
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
                card.number = m_cardList[j].number;
                list.Add(card);


                for (int i = j + 1; i < m_cardList.Count; i++)
                {
                    if (m_cardList[i].number == m_cardList[j].number)
                    {
                        tmpCard card1 = new tmpCard();
                        card1.MyCardId = i;
                        card1.color = m_cardList[i].color;
                        card1.number = m_cardList[i].number;
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

            // 
        }



        private List<tmpCard> GetContinueCard(int first, int c)
        {
            List<tmpCard> res = new List<tmpCard>();
            for (int i = first; i <= 13; i++)
            {
                bool isFound = false;
                for (int j = 0; j < m_cardList.Count; j++)
                {
                    if (m_cardList[j].color == c && m_cardList[j].number == i)
                    {
                        tmpCard card = new tmpCard();
                        card.MyCardId = j;
                        card.number = i;
                        card.color = c;
                        res.Add(card);
                        isFound = true;
                        break;
                    }
                }
                if (isFound != true)
                {
                    break;
                }
            }
            return res;
        }

        private List<tmpCard> ArrangeWithColor() // Get Arranged CardList
        {
            List<tmpCard> resList = new List<tmpCard>();

            for (int i = 0; i < m_cardList.Count; i++)
            {
                tmpCard item = new tmpCard();
                item.MyCardId = i;
                item.number = m_cardList[i].number;
                item.color = m_cardList[i].color;
                resList.Add(item);
            }

            tmpCard[] array = resList.ToArray();
            resList.Clear();
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
                    {
                        int MyCardId, number, color;
                        MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
                        array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
                        array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
                    }
                }
            }

            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (Math.Abs(array[i].color) > Math.Abs(array[j].color))
                    {
                        int MyCardId, number, color;
                        MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
                        array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
                        array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
                    }
                }
            }






            for (int i = 0; i < array.Length; i++)
            {
                resList.Add(array[i]);
            }

            return resList;
        }

        private List<tmpCard> ArrangeWithNumber()
        {
            List<tmpCard> resList = new List<tmpCard>();

            for (int i = 0; i < m_cardList.Count; i++)
            {
                tmpCard item = new tmpCard();
                item.MyCardId = i;
                item.number = m_cardList[i].number;
                item.color = m_cardList[i].color;
                resList.Add(item);
            }

            tmpCard[] array = resList.ToArray();
            resList.Clear();
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
                    {
                        int MyCardId, number, color;
                        MyCardId = array[i].MyCardId; color = array[i].color; number = array[i].number;
                        array[i].MyCardId = array[j].MyCardId; array[i].color = array[j].color; array[i].number = array[j].number;
                        array[j].MyCardId = MyCardId; array[j].color = color; array[j].number = number;
                    }
                }
            }

            for (int i = 0; i < array.Length; i++)
            {
                resList.Add(array[i]);
            }

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
                    cards = string.Format("{0}:{1}", m_cardList[i].color, m_cardList[i].number);
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
}