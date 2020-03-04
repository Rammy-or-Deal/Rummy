using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratMe : MeMgr
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
            GameMgr.Inst.meMgr = this;
            GameMgr.Inst.Log("Now room info:=" + string.Join(",  ", GameMgr.Inst.roomMgr.m_roomList.Select(x=>x.roomInfoString)));

            PublishMe();
        }
    }

    internal void OnClickBettingArea(int moneyId, int areaId)
    {
        if (!canDeal) return;
        if (!isPanStarted) return;
        if (DataController.Inst.userInfo.coin_value < BaccaratBankerMgr.Inst.getCoinValue(moneyId)) return;

        //DataController.Inst.userInfo.coinValue -= BaccaratBankerMgr.Inst.getCoinValue(moneyId);
        //UpdateMyCoin(DataController.Inst.userInfo.coinValue);
                
        Bet(moneyId, areaId, PhotonNetwork.LocalPlayer.ActorNumber);

        canDeal = false;
    }

    private void UpdateMyCoin()
    {
        int coinValue = 0;
        var pList = new PlayerInfoContainer();
        pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        coinValue = (int)pList.m_playerList.Where(x=>x.m_actorNumber == PhotonNetwork.LocalPlayer.ActorNumber).First().m_coinValue;
        var mySeat = GameMgr.Inst.seatMgr.m_playerList[0];
        mySeat.mCoinValue.text = coinValue.ToString();
        DataController.Inst.userInfo.coin_value = coinValue;
    }

    public static void Bet(int moneyId, int areaId, int actorNumber)
    {
        // string log = "";
        // if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(Common.PLAYER_BETTING_LOG, out object _log))
        // {
        //     log = (string)_log;
        // }
        // log += "/" + actorNumber + ":" + moneyId + ":" + areaId;

        Hashtable table = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Baccarat_OnPlayerBet},
            {Common.PLAYER_ID, actorNumber},
            {Common.NOW_BET, moneyId + ":" + areaId},
            // {Common.PLAYER_BETTING_LOG, log}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnPrizeAwarded()
    {
        var prize = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_PRIZE];
        DataController.Inst.userInfo.coin_value += prize;

        //UpdateMe();
    }

    

    internal int OnPlayerBet(int moneyId, int areaId)
    {       
        //DataController.Inst.userInfo.coinValue -= BaccaratBankerMgr.Inst.getCoinValue(moneyId);
        BaccaratPanMgr.Inst.OnPlayerBet(UIBBetBtnList.Inst.btns[moneyId].gameObject.transform.position, moneyId, areaId);
        canDeal = true;
        UpdateMyCoin();

        return BaccaratBankerMgr.Inst.getCoinValue(moneyId);
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