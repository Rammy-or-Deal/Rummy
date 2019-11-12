using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class FortuneMe : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortuneMe Inst;
    public FortuneMissionCard mission;
    List<Card> cardList;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            cardList = new List<Card>();
            mission = new FortuneMissionCard();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal async void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

        cardList.Clear();
        var cardString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.CARD_LIST_STRING];
        LogMgr.Inst.Log("Card Received. cardString=" + cardString, (int)LogLevels.PlayerLog1);
        foreach (var str in cardString.Split(','))
        {
            Card card = new Card();
            card.cardString = str;
            cardList.Add(card);
        }

    //  Set Mission String
        
        string missionString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_MISSION_CARD];
        mission.missionString = missionString;
        LogMgr.Inst.Log("OnCardDistributed Received. MissionCard=" + missionString, (int)LogLevels.RoomLog1);

        var changeDlg = FortuneUIController.Inst.changeDlg;
        changeDlg.Init();
        changeDlg.SetMission(mission);
        FortunePanMgr.Inst.SetMissionText(mission);

        // Send I am ready.
        await Task.Delay(5000);

        try
        {
            LogMgr.Inst.Log("My status(" + PhotonNetwork.LocalPlayer.ActorNumber + ") is changed: " + (int)FortunePlayerStatus.Ready);
            FortunePlayMgr.Inst.m_playerList[0].Status = (int)FortunePlayerStatus.Ready;
            Hashtable table = new Hashtable{
                {Common.FORTUNE_MESSAGE, (int)FortuneMessages.OnUserReady},
                {Common.PLAYER_STATUS, (int)FortunePlayerStatus.Ready}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
        }
        catch { }
    }

    internal void OnGameStarted()
    {
        if((int)PhotonNetwork.LocalPlayer.CustomProperties[Common.PLAYER_STATUS] != (int)FortunePlayerStatus.Ready) 
            return;
            
        LogMgr.Inst.Log("Game Started Message received.");
        FortunePlayMgr.Inst.userCardList.Clear();

        var changeDlg = FortuneUIController.Inst.changeDlg;
        
        changeDlg.gameObject.SetActive(true);

        for (int i = 0; i < cardList.Count; i++)
        {
            changeDlg.myCards[i].SetValue(cardList[i]);
        }
        changeDlg.UpdateHandSuitString();
        SetMyProperty((int)FortunePlayerStatus.OnChanging);

        changeDlg.StartTimer();
    }


    public void SetMyProperty(int status)
    {        
        Hashtable props = new Hashtable{
            {Common.FORTUNE_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.PLAYER_STATUS, status},
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}