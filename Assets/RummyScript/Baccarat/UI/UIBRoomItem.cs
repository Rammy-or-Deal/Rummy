using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBRoomItem : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public BaccaratRoomInfo roomInfo;


    public Text UI_tableName;
    public Image UI_isPrivate;
    public Text UI_minBet;
    public Text UI_maxBet;
    public Button UI_status;
    public Text UI_players;
    public Button UI_join;

    #region  Unity
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    internal void SetMe(BaccaratRoomInfo room)
    {
        this.roomInfo = room;
        UI_tableName.text = roomInfo.tableName;

        UI_isPrivate.gameObject.SetActive(roomInfo.isPrivate);
        UI_minBet.text = roomInfo.minBet.ToString();
        UI_maxBet.text = roomInfo.maxBet.ToString();
        UI_players.text = roomInfo.playersNum + " / " + roomInfo.totalPlayers;
    }
    public void JoinRoom()
    {
        
        PunController.Inst.JoinRoom(this.roomInfo.tableName);
    }
}
