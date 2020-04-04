using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newRoomPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text minBet;
    public Text maxBet;
    public Text maxPlayers;

    public Text coin;
    public Text gem;

    public List<BaccaratRoomMoneyButton> moneyList;
    public List<BaccaratRoomMoneyButton> privacyList;
    public InputField password;

    public BaccaratRoomInfo roomInfo = new BaccaratRoomInfo();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCreateTable()
    {
        //Debug.Log("password:="+password.text);
        GetMoney();

        GameMgr.Inst.Log("Current Game Tier = " + GameMgr.Inst.m_gameTier);
        var tmpRoom = staticFunction_Baccarat.GetBaccaratRoomInfoFromTier(GameMgr.Inst.m_gameTier);
        tmpRoom.password = password.text;
        tmpRoom.isPrivate = privacyList[1].isSelected;
        tmpRoom.status = "";

        GameMgr.Inst.roomMgr.CreateRoom(GameMgr.Inst.m_gameType, GameMgr.Inst.m_gameTier, tmpRoom.roomString);
        GameMgr.Inst.Log("Current Room Info = " + tmpRoom.roomString + ", gameType="+GameMgr.Inst.m_gameType + ", gameTier="+GameMgr.Inst.m_gameTier);
        // PunController.Inst.CreateBacaratRoom(roomInfo);
    }

    private void GetMoney()
    {
        if(moneyList[0].isSelected)
        {
            DataController.Inst.userInfo.coin -= roomInfo.coin;
        }
        else{
            DataController.Inst.userInfo.leaf_value -= roomInfo.gem;
        }
    }

    public void ShowRoom(BaccaratRoomInfo info)
    {
        this.roomInfo = info;
        minBet.text = string.Format("{0:n0}", roomInfo.minBet);
        maxBet.text = string.Format("{0:n0}", roomInfo.maxBet);
        maxPlayers.text = string.Format("{0:n0}", roomInfo.maxBet*roomInfo.totalPlayers);
        coin.text = string.Format("{0:n0} coins", roomInfo.coin);
        gem.text = roomInfo.gem + " gem";
        if(roomInfo.gem > 1)
            gem.text = gem.text + "s";
    }
}
