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
}

public class FortuneMessageMgr : MonoBehaviour
{
    public static FortuneMessageMgr Inst;
    // Start is called before the first frame update
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            RoomMessageManagement.Inst.GameID = 30;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMessageArrived(int messageId, Player p = null)
    {
        switch (messageId)
        {
            case (int)RoomManagementMessages.OnUserSit: // This function is used only one time - start time.
                if (PhotonNetwork.IsMasterClient)
                    FortunePlayMgr.Inst.OnUserSit();
                break;
            case (int)FortuneMessages.OnUserReady:

                break;
            case (int)FortuneMessages.OnGameStarted:

                break;
            case (int)FortuneMessages.OnCardDistributed:
                FortuneMe.Inst.OnCardDistributed();
                FortunePanMgr.Inst.OnCardDistributed();
                break;
            default:
                RoomMessageManagement.Inst.OnMessageArrived(messageId, p);
                break;
        }
    }
}