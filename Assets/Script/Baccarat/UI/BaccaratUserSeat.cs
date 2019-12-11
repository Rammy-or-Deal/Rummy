using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using RummyScript.Model;
using UnityEngine;
using UnityEngine.UI;

public class BaccaratUserSeat : UserSeat
{
    //user info

    //seat state
    public Image firstImage;
    public GameObject userBack;
    //
    public int id = -1;
    public int type;

    public Transform[] cardPos;

    UserInfoModel userInfo = new UserInfoModel();

    #region UNITY   


    private void Awake()
    {
        // if (!Inst)
        //     Inst = this;
    }
    #endregion

    internal void SetMe(string infoString)
    {
        var list = infoString.Split(':');
        id = int.Parse(list[0]);
        userInfo.id = id;
        userInfo.name = list[1];
        userInfo.pic = list[2];
        userInfo.coinValue = int.Parse(list[3]);
        userInfo.skillLevel = list[4];
        userInfo.frameId = int.Parse(list[5]);
        type = int.Parse(list[6]);

        isSeat = true;

        ShowMe();
    }

    public override void SetPlayerInfo(PlayerInfo info)
    {
        base.SetPlayerInfo(info);
       
    }

    internal void OnUserLeave()
    {
        id = -1;
        isSeat = false;
        ShowMe();
    }

    private void ShowMe()
    {
        firstImage.gameObject.SetActive(!isSeat);
        userBack.SetActive(isSeat);

        if (isSeat == false) return;

        mUserPic.sprite = Resources.Load<Sprite>(userInfo.pic);
        mUserName.text = userInfo.name;
        mCoinValue.text = userInfo.coinValue.ToString();
        mUserSkillName.text = userInfo.skillLevel;
        mCoinValue.text = userInfo.coinValue.ToString();
    }

    public int OnPlayerBet()
    {
        try
        {
            string betString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NOW_BET];

            //LogMgr.Inst.Log(player.ActorNumber+"st PlayerLog:=" + (string)player.CustomProperties[Common.PLAYER_BETTING_LOG], (int)LogLevels.PlayerLog1);

            int moneyId = int.Parse(betString.Split(':')[0]);
            int areaId = int.Parse(betString.Split(':')[1]);
            
            BaccaratPanMgr.Inst.OnPlayerBet(gameObject.transform.position, moneyId, areaId);            

            return BaccaratBankerMgr.Inst.getCoinValue(moneyId);
        }
        catch { return 0; }
        
    }

    // public void LeftRoom() // the number of left user
    // {
    //     isSeat = false;
    //     firstImage.gameObject.SetActive(true);
    //     userBack.SetActive(false);
    // }

    // private void OnPlayerNumberingChanged()
    // {
    //     foreach (Player p in PhotonNetwork.PlayerList)
    //     {
    //     }
    // }

    public void OnClick()
    {
//        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }
}
