using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UIFResultPlayer : MonoBehaviour
{

    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public Image missionImg;
    public Text resultText;
    public Image avatar;
    public Image frameImg;

    public int actorNumber;

    [HideInInspector]
    public bool IsSeat
    {
        get { return this.gameObject.activeSelf; }
        set { this.gameObject.SetActive(value); }
    }
    void Start()
    {

    }

    internal void Init(UserSeat _seat)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        var seat = (FortuneUserSeat)_seat;
        try
        {
            if (pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_dealtCard ||
                    pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_Doubled)
                IsSeat = seat.isSeat;
            else
                IsSeat = false;
        }
        catch { IsSeat = false; }


        actorNumber = seat.m_playerInfo.m_actorNumber;

        avatar.sprite = seat.mUserPic.sprite;

        frameImg.sprite = seat.mUserFrame.sprite;
        missionImg.gameObject.SetActive(false);

        for (int i = 0; i < seat.frontCards.Length; i++)
            frontCards[i].SetValue(seat.frontCards[i].GetValue());
        for (int i = 0; i < seat.middleCards.Length; i++)
            middleCards[i].SetValue(seat.middleCards[i].GetValue());
        for (int i = 0; i < seat.backCards.Length; i++)
            backCards[i].SetValue(seat.backCards[i].GetValue());
    }

    internal void SetProperty(UIFCalcPlayer uIFCalcPlayer)
    {
        if (uIFCalcPlayer.totalCoin < 0)
            resultText.text = uIFCalcPlayer.totalCoin.ToString();
        else
            resultText.text = (uIFCalcPlayer.totalCoin * 0.9).ToString();
    }
}
