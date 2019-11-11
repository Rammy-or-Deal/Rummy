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
        await Task.Delay(10000);

        try
        {
            var seatList = PlayerManagement.Inst.getSeatList();
            var mySeat = seatList.Where(x => x.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber).First();
            mySeat.status = (int)FortunePlayerStatus.Ready;

            LogMgr.Inst.Log("My status(" + PhotonNetwork.LocalPlayer.ActorNumber + ") is changed: seatstring=" + string.Join(",", seatList.Select(x => x.seatString)), (int)LogLevels.PlayerLog1);
            FortunePlayMgr.Inst.m_playerList[0].Status = (int)FortunePlayerStatus.Ready;
            Hashtable table = new Hashtable{
                {Common.FORTUNE_MESSAGE, (int)FortuneMessages.OnUserReady},
                {Common.SEAT_STRING, string.Join(",", seatList.Select(x=>x.seatString))}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(table);
        }
        catch { }
    }

    internal void OnGameStarted()
    {
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
        var seatList = PlayerManagement.Inst.getSeatList();
        foreach (var seat in seatList.Where(x => x.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber))
        {
            seat.status = status;
        }

        var seatString = string.Join(",", seatList.Select(x => x.seatString));
        Hashtable props = new Hashtable{
            {Common.FORTUNE_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}