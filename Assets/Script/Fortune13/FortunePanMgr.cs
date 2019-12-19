using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class FortunePanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortunePanMgr Inst;
    public GameObject centerCard;
    public GameObject centerCoin;

    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    public void OnInitCard()
    {
        foreach(FortuneUserSeat seat in GameMgr.Inst.seatMgr.m_playerList)
        {
            seat.InitCards();
        }
    }
    public void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

        var playerList = FortunePlayerMgr.Inst.m_playerList;
        centerCard.SetActive(true);
        GameMgr.Inst.Log("PanMgr OnCardDistributed called");
        foreach (FortuneUserSeat player in playerList)
        {            
            if(player.isSeat == false) continue;

            player.InitCards();
            player.moveDealCard(centerCard.transform.position);
        }
        centerCard.SetActive(false);
    }

    internal async void OnOpenCard()
    {
        if(GameMgr.Inst.m_gameStatus != enumGameStatus.OnGameStarted) return;

        int lineNo = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_OPEN_CARD_LINE];
        var playerList = FortunePlayerMgr.Inst.m_playerList;
        GameMgr.Inst.Log("OnOpenCard is called. playerCount=" + playerList.Count);

        if (lineNo == 2)
        {
            FortuneUIController.Inst.calcDlg.gameObject.SetActive(true);
            FortuneUIController.Inst.calcDlg.Init(playerList);
        }

        FortuneUIController.Inst.calcDlg.showLineLabel(lineNo);
//        await Task.Delay(1000);

        // Showing card
        foreach (var user in FortunePlayerMgr.Inst.userCardList)
        {
            try
            {
                var seat = (FortuneUserSeat)playerList.Where(x => x.isSeat == true && x.m_playerInfo.m_actorNumber == user.actorNumber).First();
                List<Card> showList = new List<Card>();
                switch (lineNo)
                {
                    case 0:
                        showList = user.frontCard;
                        break;
                    case 1:
                        showList = user.middleCard;
                        break;
                    case 2:
                        showList = user.backCard;
                        break;
                }
                seat.ShowCards(lineNo, showList);
                FortuneUIController.Inst.calcDlg.ShowCards(user, showList);
            }
            catch
            {
                break;
            }
        }
        await Task.Delay(500);
        FortuneUIController.Inst.calcDlg.SendReceiveCoin(lineNo);

        if (lineNo == 0)
        {
            FortuneUIController.Inst.resultDlg.Init(playerList);
            await Task.Delay(7000);

            FortuneUIController.Inst.calcDlg.gameObject.SetActive(false);
            FortuneUIController.Inst.resultDlg.SetProperty(FortuneUIController.Inst.calcDlg);
            
            FortuneUIController.Inst.resultDlg.ShowResult();
        }
        
    }

    internal void OnShowLuckResult()
    {
        FortuneUIController.Inst.luckyAlert.gameObject.SetActive(false);

        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();

        int luckyActor = pList.m_playerList.Where(x=>x.m_status == enumPlayerStatus.Fortune_Lucky).First().m_actorNumber;
        FortuneUIController.Inst.luckyDlg.players[0].Init(GameMgr.Inst.seatMgr.m_playerList.Where(x=>x.isSeat == true && x.m_playerInfo.m_actorNumber == luckyActor).First());

        
        FortuneUIController.Inst.luckyDlg.gameObject.SetActive(true);
    }

    internal void OnLucky()
    {
        FortuneUIController.Inst.changeDlg.gameObject.SetActive(false);
        FortuneUIController.Inst.luckyAlert.gameObject.SetActive(true);
    }

    internal void OnTickTimer()
    {
        //mClockText.text = waitTime.ToString();
        int remainTime = 0;
        remainTime = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_REMAIN_TIME];
        FortuneUIController.Inst.changeDlg.mClockText.text = remainTime.ToString();

        if(remainTime == 0)
        {
            FortuneUIController.Inst.changeDlg.SendMyCards(enumPlayerStatus.Fortune_dealtCard);
        }
    }

    internal void SetMissionText(FortuneMissionCard mission)
    {
        var missionText = centerCard.gameObject.transform.parent.parent.GetComponentsInChildren<UnityEngine.UI.Text>(true).Where(x => x.gameObject.name == "MissionText").First();
        missionText.text = FortuneRuleMgr.GetCardTypeString((HandSuit)mission.missionNo);
    }
}
