public enum FortunePlayerStatus{
    Init = 0,
    Ready = 1,
    canStart = 2,
}

public enum Game_Identifier{
    Lami = 10,
    Baccarat = 20,
    Fortune14 = 30,
}
public enum RoomManagementMessages{
    OnJoinSuccess = 1000,
    OnUserEnteredRoom_M = 1001,
    OnUserEnteredRoom = 1002,
    OnUserLeave = 1003,
    OnBotRemoved = 1004,
    OnRoomSeatUpdate = 1005,
    OnUserSit = 1006,
}
public enum BaccaratShowingCard_NowTurn
{
    Player1 = 0, 
    Banker1 = 1, 
    Player2 = 2, 
    Banker2 = 3, 
    Player3 = 4, 
    Banker3 = 5
}

public enum BaccaratMessages
{
    OnJoinSuccess,
    OnUserEnteredRoom,
    OnUserLeave,
    OnStartNewPan,
    OnPanTimeUpdate,
    OnEndPan,
    OnPlayerBet,
    OnCatchedCardDistributed,
    OnShowingCatchedCard,
    OnShowingVictoryArea,
    OnPrizeAwarded,
    OnUpdateMe,
    OnInitUI
}
public enum BaccaratPlayerType
{
    Player,
    Banker,
}
public enum BaccaratRoomType
{
    Regular, Silver, Gold, Platinum    
}
public enum BuildMethod
{
    Product,
    Develop_Message,
    Develop_No_Message
}

public static class Constants
{
    public static BuildMethod LamiBuildMethod = BuildMethod.Develop_Message;
    public static int FinishDiabledFlag = 1;
    public static float turnTime_AutoPlay = 3.0f;
    public static float turnTime_Develop = 1000;
    public static float turnTime_Product = 30;
    public static float waitTime_Develop = 100;
    public static float waitTime_Product = 30;

    public static int botWaitTime = 2;
    public static int botWaitTimeforBuild = 2;

    #region  constants for Baccarat
    public static int BaccaratCurrentTime = 7;

    public static int BaccaratCardUnitNumber = 4;
    public static int BaccaratScoreLimit = 5;
    public static int BaccaratHighScore = 8;
    public const int BaccaratPlayerArea = 0;
    public const int BaccaratPlayerArea_prize = 2;
    public const int BaccaratBankerArea = 1;
    public const int BaccaratBankerArea_prize = 2;
    public const int BaccaratDrawArea = 4;
    public const int BaccaratDrawArea_prize = 8;
    public const int BaccaratPPArea = 2;
    public const int BaccaratPPArea_prize = 11;
    public const int BaccaratBPArea = 3;
    public const int BaccaratBPArea_prize = 11;
    

    public static float BaccaratShowingCard_waitTime = 1;

    public static float BaccaratDistributionTime = 2.0f;
    #endregion
}
