using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaccaratRoomController : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratRoomController Inst;
    public existingRoomPanelController existingRoomPanel;
    public newRoomPanelController newRoomPanel;
    public Text UIPassword;
    BaccaratRoomInfo tmpRoomInfo = new BaccaratRoomInfo();

    void Awake()
    {
        try
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(false);
        }
        catch { }
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");
    }
    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickCreateRoomDialog()
    {
        existingRoomPanel.gameObject.SetActive(false);
        newRoomPanel.gameObject.SetActive(true);
    }
    public void OnClickRoomViewDialog(int id)
    {
        existingRoomPanel.gameObject.SetActive(true);
        newRoomPanel.gameObject.SetActive(false);

        ShowRoomList(id);
    }

    private void ShowRoomList(int id)
    {
        //existingRoomPanel
        existingRoomPanel.ShowRoomList(id);
    }

    public void CreateBaccaratRoom_Type(int id)
    {
        tmpRoomInfo = tmpRoomInfo.InitRoomByType(id);
        newRoomPanel.ShowRoom(tmpRoomInfo);
    }
    public void CreateBaccaratRoom()
    {

    }    
}

public class BaccaratRoomInfo
{
    public string tableName;
    public bool isPrivate;
    public int minBet;
    public int maxBet;
    public string status;
    public int playersNum;
    public int totalPlayers;
    public int roomNo;
    public string password;
    public int roomType;
    public int coin;
    public int gem;
    public BaccaratRoomInfo()
    {
        tableName = "baccaratTable";
        isPrivate = false;
        minBet = 0;
        maxBet = 0;
        status = "";
        playersNum = 0;
        totalPlayers = 0;
        roomNo = 0;
        password = "";
        roomType = -1;
        gem = 0;
        coin = 0;
    }
    public BaccaratRoomInfo InitRoomByType(int id)
    {
        switch (id)
        {
            case (int)BaccaratRoomType.Regular:
                minBet = 100;
                maxBet = 12500;
                totalPlayers = 9;
                roomType = id;
                coin = 1500;
                gem = 1;
                break;
            case (int)BaccaratRoomType.Silver:
                minBet = 1000;
                maxBet = 125000;
                totalPlayers = 9;
                roomType = id;
                coin = 15000;
                gem = 10;
                break;
            case (int)BaccaratRoomType.Gold:
                minBet = 5000;
                maxBet = 625000;
                totalPlayers = 9;
                roomType = id;
                coin = 75000;
                gem = 50;
                break;        
            case (int)BaccaratRoomType.Platinum:
                minBet = 10000;
                maxBet = 1250000;
                totalPlayers = 9;
                roomType = id;
                coin = 150000;
                gem = 100;
                break;
        }
        return this;
    }
    public string roomString
    {
        get
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", tableName, isPrivate, minBet, maxBet, status, playersNum, totalPlayers, roomNo, password, roomType);
        }
        set
        {
            var list = value.Split(':');
            tableName = list[0];
            isPrivate = bool.Parse(list[1]);
            minBet = int.Parse(list[2]);
            maxBet = int.Parse(list[3]);
            status = list[4];
            playersNum = int.Parse(list[5]);
            totalPlayers = int.Parse(list[6]);
            roomNo = int.Parse(list[7]);
            password = list[8];
            roomType = int.Parse(list[9]);
        }
    }
}
