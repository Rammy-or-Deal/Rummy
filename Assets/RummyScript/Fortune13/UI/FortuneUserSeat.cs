using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class FortuneUserSeat : CommonSeat
{
    
    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    internal void InitCards()
    {
        foreach(var card in myCards)
        {
            card.Init(false);
        }
    }

    internal async void moveDealCard(Vector3 srcPos)
    {
        foreach(var card in myCards)
        {
            await Task.Delay(500);
            card.moveDealCard(srcPos);
        }
    }

    //seat state
    //

 
    #region UNITY

    public void Start()
    {
        
        mUserPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
        mUserName.text = DataController.Inst.userInfo.name;
        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
        mUserSkillName.text = DataController.Inst.userInfo.skillLevel;
        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();

        MyObject = this.gameObject;
    }


    public void LeftRoom() // the number of left user
    {
        isSeat = false;
    }


    #endregion


    public void OnClick()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }

}
