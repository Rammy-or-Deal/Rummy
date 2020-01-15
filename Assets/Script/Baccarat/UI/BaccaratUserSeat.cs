using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using RummyScript.Model;
using UnityEngine;
using UnityEngine.UI;

public class BaccaratUserSeat : UserSeat
{
    //user info

    //seat state
    public Image firstImage;
    public GameObject userBack;
    //
    public int id = -1;
    public int type;

    public Transform[] cardPos;

    
    public override void SetPlayerInfo(PlayerInfo info)
    {
        base.SetPlayerInfo(info);
       
    }

    public int OnPlayerBet(int moneyId, int areaId)
    {
        try
        {
            BaccaratPanMgr.Inst.OnPlayerBet(gameObject.transform.position, moneyId, areaId);            

            return BaccaratBankerMgr.Inst.getCoinValue(moneyId);
        }
        catch { return 0; }
        
    }

    public void OnClick()
    {

    }
}
