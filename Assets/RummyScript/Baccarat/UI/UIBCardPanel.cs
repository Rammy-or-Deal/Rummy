using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBCardPanel : MonoBehaviour
{
    public UIBCard[][] cards;
    public UIBCard[] leftCards;
    public UIBCard[] rightCards;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void Init()
    {
        foreach(var card in leftCards)
        {
            card.Init();
        }
        leftCards[leftCards.Length-1].Init(false);
        foreach(var card in rightCards)
        {
            card.Init();
        }
        rightCards[rightCards.Length-1].Init(false);
    }
}
