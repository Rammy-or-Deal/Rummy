using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

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
            GameMgr.Inst.m_gameTier = enumGameTier.BaccaratRegular;
        }
        catch { }
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");
    }
    void Start()
    {
        if (!Inst)
            Inst = this;
        //GameMgr.Inst.Log("Now room info:=" + string.Join(",  ", GameMgr.Inst.roomMgr.m_roomList.Select(x=>x.roomInfoString)));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickRoomViewDialog(int id)
    {
        if (id == (int)enumGameTier.BaccaratCreateRoom)
        {
            existingRoomPanel.gameObject.SetActive(false);
            newRoomPanel.gameObject.SetActive(true);
        }
        else
        {
            existingRoomPanel.gameObject.SetActive(true);
            newRoomPanel.gameObject.SetActive(false);
            ShowRoomList();
        }
    }

    private void ShowRoomList()
    {
        //existingRoomPanel
        existingRoomPanel.ShowRoomList();
    }

    public void CreateBaccaratRoom_Type(int id)
    {        
        newRoomPanel.ShowRoom(staticFunction_Baccarat.GetBaccaratRoomInfoFromTier(GameMgr.Inst.m_gameTier));
    }
    public void CreateBaccaratRoom()
    {

    }
}

public class BaccaratRoomInfo
{
    public bool isPrivate;
    public int minBet;
    public int maxBet;
    public string status;
    public int totalPlayers;
    public string password;
    public int coin;
    public int gem;
    public BaccaratRoomInfo()
    {
        isPrivate = false;
        minBet = 0;
        maxBet = 0;
        status = "";
        totalPlayers = 0;
        password = "";
        gem = 0;
        coin = 0;
    }
    public string roomString
    {
        get
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", isPrivate, minBet, maxBet, status, totalPlayers, password);
        }
        set
        {
            try
            {
                var list = value.Split(':');
                isPrivate = bool.Parse(list[0]);
                minBet = int.Parse(list[1]);
                maxBet = int.Parse(list[2]);
                status = list[3];
                totalPlayers = int.Parse(list[4]);
                password = list[5];
            }
            catch { }
        }
    }
}
