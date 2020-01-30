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
    [HideInInspector]public MeMgr meMgr;
    [HideInInspector]public SeatMgr seatMgr;
    [HideInInspector]public MessageMgr messageMgr;
    [HideInInspector]public BotMgr botMgr;    
    [HideInInspector]public ResultMgr resultMgr;
    
    #endregion

    #region Game Status Variables
    private enumGameStatus _m_gamestatus;
    public enumGameStatus m_gameStatus{
        get{return _m_gamestatus;}
        set{
            _m_gamestatus = value;
        }
    }
    public enumGameType m_gameType;
    public enumGameTier m_gameTier;
    public enumPlayerStatus m_playerStatus;
    #endregion

    #region UnityEngine
    private void Awake() {
        if(!Inst)
            Inst = this;
    }

    #endregion
    internal void LoadGameScene2(enumGameType v)
    {
        Log("Load 2 Scene");
        sceneMgr.LoadGameScene2(v);

        m_gameType = v;
        m_gameStatus = enumGameStatus.InGameLevelSelect;       
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
    public void InitStatus()
    {
        m_gameType = enumGameType.Lobby;
        m_gameTier = enumGameTier.Lobby;
        m_gameStatus = enumGameStatus.InLobby;
        m_playerStatus = enumPlayerStatus.Init;
    }
    public void Log(string logMessage, LogLevel level = LogLevel.initLog)
    {
        debugMgr.Log(logMessage, level);
    }
}