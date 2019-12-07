using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MeMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void PublishMe()
    {
        GameMgr.Inst.Log("Publish me called.", enumLogLevel.MeLog);
        GameMgr.Inst.m_playerStatus = enumPlayerStatus.Init;
        int status = (int)GameMgr.Inst.m_playerStatus;

        string infoString = "";
        PlayerInfo info = new PlayerInfo();
        info.setValues((int)PhotonNetwork.LocalPlayer.ActorNumber, GameMgr.Inst.m_playerStatus, DataController.Inst);
        infoString = info.playerInfoString;

        // Set local player's property.
        Hashtable props = new Hashtable
            {
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnUserEnteredRoom_onlyMaster},
                {PhotonFields.NEW_PLAYER_INFO, infoString}
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        GameMgr.Inst.Log("Tell I am entered. " + infoString, enumLogLevel.MeLog);
        GameMgr.Inst.Log("This room info :=" + (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.RoomInfo]);
    }
}