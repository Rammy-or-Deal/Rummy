using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratMe : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratMe Inst;
    public int type;

    public bool canDeal = false;
    public bool isPanStarted = false;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            type = (int)BaccaratPlayerType.Player;

            GameMgr.Inst.Log("Now room info:=" + string.Join(",  ", GameMgr.Inst.roomMgr.m_roomList.Select(x=>x.roomInfoString)));
        }
    }

    internal void OnClickBettingArea(int moneyId, int areaId)
    {
        if (!canDeal) return;
        if (!isPanStarted) return;
        if (DataController.Inst.userInfo.coinValue < BaccaratBankerMgr.Inst.getCoinValue(moneyId)) return;

        DataController.Inst.userInfo.coinValue -= BaccaratBankerMgr.Inst.getCoinValue(moneyId);
        UpdateMe();
        
        string log = "";
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Common.PLAYER_BETTING_LOG, out object _log))
        {
            log = (string)_log;
        }
        log += "/" + moneyId + ":" + areaId;

        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnPlayerBet},
            {Common.NOW_BET, moneyId + ":" + areaId},
            {Common.PLAYER_BETTING_LOG, log}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);

        canDeal = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void PublishMe()
    {
        LogMgr.Inst.Log("Publish me called.", (int)LogLevels.MeLog_Baccarat);
        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                (int)PhotonNetwork.LocalPlayer.ActorNumber,
                DataController.Inst.userInfo.name,
                DataController.Inst.userInfo.pic,
                DataController.Inst.userInfo.coinValue,
                DataController.Inst.userInfo.skillLevel,
                DataController.Inst.userInfo.frameId,
                type
            );

        // Save my info to photon
        Hashtable props = new Hashtable
            {
                {Common.PLAYER_INFO, infoString},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Send Add New player Message. - OnUserEnteredRoom
        props = new Hashtable
            {
                {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnUserEnteredRoom},
                {Common.NEW_PLAYER_INFO, infoString},
            };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        BaccaratPlayerMgr.Inst.m_playerList[0].SetMe(infoString);

        LogMgr.Inst.Log("Tell I am entered. " + infoString, (int)LogLevels.RoomLog1);
    }

    internal void OnPrizeAwarded()
    {
        var prize = (int)PhotonNetwork.LocalPlayer.CustomProperties[Common.BACCARAT_PRIZE];
        DataController.Inst.userInfo.coinValue += prize;

        UpdateMe();
    }

    private void UpdateMe()
    {
        LogMgr.Inst.Log("Publish me called.", (int)LogLevels.MeLog_Baccarat);
        string infoString = "";
        infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                (int)PhotonNetwork.LocalPlayer.ActorNumber,
                DataController.Inst.userInfo.name,
                DataController.Inst.userInfo.pic,
                DataController.Inst.userInfo.coinValue,
                DataController.Inst.userInfo.skillLevel,
                DataController.Inst.userInfo.frameId,
                type
            );

        // Save my info to photon
        Hashtable props = new Hashtable
            {
                {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnUpdateMe},
                {Common.PLAYER_INFO, infoString},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    internal void OnPlayerBet()
    {

        string betString = (string)PhotonNetwork.LocalPlayer.CustomProperties[Common.NOW_BET];

        LogMgr.Inst.Log("MyLog:=" + (string)PhotonNetwork.LocalPlayer.CustomProperties[Common.PLAYER_BETTING_LOG], (int)LogLevels.PlayerLog1);

        int moneyId = int.Parse(betString.Split(':')[0]);
        int areaId = int.Parse(betString.Split(':')[1]);

        var x = UIBBetBtnList.Inst.btns[moneyId].gameObject.transform.position.x;
        var y = UIBBetBtnList.Inst.btns[moneyId].gameObject.transform.position.y;

        BaccaratPanMgr.Inst.OnPlayerBet(x, y, moneyId, areaId);

        canDeal = true;
    }

    internal void OnEndPan()
    {
        isPanStarted = false;

    }

    internal void OnStartNewPan()
    {
        isPanStarted = true;
        canDeal = true;
        UIBBetBtnList.Inst.Init();
    }

}