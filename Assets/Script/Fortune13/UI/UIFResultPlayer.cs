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

    public void Init(UserSeat _seat)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        var seat = (FortuneUserSeat)_seat;
        try
        {
            if (pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_dealtCard ||
                    pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_Doubled ||
                    pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_Lucky ||
                    pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First().m_status == enumPlayerStatus.Fortune_OnChanging)
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

    public virtual void SetProperty(UIFCalcPlayer uIFCalcPlayer)
    {
        if (uIFCalcPlayer.totalCoin < 0)
            resultText.text = uIFCalcPlayer.totalCoin.ToString();
        else
            resultText.text = (uIFCalcPlayer.totalCoin * 0.9).ToString();

        changeTextColorByScore(resultText);
        
    }

    private void changeTextColorByScore(Text resultText)
    {
        if(int.Parse(resultText.text) == 0) resultText.color = Color.white;
        if(int.Parse(resultText.text) > 0) resultText.color = Color.green;
        if(int.Parse(resultText.text) < 0) resultText.color = Color.red;
    }

    public void SetLuckyScore(int score)
    {
        resultText.text = score.ToString();
        changeTextColorByScore(resultText);
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
