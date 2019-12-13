using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class FortuneMe : MeMgr
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

            GameMgr.Inst.meMgr = this;
            PublishMe();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnClickReadyButton()
    {
        GameMgr.Inst.m_playerStatus = enumPlayerStatus.Init_Ready;

    }

    internal async void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        // if(PhotonNetwork.IsMasterClient)
        // {
        //     UpdateCardReceivedPlayer(actorNumber);                    
        // }
        
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
            LogMgr.Inst.Log("My status(" + PhotonNetwork.LocalPlayer.ActorNumber + ") is changed: " + (int)enumPlayerStatus.Fortune_Ready);
            GameMgr.Inst.seatMgr.m_playerList[0].m_playerInfo.m_status = enumPlayerStatus.Fortune_Ready;
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnUserReady},
                {Common.PLAYER_STATUS, (int)enumPlayerStatus.Fortune_Ready}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        catch { }
        
    }

    private void UpdateCardReceivedPlayer(int actorNumber)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        pList.m_playerList.Where(x=>x.m_actorNumber == actorNumber).First().m_status = enumPlayerStatus.Fortune_canStart;
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, -1},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    internal void OnGameStarted()
    {
        if((int)PhotonNetwork.LocalPlayer.CustomProperties[Common.PLAYER_STATUS] != (int)enumPlayerStatus.Fortune_Ready) 
            return;
            
        LogMgr.Inst.Log("Game Started Message received.");
        FortunePlayerMgr.Inst.userCardList.Clear();

        var changeDlg = FortuneUIController.Inst.changeDlg;
        
        changeDlg.gameObject.SetActive(true);

        for (int i = 0; i < cardList.Count; i++)
        {
            changeDlg.myCards[i].SetValue(cardList[i]);
        }
        changeDlg.UpdateHandSuitString();
        SetMyProperty((int)enumPlayerStatus.Fortune_OnChanging);

        //changeDlg.StartTimer();
    }


    public void SetMyProperty(int status)
    {        
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.PLAYER_STATUS, status},
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}