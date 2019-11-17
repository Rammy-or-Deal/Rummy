using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
public class LamiUserSeat : MonoBehaviour
{

    public int cardPoint;
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
    public GameObject playerBurntImage;
    public GameObject playerGiveupImage;
    public GameObject autoOnImage;

    public int id;
    private bool isPlayerReady;

    public bool isSeat;

    public bool isBot = false;
    public int frameId;
    public List<Card> cardList = new List<Card>();

    public int score = 0;
    internal bool isAuto;

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

    public void OnUserDealt(string dealString)
    {
        
        var cards = dealString.Split(',').Select(Int32.Parse).ToArray();
        int aCount = cards.Count(x => x == 1);
        int jokerCount = cards.Count(x => x == 15);

        mAceValue.text = (int.Parse(mAceValue.text) + aCount) + "";
        mJokerValue.text = (int.Parse(mJokerValue.text) + jokerCount) + "";
        
    }

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
            mUserName.text = tmp[1];
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

    void InitStatus()
    {
        playerReadyImage.gameObject.SetActive(false);
        playerGiveupImage.gameObject.SetActive(false);
        playerBurntImage.gameObject.SetActive(false);
        autoOnImage.gameObject.SetActive(false);
    }
    public void Show()
    {
        InitStatus();
        if (canShow)
        {
            gameObject.SetActive(true);
            switch (status)
            {
                case (int)LamiPlayerStatus.Ready:
                    playerReadyImage.gameObject.SetActive(true);
                    mAceJokerPanel.gameObject.SetActive(true);
                    mJokerValue.text = "0";
                    mAceValue.text = "0";
                    break;
                case (int)LamiPlayerStatus.GiveUp:
                    playerGiveupImage.gameObject.SetActive(true);
                    break;
                case (int)LamiPlayerStatus.Burnt:
                    playerBurntImage.gameObject.SetActive(true);
                    break;
            }

            if(isAuto)
                autoOnImage.gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
    public void Show(Player p)
    {

    }
    private void OnPlayerNumberingChanged()
    {

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

    internal void cardListUpdate(string totalCardString, string totalPayString)
    {
        LogMgr.Inst.Log(string.Format("GameFinished. totalCardString={0}, totalPayString={1}", totalCardString, totalPayString), (int)LogLevels.LamiFinishLog);

        cardList.Clear();
        var players = totalCardString.Trim('/').Split('/');
        foreach (var player in players)
        {
            if(player == "") continue;
            int playerActor = int.Parse(player.Split(':')[0]);
            if (playerActor == id)
            {
                var numList = player.Split(':')[1].Split(',').Select(Int32.Parse).ToArray();
                var colList = player.Split(':')[2].Split(',').Select(Int32.Parse).ToArray();
                for(int i = 0; i < numList.Length; i++)
                {
                    Card card = new Card(numList[i], colList[i]);
                    cardList.Add(card);
                }
                
            }
        }

        var payItems = totalPayString.Trim('/').Split('/');
        foreach(var item in payItems)
        {
            var tmp = item.Split(':').Select(Int32.Parse).ToArray();
            if(tmp.Length == 0) continue;
            if(tmp[0] == id)
            {
                for(int i = 0; i < cardList.Count; i++)
                {
                    if((cardList[i].num == tmp[1] && cardList[i].color == tmp[2] && cardList[i].MyCardId != -1) ||
                        (cardList[i].num == 15 && tmp[2] == 15 && cardList[i].MyCardId != -1))
                        {
                            cardList[i].MyCardId = 1;
                        }
                }
            }
        }
    }
}
