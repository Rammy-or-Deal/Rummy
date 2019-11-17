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
        UIBHistory.Inst.gameObject.SetActive(false);    
    }
    List<BaccaratRoomInfo> existingList = new List<BaccaratRoomInfo>();
    internal void ShowRoomList(int roomType)
    {
        roomInfoList.Clear();
        foreach(var room in PunController.Inst.baccaratRoomList)
        {
            BaccaratRoomInfo info = new BaccaratRoomInfo();
            info.roomString = room.Value;
            if(info.roomType == roomType)
            {
                //baccaratRoomList.Add(room.Key, room.Value);
                info.tableName = room.Key;
                roomInfoList.Add(info);
            }
        }
        roomList.Clear();
        try{
        foreach(var item in roomInfoList)
        {
            AddNewRoom(item);
        }
        }catch{}

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
