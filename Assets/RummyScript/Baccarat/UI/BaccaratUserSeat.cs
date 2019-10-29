﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class BaccaratUserSeat : MonoBehaviour
{
    //user info
    public Image mUserFrame;
    public Image mUserPic;
    public Image mUserSkillPic;
    public Text mUserName;
    public Text mUserSkillName;
    public Text mCoinValue;
    //seat state
    public Image firstImage;
    public GameObject userBack;
    //
    public int id;
    public bool isSeat;
    public int actorNumber;
    #region UNITY   
    
    public void Start()
    {
//        mUserPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
//        mUserName.text = DataController.Inst.userInfo.name;
//        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
//        mUserSkillName.text = DataController.Inst.userInfo.skillLevel;
//        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
    }
    public void Show(Player p)
    {
        actorNumber = p.ActorNumber;
        mUserName.text = p.NickName;
        isSeat = true;

        object playerPic;
        object playerLevel;
        object playerCoin;
        if (p.CustomProperties.TryGetValue(Common.PLAYER_PIC, out playerPic))
        {
            mUserPic.sprite = Resources.Load<Sprite>((string) playerPic);
        }

        if (p.CustomProperties.TryGetValue(Common.PLAYER_LEVEL, out playerLevel))
        {
            mUserSkillName.text = (string) playerLevel;
        }

        if (p.CustomProperties.TryGetValue(Common.PLAYER_COIN, out playerCoin))
        {
            mCoinValue.text = playerCoin.ToString();
        }
        
        firstImage.gameObject.SetActive(false);
        userBack.SetActive(true);
        Debug.Log("Player//" + id);
        
        if (!BaccaratGameController.Inst.seatNumList.ContainsKey(p.ActorNumber))
        {
            BaccaratGameController.Inst.seatNumList.Add(p.ActorNumber, id);    
        }
        
    }

    public void LeftRoom() // the number of left user
    {
        isSeat = false;
        firstImage.gameObject.SetActive(true);
        userBack.SetActive(false);
    }

    private void OnPlayerNumberingChanged()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
        }
    }

    #endregion


    public void OnClick()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }
}
