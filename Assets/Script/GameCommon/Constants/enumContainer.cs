
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
}

public enum enumPlayerStatus
{
    Init,
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
    RoomManagementLog,
    MeLog,
    RoomLog
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
    OnNewUserEnteredRoom,

    #endregion

    #region Bot Management Messsages
    #endregion

    #region Rummy Game Messages
    #endregion

    #region Baccarat Game Messages
    #endregion

    #region Fortune Game Messages
    #endregion
}
#endregion