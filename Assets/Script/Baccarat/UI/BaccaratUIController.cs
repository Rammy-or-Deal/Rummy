﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaccaratUIController : GameUIController
{
    public static BaccaratUIController Inst;
    public GameObject bendCardBlankBtn;
    public GameObject camera;
    
    public void Awake()
    {
        base.Awake();
        if (!Inst)
            Inst = this;
    }

    public void OnClickBettingArea(int id) // Id=0,1,2,3,4
    {
//        LogMgr.Inst.Log("Clicked Betting Area. id="+id, (int)LogLevels.PlayerLog1);

        if(UIBBetBtnList.Inst.selectedId == -1) return;
        BaccaratMe.Inst.OnClickBettingArea(UIBBetBtnList.Inst.selectedId, id);
    }
}
