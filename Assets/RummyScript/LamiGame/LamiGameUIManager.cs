﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LamiGameUIManager : MonoBehaviour
{
    public static LamiGameUIManager Inst;

    //Button
    public GameObject chatButton;
    public GameObject readyButton;
    public Button tipButton;
    public Button playButton;
    public Button arrangeButton;
    public GameObject settingDlg;

    //Menu
    public GameObject mMenuPanel;

    //Game
    public GameObject firstImg;

    //CardPanel
    public UIMyCardPanel myCardPanel;

    //GameCard
    public List<LamiGameCardList> mGameCardPanelList; //GameCardLsit List in GamePanel

    public GameObject gameCardPanelPan;

    //prefabs
    public LamiGameCard gameCardPrefab;
    public LamiGameCardList gameCardListPrefab;

    public LamiGameCardList curGameCardList;

    void Awake()
    {
        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
        mGameCardPanelList = new List<LamiGameCardList>();
    }

    public void InitButtonsFirst()
    {
        playButton.gameObject.SetActive(false);
        tipButton.gameObject.SetActive(false);
        arrangeButton.gameObject.SetActive(true);
        firstImg.SetActive(false);
    }

    public void OnReadyClick()
    {
        LamiCountdownTimer.Inst.StopTimer();
        readyButton.SetActive(false);

        Hashtable props = new Hashtable
        {
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnUserReady},
            {Common.PLAYER_STATUS, (int)LamiPlayerStatus.Ready},
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log("ready click");
    }

    public void OnClickPlay()
    {
        //added GameCardList
//        curGameCardList = Instantiate(gameCardListPrefab,gameCardPanelPan.transform);
//        curGameCardList.gameObject.transform.localScale = Vector3.one;
//        mGameCardPanelList.Add(curGameCardList);

        myCardPanel.DealCards();
        playButton.gameObject.SetActive(false);
        tipButton.gameObject.SetActive(false);
        //
        //LamiCountdownTimer.Inst.StopTurnTimer();
        //LamiGameController.Inst.GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(false);
    }

    public void AddGameCard(LamiMyCard card)
    {
        //
        LamiGameCard entry = Instantiate(gameCardPrefab, curGameCardList.transform);
        entry.gameObject.transform.localScale = Vector3.one;
        entry.num = card.num;
        entry.color = card.color;
        curGameCardList.mGameCardList.Add(entry);
    }

    public void PlayerCardUpdate(Player otherPlayer, Hashtable dealCard)
    {
        object cardList;
        object cardList_pan;
        object cardList_pan_pos;
        object reaminCard;
        object isTurn;
        Card[] cards;
        int card_pan_id;
        int card_pan_pos;
        int id = 0;
        if (dealCard.TryGetValue(Common.GAME_CARD, out cardList))
        {
            id = LamiGameController.Inst.seatNumList[otherPlayer.ActorNumber];
            string cardStr = (string) cardList;
            Debug.Log(cardStr);
            curGameCardList = Instantiate(gameCardListPrefab, gameCardPanelPan.transform);
            curGameCardList.gameObject.transform.localScale = Vector3.one;
            mGameCardPanelList.Add(curGameCardList);

            foreach (Card card in LamiCardMgr.ConvertCardStrToCardList(cardStr))
            {
                LamiGameCard entry = Instantiate(gameCardPrefab, curGameCardList.transform);
                entry.gameObject.transform.localScale = Vector3.one;
                entry.num = card.num;
                entry.color = card.color;
                curGameCardList.mGameCardList.Add(entry);
            }

            if (id == PhotonNetwork.CurrentRoom.PlayerCount - 1)
            {
                id = -1;
            }


            if (LamiGameController.Inst.seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] == id + 1)
            {
                Debug.Log("LamiGameController.Inst.seatNumList[PhotonNetwork.LocalPlayer.ActorNumber]" + (id + 1));

                Hashtable turnChange = new Hashtable
                {
                    {Common.PLAYER_TURN, id + 1},
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(turnChange);
                playButton.gameObject.SetActive(true);
                playButton.interactable = false;
                tipButton.gameObject.SetActive(true);
                LamiGameController.Inst.GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(true);
                LamiCountdownTimer.Inst.StartTurnTimer();
                Debug.Log((id + 1).ToString() + ": isTurn");
            }
        }

        if (dealCard.TryGetValue(Common.PLAYER_TURN, out isTurn))
        {
            int seatPos = LamiGameController.Inst.GetUserSeat((int) isTurn);

        }

        if (dealCard.TryGetValue(Common.GAME_CARD_PAN, out cardList_pan))
        {
//            Debug.Log("cardListPan ID:" + (int)cardList_pan);
            card_pan_id = (int) cardList_pan;
        }

        if (dealCard.TryGetValue(Common.GAME_CARD_PAN, out cardList_pan_pos))
        {
//            Debug.Log("cardListPanPos:" + (int)cardList_pan_pos);
            card_pan_pos = (int) cardList_pan_pos;
        }

        if (dealCard.TryGetValue(Common.REMAIN_CARD_COUNT, out reaminCard))
        {
            Debug.Log("REMAIN_CARD_COUNT:" + (int) reaminCard);
            LamiGameController.Inst.GetUserSeat(otherPlayer).mCardNum.text = reaminCard.ToString();
        }
    }

    public void OnClickTips()
    {
    }

    public void OnClickArrange()
    {
    }

    public void OnClickChat()
    {
        Debug.Log("chat clicked ");
        UIController.Inst.chatDlg.gameObject.SetActive(true);
    }

    public void OnClickMenu()
    {
        bool active = mMenuPanel.activeSelf == true ? false : true;
        mMenuPanel.SetActive(active);
    }

    public void OnExitClick()
    {
        Debug.Log("Exit clicked");
        PunController.Inst.LeaveGame();
    }

    public void OnHelpClick()
    {
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }

    public void OnSettingClick()
    {
        settingDlg.SetActive(true);
    }
}