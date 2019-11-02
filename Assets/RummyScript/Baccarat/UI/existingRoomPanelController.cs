using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class existingRoomPanelController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject tableContainer;
    public UIBRoomItem originItem;

    public List<UIBRoomItem> roomList = new List<UIBRoomItem>();
    public List<BaccaratRoomInfo> roomInfoList = new List<BaccaratRoomInfo>();
    void Start()
    {
        BaccaratRoomInfo roomInfo = new BaccaratRoomInfo();
        roomInfo.isPrivate = false;
        roomInfo.tableName = "11111";
        roomInfo.status = "";
        roomInfo.minBet = 50;
        roomInfo.maxBet = 42342;
        roomInfo.totalPlayers = 10;
        roomInfo.playersNum = 3;

        AddNewRoom(roomInfo);

        BaccaratRoomInfo roomInfo1 = new BaccaratRoomInfo();
        roomInfo1.isPrivate = true;
        roomInfo1.tableName = "22222";
        roomInfo1.status = "";
        roomInfo1.minBet = 50;
        roomInfo1.maxBet = 42342;
        roomInfo1.totalPlayers = 10;
        roomInfo1.playersNum = 3;

        AddNewRoom(roomInfo1);


        BaccaratRoomInfo roomInfo2 = new BaccaratRoomInfo();
        roomInfo2.isPrivate = true;
        roomInfo2.tableName = "33333";
        roomInfo2.status = "";
        roomInfo2.minBet = 50;
        roomInfo2.maxBet = 42342;
        roomInfo2.totalPlayers = 10;
        roomInfo2.playersNum = 3;

        AddNewRoom(roomInfo2);
    }

    internal void ShowRoomList(int roomType)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddNewRoom(BaccaratRoomInfo room)
    {
        roomInfoList.Add(room);        
        originItem = Instantiate(originItem, tableContainer.transform);
        originItem.SetMe(room);        
        roomList.Add(originItem);
    }
}
