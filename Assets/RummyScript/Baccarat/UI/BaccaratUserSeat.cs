using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using RummyScript.Model;
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
    public int id = -1;
    public bool isSeat = false;
    public int type;

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




    // public void OnClick()
    // {
    //     UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    // }
}
