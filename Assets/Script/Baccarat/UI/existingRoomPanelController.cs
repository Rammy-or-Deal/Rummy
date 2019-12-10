using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class existingRoomPanelController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject tableContainer;
    public UIBRoomItem originItem;

    public List<UIBRoomItem> roomList = new List<UIBRoomItem>();
    public List<string> roomInfoList = new List<string>();

    void Start()
    {
        //UIBHistory.Inst.gameObject.SetActive(false);        
    }
    List<BaccaratRoomInfo> existingList = new List<BaccaratRoomInfo>();
    internal void ShowRoomList()
    {

        roomInfoList.Clear();
        GameMgr.Inst.Log("Fit room count:=" + GameMgr.Inst.roomMgr.m_roomList.Count(x => x.m_gameType == GameMgr.Inst.m_gameType && x.m_gameTier == GameMgr.Inst.m_gameTier));
        foreach (var room in GameMgr.Inst.roomMgr.m_roomList.Where(x => x.m_gameType == GameMgr.Inst.m_gameType && x.m_gameTier == GameMgr.Inst.m_gameTier))
        {
            roomInfoList.Add(room.roomInfoString);
            GameMgr.Inst.Log("Room Info:=" + room.roomInfoString);
        }


        foreach (var uiRoom in roomList)
        {
            //Destroy(uiRoom);
            uiRoom.gameObject.SetActive(false);
        }
        roomList.Clear();

        GameMgr.Inst.Log("Showing room Count:=" + roomList.Count);
        foreach (var item in roomInfoList)
        {
            AddNewRoom(item);
        }
        GameMgr.Inst.Log("Updated Showing room Count:=" + roomList.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddNewRoom(string roomInfo)
    {
        var newInfo = Instantiate(originItem, tableContainer.transform);
        newInfo.SetMe(roomInfo);
        roomList.Add(newInfo);
    }
}
