using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Models;
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
    public GameObject autoOffBtn;
    public Button tipButton;
    public Button playButton;
    public Button arrangeButton;
    public GameObject settingDlg;
    public UILamiFinish finishDlg;
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

    public UILamiSelectCardList uiSelectCardList;

    void Awake()
    {
        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
        mGameCardPanelList = new List<LamiGameCardList>();
    }

    public void InitLineNumbers()
    {
        foreach (var obj in mGameCardPanelList)
        {
            obj.lineNum = -1;
        }
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

        //myCardPanel.DealCards();
        //SetPlayButtonState

        List<List<Card>> temp = new List<List<Card>>();
        //var machingList = UIMyCardPanel.GetMatchedList(LamiGameUIManager.Inst.myCardPanel.myCards.Where(x => x.isSelected == true).ToList(), LamiMe.Inst.availList);
        var machingList = myCardPanel.m_machedList;
        if (machingList.Count == 1)
        {
            //myCardPanel.OnClickLine();
            myCardPanel.OnClickCardList(0);
        }
        else if (machingList.Count > 1)
        {
            if (machingList[0].list[0].virtual_num == machingList[0].list[1].virtual_num)
            {
                //myCardPanel.OnClickLine();
                myCardPanel.OnClickCardList(0);
            }
            else
            {
                foreach(var list in machingList)
                    temp.Add(list.list);
                uiSelectCardList.Show(temp);
            }
        }




        //InitPanList
        //
        //LamiCountdownTimer.Inst.StopTurnTimer();
        //LamiGameController.Inst.GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(false);
    }

    public void OnSelectedCardList(int id)
    {
        myCardPanel.OnClickCardList(id);
        //        Todo
    }

    public void OnDealCard(string cardStr)
    {
        Debug.Log(cardStr);
        int lineNum = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.GAME_CARD_PAN];

        var cardList = LamiCardMgr.ConvertCardStrToCardList(cardStr);
        LogMgr.Inst.Log("User dealt card, line number:= " + lineNum, (int)LogLevels.RoomLog3);
        MessageStatus lineType = MessageStatus.Flush;
        if (lineNum == -1)
        {
            curGameCardList = Instantiate(gameCardListPrefab, gameCardPanelPan.transform);
            curGameCardList.gameObject.transform.localScale = Vector3.one;
            mGameCardPanelList.Add(curGameCardList);
            curGameCardList.Init();
            foreach (Card card in cardList)
            {
                curGameCardList.AddGameCard(card);
            }
            curGameCardList.ShowCards();
            if (curGameCardList.mGameCardList[0].virtual_num == curGameCardList.mGameCardList[1].virtual_num)
                lineType = MessageStatus.Set;
        }
        else
        {
            List<Card> list = new List<Card>();
            list.AddRange(cardList.ToList());

            if (list[list.Count - 1].virtual_num + 1 == mGameCardPanelList[lineNum].mGameCardList[0].virtual_num)
            {
                mGameCardPanelList[lineNum].AddStartCards(list);
            }
            else
            {
                mGameCardPanelList[lineNum].AddEndCards(list);
            }
            if (mGameCardPanelList[lineNum].mGameCardList[0].virtual_num == mGameCardPanelList[lineNum].mGameCardList[1].virtual_num)
            {
                lineType = MessageStatus.Set;
            }
        }

        LamiEffectDialog.Inst.ShowMessage(lineType);
    }
    public void PlayerCardUpdate(Player otherPlayer, Hashtable dealCard)
    {
        
    }

    public void OnClickTips()
    {
        LamiMe.Inst.SelectTipCard();
    }

    public void OnClickArrange()
    {
        LogMgr.Inst.Log("Arrange Button is clicked", (int)LogLevels.RoomLog3);
        myCardPanel.ArrangeMyCard();
        for (int i = 0; i < LamiGameUIManager.Inst.myCardPanel.myCards.Count; i++)
        {
            LamiGameUIManager.Inst.myCardPanel.myCards[i].isSelected = false;
            LamiGameUIManager.Inst.myCardPanel.myCards[i].SetUpdate();
        }
        LamiMe.Inst.Init_FlashList();
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
        //        PunController.Inst.LeaveGame();
        finishDlg.gameObject.SetActive(true);
    }

    public void OnHelpClick()
    {
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }

    public void OnSettingClick()
    {
        settingDlg.SetActive(true);
    }

    public void OnClickAutoOffBtn()
    {
        LamiMe.Inst.isAuto = false;
        LamiCountdownTimer.Inst.StartTurnTimer(true);
        autoOffBtn.SetActive(false);
    }
}