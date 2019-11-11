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

    internal void Init(FortuneUserSeat seat)
    {
        if ((int)PhotonNetwork.PlayerList.Where(x => x.ActorNumber == seat.actorNumber).First().CustomProperties[Common.PLAYER_STATUS] == (int)FortunePlayerStatus.Init)
            IsSeat = false;
        else
            IsSeat = seat.IsSeat;
        
            
        actorNumber = seat.actorNumber;
        
        avatar.sprite = seat.mUserPic.sprite;

        frameImg.sprite = seat.mUserFrame.sprite;
        missionImg.gameObject.SetActive(false);

        for(int i = 0; i < seat.frontCards.Length; i++)
            frontCards[i].SetValue(seat.frontCards[i].GetValue());
        for(int i = 0; i < seat.middleCards.Length; i++)
            middleCards[i].SetValue(seat.middleCards[i].GetValue());
        for(int i = 0; i < seat.backCards.Length; i++)
            backCards[i].SetValue(seat.backCards[i].GetValue());
    }

    internal void SetProperty(UIFCalcPlayer uIFCalcPlayer)
    {
        if(uIFCalcPlayer.totalCoin < 0)
            resultText.text = uIFCalcPlayer.totalCoin.ToString();
        else
            resultText.text = (uIFCalcPlayer.totalCoin*0.9).ToString();
    }
}
