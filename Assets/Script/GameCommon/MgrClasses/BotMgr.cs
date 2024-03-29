﻿using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using RummyScript.Model;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class BotMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void CreateBot()
    {
        GameMgr.Inst.Log("CreateBot function called.");
        StartCreatingBot();
    }

    IEnumerator CreateBotTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds((float)Random.Range(Constant.BotCreateTime_min, Constant.BotCreateTime_max));
            var pList = new PlayerInfoContainer();
            pList.GetInfoContainerFromPhoton();
            if (PhotonNetwork.IsMasterClient)
            {
                //if ((GameMgr.Inst.m_gameType != enumGameType.Baccarat && GameMgr.Inst.roomMgr.m_currentRoom.m_playerCount < GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer)
                if ((GameMgr.Inst.m_gameType != enumGameType.Baccarat && GameMgr.Inst.roomMgr.m_currentRoom.m_playerCount < GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer)
                 || (GameMgr.Inst.m_gameType == enumGameType.Baccarat && pList.m_playerList.Count(x=>x.m_actorNumber < 0) < 2)
                )
                {
                    PlayerInfo info = GenerateNewBotInfo();
                    Hashtable props = new Hashtable{
                        {PhotonFields.GAME_MESSAGE, enumGameMessage.OnUserEnteredRoom_onlyMaster},
                        {PhotonFields.NEW_PLAYER_INFO, info.playerInfoString}
                    };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    GameMgr.Inst.Log("New Bot Generated. + BotString=" + info.playerInfoString);
                    if (GameMgr.Inst.m_gameType == enumGameType.Lami)
                        StartCoroutine(PublishBotReady(info));
                }
            }
        }
    }
    Coroutine creatingBotRoutine;
    public void StartCreatingBot()
    {
        creatingBotRoutine = StartCoroutine(CreateBotTimer());
    }

    public virtual void RejectThisBot(UserSeat bot)
    {
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnPlayerLeftRoom_onlyMaster_bot},
            {Common.PLAYER_ID, bot.m_playerInfo.m_actorNumber}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        ///OnPlayerLeftRoom_onlyMaster_bot
    }

    public void StopCreatingBot()
    {
        try
        {
            StopCoroutine(creatingBotRoutine);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public IEnumerator PublishBotReady(PlayerInfo info)
    {
        yield return new WaitForSeconds(Random.Range(Constant.Rummy_BotReadyTime_min, Constant.Rummy_BotReadyTime_max));
        GameMgr.Inst.Log("Bot Ready", LogLevel.BotLog);
        PublishIamReady(info);
    }
    public static void PublishIamReady(PlayerInfo info)
    {
        GameMgr.Inst.Log("PublishIamReady is called");
        PlayerInfoContainer pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        try
        {
            var user = pList.m_playerList.Where(x => x.m_userName == info.m_userName).First();

            user.m_status = enumPlayerStatus.Rummy_Ready;

            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, enumGameMessage.OnSeatStringUpdate},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            GameMgr.Inst.Log("New Bot Message Generated. + BotString=" + user.playerInfoString + ", status=" + (int)user.m_status + "/" + (int)enumPlayerStatus.Rummy_Ready);
        }
        catch (Exception err)
        {
            GameMgr.Inst.Log("Game Player List infomation isn't correct. Error= " + err.Message, LogLevel.BotLog);
        }
    }

    private PlayerInfo GenerateNewBotInfo()
    {
        UserInfoModel data = new UserInfoModel();
        data.Init();
        PlayerInfo info = new PlayerInfo();

        info.m_actorNumber = -Random.Range(1000, 9999);
        info.m_coinValue = data.coin;
        info.m_frameId = data.frame_id;
        info.m_skillLevel = data.skill_level;
        info.m_status = enumPlayerStatus.Init;
        info.m_userName = data.name;
        info.m_userPic = data.pic;

        return info;
    }

    public virtual void Deal()
    {

    }
}
