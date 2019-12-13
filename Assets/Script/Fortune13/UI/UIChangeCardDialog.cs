using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class UIChangeCardDialog : MonoBehaviour
{
    public Image[] cardGroups;
    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    public FortuneHandMission[] handMissions;

    public Text frontText;
    public Text middleText;
    public Text backText;
    public GameObject mClock;
    public Text mClockText;


    //
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        for (int i = 0; i < myCards.Length; i++)
        {
            myCards[i].Init(true);
        }
        for (int i = 0; i < handMissions.Length; i++)
            handMissions[i].gameObject.SetActive(false);
    }

    public void OnTipClick()
    {

    }
    public void OnDoubleDownClick()
    {

    }

    public void OnConfirmClick()
    {
        SendMyCards();
    }

    internal void SetMission(FortuneMissionCard mission)
    {
        handMissions[mission.missionLine].SetMission(mission);
    }

    internal void UpdateHandSuitString()
    {
        var frontList = getCardList(frontCards);
        var middleList = getCardList(middleCards);
        var backList = getCardList(backCards);

        List<Card> resList = new List<Card>();
        frontText.color = Color.green;
        middleText.color = Color.green;
        backText.color = Color.green;
        SetCardGroupColor(0, true);
        SetCardGroupColor(1, true);
        SetCardGroupColor(2, true);

        var frontType = FortuneRuleMgr.GetCardType(frontList, ref frontList);
        var middleType = FortuneRuleMgr.GetCardType(middleList, ref middleList);
        var backType = FortuneRuleMgr.GetCardType(backList, ref backList);

        frontText.text = FortuneRuleMgr.GetCardTypeString(frontType);
        middleText.text = FortuneRuleMgr.GetCardTypeString(middleType);
        backText.text = FortuneRuleMgr.GetCardTypeString(backType);

        LogMgr.Inst.Log("Result calculated. type=(front,middle,back) : " + frontType + ", " + middleType + ", " + backType, (int)LogLevels.PlayerLog1);

        var frontScore = FortuneRuleMgr.GetScore(frontList, frontType);
        var middleScore = FortuneRuleMgr.GetScore(middleList, middleType);
        var backScore = FortuneRuleMgr.GetScore(backList, backType);

        LogMgr.Inst.Log("score=(front,middle,back) : " + frontScore + ", " + middleScore + ", " + backScore, (int)LogLevels.PlayerLog1);
        // Compare front and middle
        if (frontScore > middleScore)
        {
            frontText.color = Color.red;
            middleText.color = Color.red;
            backText.color = Color.red;

            SetCardGroupColor(0, false);
            SetCardGroupColor(1, false);
        }
        if (middleScore > backScore)
        {
            middleText.color = Color.red;
            backText.color = Color.red;
            SetCardGroupColor(0, false);
            SetCardGroupColor(1, false);
            SetCardGroupColor(2 , false);
        }
    }
    List<Card> getCardList(FortuneCard[] cards)
    {
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            cardList.Add(cards[i].GetValue());
        }
        return cardList;
    }
    public void SendMyCards()
    {    
        FortuneMe.Inst.SetMyProperty((int)enumPlayerStatus.Fortune_dealtCard);

        var frontList = getCardList(frontCards);
        var middleList = getCardList(middleCards);
        var backList = getCardList(backCards);


        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnPlayerDealCard},
            {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
            {Common.FORTUNE_PLAYER_FRONT_CARD, string.Join(",", frontList.Select(x=>x.cardString))},
            {Common.FORTUNE_PLAYER_MIDDLE_CARD, string.Join(",", middleList.Select(x=>x.cardString))},
            {Common.FORTUNE_PLAYER_BACK_CARD, string.Join(",", backList.Select(x=>x.cardString))},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        this.gameObject.SetActive(false);
    }

    public void SetCardGroupColor(int id, bool flag)
    {
        cardGroups[id].color = flag ? new Color32(43, 43, 43, 255) : new Color32(255, 42, 42, 255);
    }
}
