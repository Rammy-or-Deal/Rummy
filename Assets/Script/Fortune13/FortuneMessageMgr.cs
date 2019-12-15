using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum FortuneMessages
{
    OnUserReady = 1,
    OnGameStarted = 2,
    OnCardDistributed = 3,
    OnPlayerDealCard = 4,
    OnOpenCard = 5,
    OnFinishedGame = 6,
}
public enum FortuneGameStatus
{
    Init = 0,
    GameStarted = 1,
    CardDealt = 2,

}
public class FortuneMessageMgr : MessageMgr
{
    public static FortuneMessageMgr Inst;
    // Start is called before the first frame update
    public FortuneGameStatus nowGameStatus;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            nowGameStatus = FortuneGameStatus.Init;
        }
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
            /*
            case RoomManagementMessages.OnUserSit: // This function is used only one time - start time.
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnUserSit();        // set status to canStart
                break;
            
            case enumGameMessage.Fortune_InitReady:
                if(PhotonNetwork.IsMasterClient)
                {
                    FortunePlayerMgr.Inst.Fortune_InitReady();
                }
                break;
            */  
            case enumGameMessage.Fortune_OnCardDistributed:
                FortuneMe.Inst.OnCardDistributed();         // set status to Ready
                FortunePanMgr.Inst.OnCardDistributed();
                break;
            case enumGameMessage.Fortune_OnUserReady:
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnUserReady();
                break;
            case enumGameMessage.Fortune_OnGameStarted:
                nowGameStatus = FortuneGameStatus.GameStarted;  // started
                FortuneMe.Inst.OnGameStarted();           // set status to OnChanging
                break;
            case enumGameMessage.Fortune_OnTickTimer:
                FortunePanMgr.Inst.OnTickTimer();
                if(PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnTickTimer();
                break;
            case enumGameMessage.Fortune_DoubleDownRequest:
                if(PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnDoubleRequest();
                break;
            case enumGameMessage.Fortune_OnPlayerDealCard:
                FortunePlayerMgr.Inst.OnPlayerDealCard();
                break;
            case enumGameMessage.Fortune_OnOpenCard:
                FortunePanMgr.Inst.OnOpenCard();
                FortunePlayerMgr.Inst.OnOpenCard();
                break;
                
            case enumGameMessage.Fortune_OnFinishedGame:
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnFinishedGame();
                break;
            default:
                return false;
        }
        return true;
    }
}