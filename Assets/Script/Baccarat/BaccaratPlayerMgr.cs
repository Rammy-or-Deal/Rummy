﻿using System;
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

    // internal void OnJoinSuccess()
    // {
    //     try
    //     {
    //         UIController.Inst.loadingDlg.gameObject.SetActive(false);
    //     }
    //     catch { }

    //     BaccaratMe.Inst.PublishMe();

    //     foreach (var player in PhotonNetwork.PlayerList)
    //     {
    //         if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) continue;

    //         if (m_playerList.Count(x => x.isSeat == false) > 0)
    //             (m_playerList.Where(x => x.isSeat == false).First()).SetMe((string)player.CustomProperties[Common.PLAYER_INFO]);
    //     }
    // }

    // internal async void OnUserEnteredRoom()
    // {
    //     var infostring = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NEW_PLAYER_INFO];

    //     if (int.Parse(infostring.Split(':')[0]) == PhotonNetwork.LocalPlayer.ActorNumber)
    //     {
    //         BaccaratPlayerMgr.Inst.m_playerList[0].SetMe(infostring);

    //         BaccaratPanMgr.Inst.message.Show("Please wait until this pan is completed.");
    //         await Task.Delay(3000);
    //         BaccaratPanMgr.Inst.message.Hide();
    //         foreach (var player in PhotonNetwork.PlayerList)
    //         {
    //             if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) continue;

    //             if (m_playerList.Count(x => x.isSeat == false) > 0)
    //                 m_playerList.Where(x => x.isSeat == false).First().SetMe((string)player.CustomProperties[Common.PLAYER_INFO]);
    //         }
    //     }
    //     else
    //     {
    //         if (m_playerList.Count(x => x.isSeat == false) > 0)
    //             m_playerList.Where(x => x.isSeat == false).First().SetMe(infostring);
    //     }

    //     //BaccaratMe.Inst.PublishMe();
    // }

    internal void OnPlayerBet()
    {
        int actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        int dealtCoin = 0;
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            dealtCoin = BaccaratMe.Inst.OnPlayerBet();
        }
        else
        {
            try
            {
                var p = (BaccaratUserSeat)m_playerList.Where(x => x.m_playerInfo.m_actorNumber == actorNumber).First();
                dealtCoin = p.OnPlayerBet();
            }
            catch { }
        }

        if(!PhotonNetwork.IsMasterClient || dealtCoin == 0) return;

        PlayerInfoContainer pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        try
        {
            var user = pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First();
            user.m_coinValue -= dealtCoin;
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, enumGameMessage.OnSeatStringUpdate},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        catch (Exception err)
        {
            GameMgr.Inst.Log("Game Player List infomation isn't correct. Error= " + err.Message, enumLogLevel.BotLog);
        }

    }

    // internal void OnUserLeave(Player player)
    // {
    //     if (m_playerList.Count(x => x.m_playerInfo.m_actorNumber == player.ActorNumber) > 0)
    //     {
    //         m_playerList.Where(x => x.m_playerInfo.m_actorNumber == player.ActorNumber).First().OnUserLeave();
    //     }
    // }

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
