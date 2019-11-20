using System;
using System.Collections.Generic;
public class LamiCardLine
{
    public List<LamiLineCard> m_cardList = new List<LamiLineCard>();

    public int lineNumber;
    public bool isSet;  // true: SET     false: FLUSH

    public LamiCardLine()
    {

    }
    public LamiCardLine(int lineNumber)
    {
        this.lineNumber = lineNumber;
    }

    public void Add(string data)
    {
        // data format: color:number, color:number, ...

        var cards = data.Split(',');

        // Erase Border
        for (int i = 0; i < m_cardList.Count; i++)
        {
            m_cardList[i].setProperty(0);
        }

        // Add new Card
        for (int i = 0; i < cards.Length; i++)
        {
            var tmp = cards[i].Split(':');
            LamiLineCard card = new LamiLineCard(int.Parse(tmp[0]), int.Parse(tmp[1]));
            m_cardList.Add(card);
        }

        // Get Line Property
        GetMyProperty();

        // Sort cards
        Sort();
    }

    public void Show()
    {
        // Show line

    }

    private void GetMyProperty()
    {
        int first_num = -1;
        for (int i = 0; i < m_cardList.Count; i++)
        {
            if (first_num == -1) // Get the first number
            {
                first_num = Math.Abs(m_cardList[i].number);
            }
            else
            {
                isSet = true;
                if (first_num != Math.Abs(m_cardList[i].number))  // If first_num <> m_cardList[i] then, this line is not SET. it's FLUSH.
                    isSet = false;
                break;
            }

        }
    }

    private void Sort()
    {
        // Sort cards
        // If this line is SET, we will not sort.
        if (isSet == true) return;

        LamiLineCard[] array = m_cardList.ToArray();
        m_cardList.Clear();
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (Math.Abs(array[i].number) > Math.Abs(array[j].number))
                {
                    int color, number, property;
                    color = array[i].color; number = array[i].number; property = array[i].property;
                    array[i].color = array[j].color; array[i].number = array[j].number; array[i].property = array[j].property;
                    array[j].color = color; array[j].number = number; array[j].property = property;
                }
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            m_cardList.Add(array[i]);
        }
    }

    internal void Init_Clear()
    {
        foreach(var card in m_cardList)
        {
            card.Init_Clear();
        }
        m_cardList.Clear();
    }
}
