using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RummyMessageMgr : MessageMgr
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

    public override bool OnMessageArrived(int message, Player p = null)
    {
        if (base.OnMessageArrived(message, p)) return true;
        GameMgr.Inst.Log("Message Arrived(children): " + (enumGameMessage)message);

        enumGameMessage msg = (enumGameMessage)message;

        switch (msg)
        {
            case enumGameMessage.OnGameStarted_Rummy:
                GameMgr.Inst.m_gameStatus = enumGameStatus.OnGameStarted;
                //RummyGameMgr.Inst.seatMgr.OnGameStarted_Rummy();
                break;
            case enumGameMessage.Rummy_OnCardDistributed:
                //RummyGameMgr.Inst.seatMgr.OnCardDistributed_Rummy();
                break;
            default:
                GameMgr.Inst.Log("Message is Unknown." + msg);
                return false;
        }

        return true;
    }
}
