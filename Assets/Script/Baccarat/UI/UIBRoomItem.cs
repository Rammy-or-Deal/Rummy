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

    [HideInInspector] public BaccaratRoomInfo baccaratRoomInfo = null;
    [HideInInspector] public string roomName;
    [HideInInspector] public int maxPlayer;
    [HideInInspector] public int nowPlayer;

    [HideInInspector]public GameRoomInfo commonRoomInfo = null;
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
        if(commonRoomInfo == null)
            commonRoomInfo = new GameRoomInfo();
        if(baccaratRoomInfo == null)
            baccaratRoomInfo = new BaccaratRoomInfo();    
        
        commonRoomInfo.roomInfoString = roomInfoString;        
        baccaratRoomInfo.roomString = commonRoomInfo.m_additionalString;
        GameMgr.Inst.Log("Newly created room info(origin):=" + roomInfoString);
        GameMgr.Inst.Log("Newly created room info(additional):=" + commonRoomInfo.m_additionalString);
        GameMgr.Inst.Log("Newly created room info(parsed):=" + baccaratRoomInfo.roomString);

        roomName = commonRoomInfo.m_roomName;
        maxPlayer = commonRoomInfo.m_maxPlayer;
        nowPlayer = commonRoomInfo.m_playerCount;

        UI_tableName.text = roomName;

        UI_isPrivate.gameObject.SetActive(baccaratRoomInfo.isPrivate);
        UI_minBet.text = baccaratRoomInfo.minBet.ToString();
        UI_maxBet.text = baccaratRoomInfo.maxBet.ToString();
        UI_players.text = nowPlayer.ToString();
        //UI_players.text = roomInfo.playersNum + " / " + roomInfo.totalPlayers;
    }
    public void JoinRoom()
    {        
        GameMgr.Inst.Log("Try to join room. roomInfo=" + commonRoomInfo.roomInfoString);
        
        if(baccaratRoomInfo.isPrivate)
        {
            UIBPasswordVerificationDlg.Inst.CheckPassword(commonRoomInfo.roomInfoString);
        }
        else if(!GameMgr.Inst.roomMgr.JoinRoom(roomName))    // If there's no room, Create room based on roomInfo
        {
            GameMgr.Inst.Log("There's no room. So I should create a new room");
            GameMgr.Inst.roomMgr.CreateRoom_basedRoomInfo(commonRoomInfo);
        }
    }
}
