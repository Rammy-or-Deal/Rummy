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
    OnShowingVictoryArea
}
public enum BaccaratPlayerType
{
    Player,
    Banker,
}
public enum BuildMethod
{
    Product,
    Develop_Message,
    Develop_No_Message
}

public static class Constants
{
    public static BuildMethod LamiBuildMethod = BuildMethod.Product;
    public static int FinishDiabledFlag = 1;
    public static float turnTime_AutoPlay = 3.0f;
    public static float turnTime_Develop = 1000;
    public static float turnTime_Product = 30;
    public static float waitTime_Develop = 100;
    public static float waitTime_Product = 30;

    public static int botWaitTime = 2;
    public static int botWaitTimeforBuild = 2;

    #region  constants for Baccarat
    public static int BaccaratCurrentTime = 15;

    public static int BaccaratCardUnitNumber = 4;
    public static int BaccaratScoreLimit = 5;
    public static int BaccaratHighScore = 8;
    public static int BaccaratPlayerArea = 0;
    public static int BaccaratBankerArea = 1;
    public static int BaccaratDrawArea = 4;
    public static int BaccaratPPArea = 2;
    public static int BaccaratBPArea = 3;

    public static float BaccaratShowingCard_waitTime = 5;
    #endregion
}
