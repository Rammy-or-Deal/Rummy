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
    public LamiGameBot getBotStringFromPhoton(int botID)
    {
        string res = "";

        string botListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.BOT_LIST_STRING];
        //LogMgr.Inst.Log(botListString, (int)LogLevels.BotLog);

        var tmp = botListString.Split(',');
        LamiGameBot resBot = null;
        for (int i = 0; i < tmp.Length; i++)
        {
            LamiGameBot bot = new LamiGameBot();
            bot.SetBotInfo(tmp[i]);
            if (bot.id == botID)
            {
                resBot = new LamiGameBot();
                resBot.SetBotInfo(tmp[i]);
            }
        }

        return resBot;
    }
    internal void SetProperty(int tmpActor)
    {
        
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
            // for (int i = 0; i < LamiPlayerMgr.Inst.m_botList.Count; i++)
            // {
            //     if (LamiPlayerMgr.Inst.m_botList[i].id == tmpActor)
            //     {
                    //status = LamiPlayerMgr.Inst.m_botList[i].status;
                    var infoBot = getBotStringFromPhoton(tmpActor);
                    if(infoBot == null) return;
                    infoString = infoBot.getBotString();
                    status = infoBot.status;
            //         break;
            //     }
            // }
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

            canShow = true;
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

            if (isAuto)
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
        //LogMgr.Inst.Log(string.Format("GameFinished. totalCardString={0}, totalPayString={1}", totalCardString, totalPayString), (int)LogLevels.LamiFinishLog);
        //Debug.Log(string.Format("GameFinished. totalCardString={0}, totalPayString={1}", totalCardString, totalPayString));
        cardList.Clear();
        var players = totalCardString.Trim('/').Split('/');
        foreach (var player in players)
        {
            if (player == "") continue;
            int playerActor = int.Parse(player.Split(':')[0]);
            if (playerActor == id)
            {
                var numList = player.Split(':')[1].Split(',').Select(Int32.Parse).ToArray();
                var colList = player.Split(':')[2].Split(',').Select(Int32.Parse).ToArray();
                for (int i = 0; i < numList.Length; i++)
                {
                    Card card = new Card(numList[i], colList[i]);
                    card.MyCardId = -1;
                    cardList.Add(card);
                }

            }
        }
        // string ss = "CreatedCardList := ";
        // foreach (var card in cardList)
        // {
        //     ss += string.Format("{0}:{1}/{2}, ", card.num, card.color, card.MyCardId);
        // }
        // Debug.Log(ss);
        var payItems = totalPayString.Trim('/').Split('/');
        foreach (var item in payItems)
        {
            var tmp = item.Split(':').Select(Int32.Parse).ToArray();
            //Debug.Log(string.Format("MyId={0}, payId={1}, payCard={2}:{3}", id, tmp[0], tmp[1], tmp[2]));
            if (tmp.Length == 0) continue;
            if (tmp[0] == id)
            {
                for (int i = 0; i < cardList.Count; i++)
                {
                    if ((cardList[i].num == tmp[1] && cardList[i].color == tmp[2] && cardList[i].MyCardId == -1) ||
                        (cardList[i].num == 15 && tmp[2] == 15 && cardList[i].MyCardId == -1))
                    {
                        cardList[i].MyCardId = 1;
                        break;
                    }
                }
            }
        }
        // ss = "UpdatedCardList := ";
        // foreach (var card in cardList)
        // {
        //     ss += string.Format("{0}:{1}/{2}, ", card.num, card.color, card.MyCardId);
        // }

        cardList = cardList.OrderByDescending(x=>x.MyCardId).ToList();
        //Debug.Log(ss);
    }

    public int score = 0;
    public int cardPoint = 0;
    public int aCount = 0;
    public int jokerCount = 0;

    public int AddScore = 0;

    internal void calcScore()
    {
        // Calc Card Score
        cardPoint = cardList.Count(x => x.MyCardId != 1);
        score = 0;
        foreach (var card in cardList.Where(x => x.MyCardId != 1).ToList())
        {
            int addVal = 0;
            switch (card.num)
            {
                case 1:
                    addVal = 15; break;
                case 15:
                    addVal = 0; break;
                case 11:
                case 12:
                case 13:
                    addVal = 10;
                    break;
                default:
                    addVal = card.num;
                    break;
            }
            score -= addVal;
            //cardPoint++;
        }
        jokerCount = cardList.Count(x => x.num == 15);
        aCount = cardList.Count(x => x.num == 1);
    }

    internal void Init_Clear()
    {
        isAuto = false;
        status = (int)LamiPlayerStatus.Init;
        if (id < 0)
            status = (int)LamiPlayerStatus.Ready;

        mAceValue.text = "0";
        mJokerValue.text = "0";
        mCardNum.text = "20";
        mClock.gameObject.SetActive(false);
        Show();
    }
}
