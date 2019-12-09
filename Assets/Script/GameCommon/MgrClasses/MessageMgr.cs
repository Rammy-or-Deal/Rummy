using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MessageMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual bool OnMessageArrived(int message, Player p = null)
    {
        GameMgr.Inst.Log("Message Arrived(parent): " + (enumGameMessage)message);

        enumGameMessage msg = (enumGameMessage)message;

        switch (msg)
        {
            // case enumGameMessage.OnJoinSuccess:
            //     GameMgr.Inst.meMgr.PublishMe();
            //     break;
            case enumGameMessage.OnUserEnteredRoom_onlyMaster:
                if (PhotonNetwork.IsMasterClient)
                    GameMgr.Inst.roomMgr.OnUserEnteredRoom_onlyMaster();
                break;
            case enumGameMessage.OnSeatStringUpdate:
                GameMgr.Inst.seatMgr.OnSeatStringUpdate();
                break;
            case enumGameMessage.OnPlayerLeftRoom_onlyMaster:
                GameMgr.Inst.roomMgr.OnPlayerLeftRoom_onlyMaster(p.ActorNumber);
                break;
            default:
                GameMgr.Inst.Log(msg + " isn't my section. go to children");                
                return false;
        }
        return true;
    }
}
