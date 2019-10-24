using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
public class LamiUserSeat : MonoBehaviour
{
    static public LamiUserSeat Inst;
    public bool canShow;
    public int status;
    public Text mCardNum;
    public Text mClockTime;
    public Image mUserFrame;
    public Image mUserPic;
    public Image mUserSkillPic;
    public Text mUserName;
    public Text mUserSkillName;
    public Text mCoinValue;
    public Text mAceValue;
    public Text mJokerValue;
    public GameObject mClock;
    public GameObject mAceJokerPanel;

    public Image playerReadyImage;

    public int id;
    private bool isPlayerReady;
    public bool isSeat;

    public bool isBot = false;
    public int frameId;

    #region UNITY

    public void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    private void Awake()
    {
        if (!Inst)
            Inst = this;
    }

    public void Start()
    {
        mClock.SetActive(false);
    }

    public void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }

    #endregion



    #region Property
    internal void SetProperty(int tmpActor)
    {
        canShow = true;
        isBot = true;
        string infoString = "";
        foreach (Player p in PhotonNetwork.PlayerList)  // If this isn't a bot
        {
            if (tmpActor == p.ActorNumber)
            {
                string user_info = (string)p.CustomProperties[Common.PLAYER_INFO];
                status = (int)p.CustomProperties[Common.PLAYER_STATUS];
                infoString = user_info;
                isBot = false;
                break;
            }
        }

        if (isBot)  //If this player is bot, pull data from parent's bot information
        {
            for (int i = 0; i < LamiPlayerMgr.Inst.m_botList.Count; i++)
            {
                if (LamiPlayerMgr.Inst.m_botList[i].id == tmpActor)
                {
                    status = LamiPlayerMgr.Inst.m_botList[i].status;
                    infoString = LamiPlayerMgr.Inst.m_botList[i].getBotString();
                    break;
                }
            }
        }
        if (infoString != "")
        {
            var tmp = infoString.Split(':');
            id = int.Parse(tmp[0]);
            name = tmp[1];
            mUserName.text =tmp[1];
            mUserPic.sprite = Resources.Load<Sprite>((string)tmp[2]);
            mCoinValue.text = tmp[3];
            mUserSkillName.text = tmp[4];
            frameId = int.Parse(tmp[5]);
        }
    }

    public void SetAdditionalCardInfo(string data)
    {
        var tmp = data.Split(':');
        int jokerCount = int.Parse(tmp[0]);
        int ACount = int.Parse(tmp[1]);
        mJokerValue.text = tmp[0];
        mAceValue.text = tmp[1];
    }
    #endregion

    public void Show()
    {
        if (canShow)
        {
            gameObject.SetActive(true);
            if (status == (int)LamiPlayerStatus.Ready)
            {
                playerReadyImage.gameObject.SetActive(true);
            }
            else
            {
                playerReadyImage.gameObject.SetActive(false);
            }
            
        }
        else
            gameObject.SetActive(false);
    }
    public void Show(Player p)
    {

    }
    private void OnPlayerNumberingChanged()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
        }
    }

    public void SetPlayerIsReady(bool playerReady)
    {
        playerReadyImage.gameObject.SetActive(playerReady);
    }

    public void OnClick()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }

    internal void LeftRoom()
    {
        throw new NotImplementedException();
    }
}
