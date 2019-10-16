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
    
    public int actorNumber;
    public Image playerReadyImage;

    public int id;
    private bool isPlayerReady;
    public bool isSeat;

    public bool isBot = false;
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

    public void Show(Player p)
    {
        actorNumber = p.ActorNumber;
        mUserName.text = p.NickName;
        isSeat = true;

        object isPlayerReady;

        if (p.CustomProperties.TryGetValue(Common.PLAYER_STATUS, out isPlayerReady))
        {
            SetPlayerIsReady((bool) isPlayerReady);
        }
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
        
         gameObject.SetActive(true);
         Debug.Log("Player//" + id);
    }

    public void ShowBot(LamiBot p)
    {
        actorNumber = p.id;
        mUserName.text = p.name;
        isSeat = true;

        SetPlayerIsReady(true);
        mUserPic.sprite = Resources.Load<Sprite>(p.pic);
        mUserSkillName.text = p.skillLevel;
        mCoinValue.text = p.coinValue.ToString();
        isBot = true;

        gameObject.SetActive(true);
        Debug.Log("BOT //" + id);


    }
    public void LeftRoom() // the number of left user
    {
        isSeat = false;
        gameObject.SetActive(false);
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
}