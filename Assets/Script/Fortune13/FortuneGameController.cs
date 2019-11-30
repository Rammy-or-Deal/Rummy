using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneGameController : GameController
{
    public override void SendMessage(int messageId, Player p = null)
    {
        FortuneMessageMgr.Inst.OnMessageArrived(messageId, p);
    }
}
