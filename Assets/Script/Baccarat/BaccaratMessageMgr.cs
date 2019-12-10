using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BaccaratMessageMgr : MessageMgr
{
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.Inst.messageMgr = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool OnMessageArrived(int message, Player player = null)
    {
        if (base.OnMessageArrived(message, player)) return true;

        enumGameMessage msg = (enumGameMessage)message;

        switch (msg)
        {
            // case enumGameMessage.Baccarat_OnJoinSuccess:
            //     BaccaratPlayerMgr.Inst.OnJoinSuccess();
            //     break;
            // case enumGameMessage.Baccarat_OnUserEnteredRoom:
            //     BaccaratPlayerMgr.Inst.OnUserEnteredRoom();
            //     break;
            // case enumGameMessage.Baccarat_OnUserLeave:
            //     BaccaratPlayerMgr.Inst.OnUserLeave(player);
            //     break;
            case enumGameMessage.Baccarat_OnStartNewPan:
                BaccaratPanMgr.Inst.OnStartNewPan();
                BaccaratMe.Inst.OnStartNewPan();
                break;
            case enumGameMessage.Baccarat_OnPanTimeUpdate:
                BaccaratPanMgr.Inst.OnPanTimeUpdate();
                break;
            case enumGameMessage.Baccarat_OnEndPan:
                BaccaratMe.Inst.OnEndPan();
                BaccaratPanMgr.Inst.OnEndPan();
                if (PhotonNetwork.IsMasterClient)
                    BaccaratBankerMgr.Inst.OnEndPan();
                break;
            case enumGameMessage.Baccarat_OnPlayerBet:
                BaccaratPlayerMgr.Inst.OnPlayerBet();
                break;
            case enumGameMessage.Baccarat_OnCatchedCardDistributed:
                BaccaratPanMgr.Inst.OnCatchedCardDistributed();
                break;
            case enumGameMessage.Baccarat_OnShowingCatchedCard:
                BaccaratPanMgr.Inst.OnShowingCatchedCard();
                break;
            case enumGameMessage.Baccarat_OnShowingVictoryArea:
                BaccaratPanMgr.Inst.OnShowingVictoryArea();
                break;
            case enumGameMessage.Baccarat_OnPrizeAwarded:
                int actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
                if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    BaccaratPanMgr.Inst.OnPrizeAwarded();
                    BaccaratMe.Inst.OnPrizeAwarded();
                }
                break;
            case enumGameMessage.Baccarat_OnUpdateMe:
                BaccaratPlayerMgr.Inst.OnUpdateMe(player);
                break;
            case enumGameMessage.Baccarat_OnInitUI:
                if (PhotonNetwork.IsMasterClient)
                    BaccaratPanMgr.Inst.StartNewPan();
                break;
            default:
                return false;
        }
        return true;
    }
}
