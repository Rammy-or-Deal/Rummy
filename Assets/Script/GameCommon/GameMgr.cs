using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;



public class GameMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameMgr Inst;

    #region  Game Management Classes    
    
    public RoomMgr roomMgr;
    public DebugMgr debugMgr;
    public SceneMgr sceneMgr;
    [HideInInspector]public MessageMgr messageMgr;
    [HideInInspector]public BotMgr botMgr;    
    [HideInInspector]public ResultMgr resultMgr;    
    
    #endregion

    #region Game Status Variables
    public enumGameStatus m_gameStatus;
    public enumGameType m_gameType;
    public enumGameTier m_gameTier;
    #endregion

    #region UnityEngine
    private void Awake() {
        if(!Inst)
            Inst = this;
    }
    #endregion
    internal void LoadGameScene2(enumGameType v)
    {
        sceneMgr.LoadGameScene2(v);     

        m_gameType = v;
        m_gameStatus = enumGameStatus.InGameLevelSelect;

        Log("Load 2 Scene");
    }

    public void EnterTier(enumGameTier tier)
    {
        m_gameTier = tier;
        Log("TierButton click: " + tier);
        m_gameStatus = enumGameStatus.InGamePlay;

        // Debug.Log("TierButton click:" + type + "  roomCount:" + PunController.Inst.cachedRoomList.Count);

        //PunController.Inst.CreateOrJoinRoom(m_gameType, m_gameTier);
        roomMgr.CreateOrJoinRoom(m_gameType, m_gameTier);
    }

    public void Log(string logMessage, enumLogLevel level = enumLogLevel.initLog)
    {
        debugMgr.Log(logMessage, level);
    }
}