using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratMe : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratMe Inst;
    public int type;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            type = (int)BaccaratPlayerType.Player;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void PublishMe()
    {
        LogMgr.Inst.Log("Publish me called.", (int)LogLevels.MeLog_Baccarat);
        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                (int)PhotonNetwork.LocalPlayer.ActorNumber,
                DataController.Inst.userInfo.name,
                DataController.Inst.userInfo.pic,
                DataController.Inst.userInfo.coinValue,
                DataController.Inst.userInfo.skillLevel,
                DataController.Inst.userInfo.frameId,
                type
            );

                // Save my info to photon
        Hashtable props = new Hashtable
            {
                {Common.PLAYER_INFO, infoString},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Send Add New player Message. - OnUserEnteredRoom
        props = new Hashtable
            {
                {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnUserEnteredRoom},
                {Common.NEW_PLAYER_INFO, infoString},
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        BaccaratPlayerMgr.Inst.m_playerList[8].SetMe(infoString);
        
        LogMgr.Inst.Log("Tell I am entered. " + infoString, (int)LogLevels.RoomLog1);
    }
}