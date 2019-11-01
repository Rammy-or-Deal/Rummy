using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaccaratRoomController : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratRoomController Inst;
    public existingRoomPanelController existingRoomPanel;
    public newRoomPanelController newRoomPanel;

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
