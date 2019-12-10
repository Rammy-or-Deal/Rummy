using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBRoomItem : MonoBehaviour
{
    // Start is called before the first frame update

    public Text UI_tableName;
    public Image UI_isPrivate;
    public Text UI_minBet;
    public Text UI_maxBet;
    public Button UI_status;
    public Text UI_players;
    public Button UI_join;
    #region  Unity

    [HideInInspector] public BaccaratRoomInfo roomInfo;
    [HideInInspector] public string roomName;
    [HideInInspector] public int maxPlayer;
    [HideInInspector] public int nowPlayer;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    internal void SetMe(string roomInfoString)//BaccaratRoomInfo room)
    {

        GameRoomInfo tmpRoom = new GameRoomInfo();
        tmpRoom.roomInfoString = roomInfoString;

        roomInfo = new BaccaratRoomInfo();
        roomInfo.roomString = tmpRoom.m_additionalString;
        GameMgr.Inst.Log("Newly created room info(origin):=" + roomInfoString);
        GameMgr.Inst.Log("Newly created room info(additional):=" + tmpRoom.m_additionalString);
        GameMgr.Inst.Log("Newly created room info(parsed):=" + roomInfo.roomString);

        roomName = tmpRoom.m_roomName;
        maxPlayer = tmpRoom.m_maxPlayer;
        nowPlayer = tmpRoom.m_playerCount;

        UI_tableName.text = roomName;

        UI_isPrivate.gameObject.SetActive(roomInfo.isPrivate);
        UI_minBet.text = roomInfo.minBet.ToString();
        UI_maxBet.text = roomInfo.maxBet.ToString();
        UI_players.text = nowPlayer.ToString();
        //UI_players.text = roomInfo.playersNum + " / " + roomInfo.totalPlayers;
    }
    public void JoinRoom()
    {        
        //PunController.Inst.JoinRoom(this.roomInfo.tableName);
    }
}
