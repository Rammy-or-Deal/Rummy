
#region  enum variables for GameMgr
public enum enumGameType{
    Lami, Baccarat, Fortune13
}

public enum enumGameStatus{
    InGameLevelSelect,
    InGamePlay,
}

public enum enumGameTier{
    LamiNewbie, LamiBeginner, LamiVeteran, LamiIntermediate, LamiAdvanced, LamiMaster,
    FortuneNewbie,
    FortuneBeginner,
    FortuneVeteran,
    FortuneIntermediate,
    FortuneAdvanced,
    FortuneMaster,
}
#endregion

#region enum variables for Debug
public enum enumLogLevel{
    initLog,
    RoomManagementLog
}
#endregion

#region enum variables for Global variable
public enum enumBuildMethod{
    Product, Product_Debug, 
    Development, Development_Debug,
}
#endregion