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
                    FortunePlayerMgr.Inst.OnUserSit();       
                break;

            case enumGameMessage.Fortune_InitReady:
                if(PhotonNetwork.IsMasterClient)
                {
                    FortunePlayerMgr.Inst.InitReady();
                }
                break;
                */
            case enumGameMessage.Fortune_OnCardDistributed:
                FortuneMe.Inst.OnCardDistributed();         // set status to Ready
                FortuneBotMgr.Inst.OnCardDistributed();
                FortunePanMgr.Inst.OnCardDistributed();
                break;
            case enumGameMessage.Fortune_OnUserReady:
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnUserReady();
                break;
            case enumGameMessage.Fortune_OnGameStarted:
                nowGameStatus = FortuneGameStatus.GameStarted;  // started
                FortuneBotMgr.Inst.OnGameStarted();
                FortuneMe.Inst.OnGameStarted();           // set status to OnChanging
                break;
            case enumGameMessage.Fortune_OnTickTimer:
                FortunePanMgr.Inst.OnTickTimer();
                if(PhotonNetwork.IsMasterClient)
                    FortunePlayerMgr.Inst.OnTickTimer();
                break;
            case enumGameMessage.Fortune_DoubleDownRequest:
                FortunePlayerMgr.Inst.OnPlayerDealCard(enumPlayerStatus.Fortune_Doubled);
                break;
            case enumGameMessage.Fortune_OnPlayerDealCard:
                FortunePlayerMgr.Inst.OnPlayerDealCard(enumPlayerStatus.Fortune_dealtCard);
                break;
            case enumGameMessage.Fortune_OnOpenCard:
                FortunePanMgr.Inst.OnOpenCard();
                FortunePlayerMgr.Inst.OnOpenCard();
                break;
            case enumGameMessage.Fortune_OnFinishedGame:
                FortunePlayerMgr.Inst.OnFinishedGame();
                break;
            default:
                return false;
        }
        return true;
    }
}