using System;
using System.Collections.Generic;
using UnityEngine;
public class LamiPanMgr : MonoBehaviour
{
    public List<LamiCardLine> m_cardLineList = new List<LamiCardLine>();

    public static LamiPanMgr Inst;
    private void Awake()
    {
        if (!Inst)
            Inst = this;
    }
    public void Add(string data)
    {
        // data format: lineNumber-color:number,color:number,...
        var lineNumber = int.Parse(data.Split('-')[0]);
        var cards = data.Split('-')[1];

        LamiCardLine line;

        if (lineNumber != 0)    // if the dealed card is in existing line
        {
            line = m_cardLineList[lineNumber];
        }
        else     // if card is in new line
        {
            line = new LamiCardLine(m_cardLineList.Count + 1);
            m_cardLineList.Add(line);
        }
        line.Add(cards);

        // Show
        Show();
    }

    private void Show()
    {
        for (int i = 0; i < m_cardLineList.Count; i++)
        {
            m_cardLineList[i].Show();
        }
    }
}
