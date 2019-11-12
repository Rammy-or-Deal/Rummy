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
public class FortuneMessageMgr : MonoBehaviour
{
    public static FortuneMessageMgr Inst;
    // Start is called before the first frame update
    public FortuneGameStatus nowGameStatus;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            RoomMessageManagement.Inst.GameID = 30;
            nowGameStatus = FortuneGameStatus.Init;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMessageArrived(int messageId, Player p = null)
    {
        try
        {
            Debug.Log((FortuneMessages)messageId + " is Called.");
        }
        catch { }
        switch (messageId)
        {
            case (int)RoomManagementMessages.OnUserSit: // This function is used only one time - start time.
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayMgr.Inst.OnUserSit();        // set status to canStart
                break;
            case (int)FortuneMessages.OnCardDistributed:
                FortuneMe.Inst.OnCardDistributed();         // set status to Ready
                FortunePanMgr.Inst.OnCardDistributed();
                break;
            case (int)FortuneMessages.OnUserReady:
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayMgr.Inst.OnUserReady();
                break;
            case (int)FortuneMessages.OnGameStarted:
                nowGameStatus = FortuneGameStatus.GameStarted;  // started
                FortuneMe.Inst.OnGameStarted();           // set status to OnChanging
                break;
            case (int)FortuneMessages.OnPlayerDealCard:
                FortunePlayMgr.Inst.OnPlayerDealCard();
                break;
            case (int)FortuneMessages.OnOpenCard:
                FortunePanMgr.Inst.OnOpenCard();
                FortunePlayMgr.Inst.OnOpenCard();
                break;
            case (int)FortuneMessages.OnFinishedGame:
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayMgr.Inst.OnFinishedGame();
                break;
            default:
                RoomMessageManagement.Inst.OnMessageArrived(messageId, p);
                break;
        }
    }
}