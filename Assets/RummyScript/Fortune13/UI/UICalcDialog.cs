using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICalcDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    internal void ShowCards(List<Card> showList)
    {
        List<FortuneCard> cardList = new List<FortuneCard>();
        
    }
}
