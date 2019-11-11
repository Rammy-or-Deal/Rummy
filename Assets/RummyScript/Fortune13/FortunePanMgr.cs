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
        var playerList = FortunePlayMgr.Inst.m_playerList;
        centerCard.SetActive(true);
        foreach (var player in playerList)
        {
            player.InitCards();
            player.moveDealCard(centerCard.transform.position);
        }
        centerCard.SetActive(false);
    }

    internal async void OnOpenCard()
    {
        var seatList = PlayerManagement.Inst.getSeatList();
        int status = seatList.Where(x => x.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber).Select(x => x.status).First();
        if (status != (int)FortunePlayerStatus.dealtCard) return;

        int lineNo = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_OPEN_CARD_LINE];
        var playerList = FortunePlayMgr.Inst.m_playerList;
        LogMgr.Inst.Log("OnOpenCard is called. playerCount=" + playerList.Count);

        if (lineNo == 2)
        {
            FortuneUIController.Inst.calcDlg.gameObject.SetActive(true);
            FortuneUIController.Inst.calcDlg.Init(playerList);
            FortuneUIController.Inst.resultDlg.Init(playerList);
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
                FortuneUIController.Inst.calcDlg.SendReceiveCoin(lineNo);

                if (lineNo == 0)
                {
                    await Task.Delay(10000);

                    FortuneUIController.Inst.calcDlg.gameObject.SetActive(false);
                    FortuneUIController.Inst.resultDlg.gameObject.SetActive(true);
                }

            }
            catch
            {
                break;
            }
        }
    }

    internal void SetMissionText(FortuneMissionCard mission)
    {
        var missionText = centerCard.gameObject.transform.parent.parent.GetComponentsInChildren<UnityEngine.UI.Text>(true).Where(x => x.gameObject.name == "MissionText").First();
        missionText.text = FortuneRuleMgr.GetCardTypeString((HandSuit)mission.missionNo);
    }
}
