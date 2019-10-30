using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratGameController : MonoBehaviour
{
    public static BaccaratGameController Inst;
    
    void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }
    void Start()
    {
        
    }

    public void SendMessage(int messageId, Player p = null)
    {
        BaccaratMessageMgr.Inst.OnMessageArrived(messageId, p);
    }



    public void ShowPlayers()
    {

    }

}
