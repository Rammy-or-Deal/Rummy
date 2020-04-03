using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SceneMgr : MonoBehaviour
{
    void Start()
    {
        
    }
    
    public void LoadGameScene2(enumGameType v)
    {
        string sceneString = "";
        switch (v)
        {
            case enumGameType.Lami:
                sceneString = Constant.strScene2Lami;
                break;
            case enumGameType.Baccarat:
                sceneString = Constant.strScene2Bacccarat;
                break;
            case enumGameType.Fortune13:
                sceneString = Constant.strScene2Fortune;
                break;
            default:
                sceneString = "";
                break;
        }
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        PhotonNetwork.LoadLevel(sceneString);
    }
}
