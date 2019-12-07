using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LamiLogicMgr : MessageMgr
{
    public static LamiLogicMgr Inst;
    public bool isStart = false;

    private void Start()
    {
        if (!Inst)
        {
            Inst = this;
            GameMgr.Inst.messageMgr = this;
        }
    }

    public override bool OnMessageArrived(int message, Player p = null)
    {
        if (base.OnMessageArrived(message, p)) return true;

        enumGameMessage msg = (enumGameMessage)message;
        //LogMgr.Inst.Log("Message Arrived: " + (LamiMessages)message);
        GameMgr.Inst.Log("Message Arrived: " + (enumGameMessage)message);

        switch (msg)
        {
            // case (int)enumGameMessage.Rummy_OnUserLeave_M:
            //     if (PhotonNetwork.IsMasterClient)
            //         LamiPlayerMgr.Inst.OnUserLeave_M(p.ActorNumber);
            //     break;
            case enumGameMessage.Rummy_OnGameStarted:
                GameMgr.Inst.botMgr.StopCreatingBot();                
                LamiPlayerMgr.Inst.OnStartGame();
                isStart = true;
                break;
            case enumGameMessage.Rummy_OnCardDistributed:
                LamiPlayerMgr.Inst.OnCardDistributed();
                break;
            case enumGameMessage.Rummy_OnUserReadyToStart_M:
                if (PhotonNetwork.IsMasterClient)
                    LamiPlayerMgr.Inst.OnUserReadyToStart_M();
                break;
            case enumGameMessage.Rummy_OnDealCard:
                LamiPlayerMgr.Inst.OnDealCard();
                LamiPanMgr.Inst.OnDealCard();
                break;
            case enumGameMessage.Rummy_OnUserTurnChanged:
                LamiPlayerMgr.Inst.OnUserTurnChanged();
                break;
            case enumGameMessage.Rummy_OnPlayerStatusChanged:
                LamiPlayerMgr.Inst.OnPlayerStatusChanged();
                break;
            case enumGameMessage.Rummy_OnGameFinished:
                LamiPlayerMgr.Inst.OnGameFinished();
                break;
            case enumGameMessage.Rummy_OnAutoPlayer:
                LamiPlayerMgr.Inst.OnAutoPlayer();
                break;
            case enumGameMessage.Rummy_OffAutoPlayer:
                LamiPlayerMgr.Inst.OffAutoPlayer();
                break;
            case enumGameMessage.Rummy_OnShuffleRequest:
                if (PhotonNetwork.IsMasterClient)
                    LamiCardMgr.Inst.OnShuffleRequest(p);
                break;
            case enumGameMessage.Rummy_OnShuffleAccept:
                if (p == PhotonNetwork.LocalPlayer)
                    LamiMe.Inst.OnShuffleAccept();
                break;
            case enumGameMessage.Rummy_OnGameRestart:
                LamiCountdownTimer.Inst.StartTimer();
                isStart = false;
                //StartCoroutine(CreateBot());
                LamiPanMgr.Inst.OnGameRestart();
                LamiMe.Inst.OnGameRestart();
                break;
            default:
                GameMgr.Inst.Log("Message is Unknown." + msg);
                return false;
        }
        return true;
    }
}