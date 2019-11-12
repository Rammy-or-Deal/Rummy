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

    }
    public void OnCardDistributed()
    {
        var actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

        var playerList = FortunePlayMgr.Inst.m_playerList;
        centerCard.SetActive(true);
        LogMgr.Inst.Log("PanMgr OnCardDistributed called");
        foreach (var player in playerList)
        {
            int status = 0;

            try
            {
                status = (int)PhotonNetwork.PlayerList.Where(x => x.ActorNumber == player.actorNumber).First().CustomProperties[Common.PLAYER_STATUS];
            }
            catch { }

            if (status == (int)FortunePlayerStatus.canStart || player.actorNumber < 0)
            {
                player.InitCards();
                player.moveDealCard(centerCard.transform.position);
            }
        }
        centerCard.SetActive(false);
    }

    internal async void OnOpenCard()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties[Common.PLAYER_STATUS] != (int)FortunePlayerStatus.dealtCard) return;

        int lineNo = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_OPEN_CARD_LINE];
        var playerList = FortunePlayMgr.Inst.m_playerList;
        LogMgr.Inst.Log("OnOpenCard is called. playerCount=" + playerList.Count);

        if (lineNo == 2)
        {
            FortuneUIController.Inst.calcDlg.gameObject.SetActive(true);
            FortuneUIController.Inst.calcDlg.Init(playerList);
        }

        FortuneUIController.Inst.calcDlg.showLineLabel(lineNo);
        await Task.Delay(1000);

        // Showing card
        foreach (var user in FortunePlayMgr.Inst.userCardList)
        {
            try
            {
                var seat = playerList.Where(x => x.actorNumber == user.actorNumber).First();
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

    internal void SetMissionText(FortuneMissionCard mission)
    {
        var missionText = centerCard.gameObject.transform.parent.parent.GetComponentsInChildren<UnityEngine.UI.Text>(true).Where(x => x.gameObject.name == "MissionText").First();
        missionText.text = FortuneRuleMgr.GetCardTypeString((HandSuit)mission.missionNo);
    }
}
