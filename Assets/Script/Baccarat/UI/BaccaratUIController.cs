using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaccaratUIController : GameUIController
{
    public static BaccaratUIController Inst;
    public GameObject bendCardBlankBtn;
    
    public void Awake()
    {
        base.Awake();
        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
        base.Start();
//        PhotonNetwork.InstantiateSceneObject("baccarat/CardBend", Vector3.zero, Quaternion.identity, 0);
    }

    public void OnClickBettingArea(int id) // Id=0,1,2,3,4
    {
//        LogMgr.Inst.Log("Clicked Betting Area. id="+id, (int)LogLevels.PlayerLog1);

        if(UIBBetBtnList.Inst.selectedId == -1) return;
        BaccaratMe.Inst.OnClickBettingArea(UIBBetBtnList.Inst.selectedId, id);
    }
}
    