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

public class LamiGameUIManager : GameUIManager
{
    public static LamiGameUIManager Inst;

    //Button
    public GameObject chatButton;
    public GameObject readyButton;
    public GameObject autoOffBtn;
    public Button tipButton;
    public Button playButton;
    public Button arrangeButton;
    public Button shuffleButton;
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
        NewMethod();

        //alert dlg example
        //UIAlertDialog.Inst.Show(Game_Identifier.Lami, OnYesDlg, "WOULD YOU LIKE TO SHUFFLE 3 CARDS RANDOMLY ?", 3);
    }

    private void NewMethod()
    {
        mGameCardPanelList = new List<LamiGameCardList>();
    }

    public void OnYesDlg()
    {
        Debug.Log("Yes clicked");
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
        shuffleButton.gameObject.SetActive(true);
        firstImg.SetActive(false);
    }

    public void OnReadyClick()
    {
       
        LamiCountdownTimer.Inst.StopTimer();
        readyButton.SetActive(false);
/*
        Hashtable props = new Hashtable
        {
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OnUserReady},
            {Common.PLAYER_STATUS, (int)LamiPlayerStatus.Ready},
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log("ready click");              
*/

        var myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        var info = GameMgr.Inst.seatMgr.m_playerList.Where(x=>x.m_playerInfo.m_actorNumber == myActorNumber).First();
        BotMgr.PublishIamReady(info.m_playerInfo);

    }
    public void OnClickShuffle()
    {
        LamiMe.Inst.OnClickShuffle();
    }
    public void OnClickPlay()
    {
        List<List<Card>> temp = new List<List<Card>>();
        List<int> matchNoList = new List<int>();
        var machingList = myCardPanel.m_machedList;
        if (machingList.Count == 1)
        {
            //myCardPanel.OnClickLine();
            myCardPanel.OnClickCardList(0);
        }
        else if (machingList.Count > 1)
        {
            if (machingList[0].list[0].virtual_num == machingList[0].list[1].virtual_num)   // set
            {
                //myCardPanel.OnClickLine();
                myCardPanel.OnClickCardList(0);
            }
            else
            {
                foreach (var list in machingList)
                {
                    if(list.list.Count >= 3 && list.lineNo != -1) continue;
                    temp.Add(list.list);
                    matchNoList.Add(machingList.IndexOf(list));
                }
                uiSelectCardList.Show(temp, matchNoList);
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

        Hashtable table = new Hashtable{
                    {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Rummy_OffAutoPlayer},
                    {Common.PLAYER_ID, (int)PhotonNetwork.LocalPlayer.ActorNumber}
                };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    public void Init_Clear()
    {
        readyButton.gameObject.SetActive(true);
        autoOffBtn.gameObject.SetActive(false);
        tipButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        arrangeButton.gameObject.SetActive(false);

        foreach (var list in mGameCardPanelList)
        {
            list.Init_Clear();
        }
        mGameCardPanelList.Clear();

        myCardPanel.Init_Clear();
    }
}