using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BaccaratMessageMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratMessageMgr Inst;
    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMessageArrived(int message, Player player = null)
    {

        switch (message)
        {
            case (int)BaccaratMessages.OnJoinSuccess:
                BaccaratPlayerMgr.Inst.OnJoinSuccess();
                break;
            case (int)BaccaratMessages.OnUserEnteredRoom:
                BaccaratPlayerMgr.Inst.OnUserEnteredRoom();
                break;
            case (int)BaccaratMessages.OnUserLeave:
                BaccaratPlayerMgr.Inst.OnUserLeave(player);
                break;
            case (int)BaccaratMessages.OnStartNewPan:
                BaccaratPanMgr.Inst.OnStartNewPan();
                BaccaratMe.Inst.OnStartNewPan();
                break;
            case (int)BaccaratMessages.OnPanTimeUpdate:
                BaccaratPanMgr.Inst.OnPanTimeUpdate();
                break;
            case (int)BaccaratMessages.OnEndPan:
                BaccaratMe.Inst.OnEndPan();
                if (PhotonNetwork.IsMasterClient)
                    BaccaratBankerMgr.Inst.OnEndPan();
                break;
            case (int)BaccaratMessages.OnPlayerBet:
                BaccaratPlayerMgr.Inst.OnPlayerBet(player.ActorNumber);
                break;
            case (int)BaccaratMessages.OnCatchedCardDistributed:
                BaccaratPanMgr.Inst.OnCatchedCardDistributed();
                break;
            case (int)BaccaratMessages.OnShowingCatchedCard:
                BaccaratPanMgr.Inst.OnShowingCatchedCard();
                break;
            default:
                break;
        }
    }
}
