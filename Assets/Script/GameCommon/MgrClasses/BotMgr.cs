using System;
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
            yield return new WaitForSeconds((float)Random.Range(constantContainer.BotCreateTime_min, constantContainer.BotCreateTime_max));

            if (PhotonNetwork.IsMasterClient)
            {
                if (GameMgr.Inst.roomMgr.m_currentRoom.m_playerCount < GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer)
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
        catch { }
    }
    public IEnumerator PublishBotReady(PlayerInfo info)
    {
        yield return new WaitForSeconds(Random.Range(constantContainer.Rummy_BotReadyTime_min, constantContainer.Rummy_BotReadyTime_max));
        GameMgr.Inst.Log("Bot Ready", enumLogLevel.BotLog);
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
            GameMgr.Inst.Log("Game Player List infomation isn't correct. Error= " + err.Message, enumLogLevel.BotLog);
        }
    }

    private PlayerInfo GenerateNewBotInfo()
    {
        UserInfoModel data = new UserInfoModel();
        data.Init();
        PlayerInfo info = new PlayerInfo();

        info.m_actorNumber = -Random.Range(1000, 9999);
        info.m_coinValue = data.coinValue;
        info.m_frameId = data.frameId;
        info.m_skillLevel = data.skillLevel;
        info.m_status = enumPlayerStatus.Init;
        info.m_userName = data.name;
        info.m_userPic = data.pic;

        return info;
    }

    public virtual void Deal()
    {

    }
}
