using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratPlayerMgr : SeatMgr
{
    public static BaccaratPlayerMgr Inst;

    // Start is called before the first frame update

    void Start()
    {
        if (!Inst)
            Inst = this;
        GameMgr.Inst.seatMgr = this;
    }


    internal void OnPlayerBet()
    {

        int actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        string betString = "";
        try
        {
            betString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NOW_BET];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        if (betString == "") return;

        int moneyId = int.Parse(betString.Split(':')[0]);
        int areaId = int.Parse(betString.Split(':')[1]);

        GameMgr.Inst.Log(string.Format("Player bet - actorNumber={0}, money={1}, area={2}", actorNumber, BaccaratBankerMgr.Inst.getCoinValue(moneyId), areaId),
                         LogLevel.BaccaratLogicLog);

        if (PhotonNetwork.IsMasterClient)
        {
            AddBettingLog(actorNumber, moneyId, areaId);
        }


        int dealtCoin = 0;
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            dealtCoin = BaccaratMe.Inst.OnPlayerBet(moneyId, areaId);
        }
        else
        {
            try
            {
                var p = (BaccaratUserSeat)m_playerList.Where(x => x.isSeat == true && x.m_playerInfo.m_actorNumber == actorNumber).First();
                dealtCoin = p.OnPlayerBet(moneyId, areaId);
            }
            catch(Exception err)
            {
                GameMgr.Inst.Log("Can't find the player.ActorNumber:=" + actorNumber + ", Message:=" + err.Message);
            }
        }

    }

    private void AddBettingLog(int actorNumber, int moneyId, int areaId)
    {
        string betLog = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_BETTING_LOG];
        betLog += "/" + actorNumber + ":" + moneyId + ":" + areaId;
        betLog = betLog.Trim('/');
        var pList = new PlayerInfoContainer();
        pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        try
        {
            var p = pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First();
            p.m_coinValue -= BaccaratBankerMgr.Inst.getCoinValue(moneyId);

            Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, -1},
            {Common.PLAYER_BETTING_LOG, betLog},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
        };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        catch (Exception err)
        {
            GameMgr.Inst.Log("Add Betting Log Error." + err.Message);
        }
    }

    internal void OnUpdateMe(Player player)
    {
        var info = (string)player.CustomProperties[Common.PLAYER_INFO];
        //(m_playerList.Where(x=>x.m_playerInfo.m_actorNumber == player.ActorNumber).First()).SetMe(info);
    }

    public void MoveCardEffect(GameObject obj, Vector3 pos)
    {
        iTween.MoveTo(obj, iTween.Hash("position", pos, "time", 0.5));
    }
}
