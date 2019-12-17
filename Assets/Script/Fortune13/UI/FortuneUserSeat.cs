using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class FortuneUserSeat : UserSeat
{

    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    internal void InitCards()
    {
        foreach (var card in myCards)
        {
            card.Init(false);
        }
    }

    internal void moveDealCard(Vector3 srcPos)
    {
        foreach (var card in myCards)
        {
            //await Task.Delay(500);
            card.moveDealCard(srcPos);
        }
    }

    //seat state
    //


    #region UNITY

    public void LeftRoom() // the number of left user
    {
        isSeat = false;
    }


    #endregion


    public void OnClick()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }

    internal void ShowCards(int lineNo, List<Card> showList)
    {
        
        switch (lineNo)
        {
            case 0:
                for (int i = 0; i < showList.Count; i++)
                {
                    frontCards[i].SetValue(showList[i]);
                }
                break;
            case 1:
                for (int i = 0; i < showList.Count; i++)
                {
                    middleCards[i].SetValue(showList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < showList.Count; i++)
                {
                    backCards[i].SetValue(showList[i]);
                }
                break;
        }
    }
}
