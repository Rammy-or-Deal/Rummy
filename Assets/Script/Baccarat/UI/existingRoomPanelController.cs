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

        ShowDefaultRoom(GameMgr.Inst.m_gameTier);

        GameMgr.Inst.Log("Fit room count:=" + GameMgr.Inst.roomMgr.m_roomList.Count(x => x.m_gameType == GameMgr.Inst.m_gameType && x.m_gameTier == GameMgr.Inst.m_gameTier));
        foreach (var room in GameMgr.Inst.roomMgr.m_roomList.Where(x => x.m_gameType == GameMgr.Inst.m_gameType
                                                                        && x.m_gameTier == GameMgr.Inst.m_gameTier
                                                                        && !x.m_roomName.Contains(constantContainer.defaultRoomPrefix)))
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

    private void ShowDefaultRoom(enumGameTier m_gameTier)
    {
        // Create a default room.
        int roomCount = constantContainer.BaccaratDefaultRoomCount;
        string defaultRoomPrefix = constantContainer.defaultRoomPrefix;
        defaultRoomPrefix += m_gameTier;

        for (int i = 1; i <= roomCount; i++)
        {
            var tblName = defaultRoomPrefix + i;
            string roomInfoString = "";
            if (GameMgr.Inst.roomMgr.m_roomList.Count(x => x.m_roomName == tblName) > 0)
            {
                var room = GameMgr.Inst.roomMgr.m_roomList.Where(x => x.m_roomName == tblName).First();
                roomInfoString = room.roomInfoString;
                GameMgr.Inst.Log("Room Info:=" + room.roomInfoString);
            }
            else
            {
                GameRoomInfo tmpRoom = new GameRoomInfo();
                tmpRoom.m_gameType = GameMgr.Inst.m_gameType;
                tmpRoom.m_gameTier = GameMgr.Inst.m_gameTier;
                tmpRoom.m_gameFee = GameMgr.Inst.roomMgr.GetGameFeeOfGame(GameMgr.Inst.m_gameType, GameMgr.Inst.m_gameTier);;
                tmpRoom.m_maxPlayer = GameMgr.Inst.roomMgr.GetMaxPlayerOfGame(GameMgr.Inst.m_gameType, GameMgr.Inst.m_gameTier);
                tmpRoom.m_playerCount = 0;
                tmpRoom.m_roomName = tblName;

                var baccaratRoom = staticFunction_Baccarat.GetBaccaratRoomInfoFromTier(GameMgr.Inst.m_gameTier);
                tmpRoom.m_additionalString = baccaratRoom.roomString;
                roomInfoString = tmpRoom.roomInfoString;
            }
            roomInfoList.Add(roomInfoString);
        }
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
