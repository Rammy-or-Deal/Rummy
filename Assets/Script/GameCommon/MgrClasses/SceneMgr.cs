using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SceneMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameScene2(enumGameType v)
    {
         string sceneString = "";
        switch (v)
        {
            case enumGameType.Lami:
                sceneString = constantContainer.strScene2Lami;
                break;
            case enumGameType.Baccarat:
                sceneString = constantContainer.strScene2Bacccarat;
                break;
            case enumGameType.Fortune13:
                sceneString = constantContainer.strScene2Fortune;
                break;
            default:
                sceneString = "";
                break;
        }
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        PhotonNetwork.LoadLevel(sceneString);
    }
}
