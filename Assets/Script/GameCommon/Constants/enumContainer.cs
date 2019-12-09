
#region  enum variables for GameMgr
public enum enumGameType
{
    Lobby, Lami, Baccarat, Fortune13
}
public enum enumGameStatus
{
    InLobby,
    InGameLevelSelect,
    InGamePlay,
    OnGameStarted,
}

public enum enumPlayerStatus
{
    Init,
    #region  Enum_Rummy
    Rummy_Ready,
    Rummy_GiveUp,
    Rummy_Burnt,
    Rummy_Init,
    Rummy_ReadyToStart,
    #endregion Enum_Rummy
}

public enum enumGameTier
{
    Lobby,
    LamiNewbie, LamiBeginner, LamiVeteran, LamiIntermediate, LamiAdvanced, LamiMaster,
    FortuneNewbie, FortuneBeginner, FortuneVeteran, FortuneIntermediate, FortuneAdvanced, FortuneMaster,
}
#endregion

#region enum variables for Debug
public enum enumLogLevel
{
    initLog,
    MeLog,
    RoomLog,
    staticClassLog,
    BotLog,
    ControllerMessage,
    RummySeatMgrLog,
    RummyCardMgrLog
}
#endregion

#region enum variables for Global variable
public enum enumBuildMethod
{
    Product, Product_Debug,
    Development, Development_Debug,
}
#endregion

#region enum variables for Game Messages
public enum enumGameMessage
{
    #region Room Management Messages
    OnJoinSuccess,
    OnUserEnteredRoom_onlyMaster,
    OnSeatStringUpdate,
    OnPlayerStatusChanged_Rummy,
    
    #endregion

    #region Bot Management Messsages
    #endregion

    #region Rummy Game Messages
    OnGameStarted_Rummy,
    Rummy_OnCardDistributed,
    Rummy_OnUserReadyToStart_M,
    Rummy_OnDealCard,
    Rummy_OnUserTurnChanged,
    Rummy_OnPlayerStatusChanged,
    Rummy_OnGameFinished,
    Rummy_OnAutoPlayer,
    Rummy_OffAutoPlayer,
    Rummy_OnShuffleRequest,
    Rummy_OnShuffleAccept,
    Rummy_OnGameRestart,
    Rummy_OnStartGame,
    Rummy_OnUserReady,
    Rummy_OnGameStarted,
    Rummy_OnGameFinished_Game,
    #endregion

    #region Baccarat Game Messages
    #endregion

    #region Fortune Game Messages
    #endregion
}
#endregion