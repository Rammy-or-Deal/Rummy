using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class constantContainer
{
    #region constants for game
    public static enumBuildMethod buildMethod = enumBuildMethod.Development_Debug;
    #endregion

    #region name of Scenes
    public static string strLobby = "2_Lobby";
    public static string strScene2Bacccarat = "2_Baccarat";
    public static string strScene2Lami = "2_Lami";
    public static string strScene2Fortune = "2_Fortune13";
    internal static string strScene3Lami = "3_PlayLami";
    internal static string strScene3Bacccarat = "3_PlayBaccarat";
    internal static string strScene3Fortune = "3_PlayFortune13";
    
    #endregion

    #region Contants
    public static int BotCreateTime_min = 5;
    public static int BotCreateTime_max = 10;
    public static string[] skillLevelList = new string[ ]{"Novice", "Expert", "Hero", "Elite", "King", "Master"} ;

    #region  Constants_for_Rummy
    public static float Rummy_BotReadyTime_min = 3;
    public static float Rummy_BotReadyTime_max = 7;
    public static int Rummy_MaxJokerCount = 8;
    #endregion Constants_for_Rummy

    #region  Constants_for_Baccarat
    public static int BaccaratDefaultRoomCount = 4;
    internal static string defaultRoomPrefix = "dTable_";

    #endregion Constants_for_Baccarat

    #endregion Constants
}

public static class PhotonFields{
    
    public static string GAME_MESSAGE = "Game_Message";

    #region string for room management
    public static string RoomInfo = "ROOM_INFO";
    public static string SEAT_STRING = "SEAT_STRING";
    public static object PLAYER_LIST_STRING = "PLAYER_LIST_STRING";

    public static object NEW_PLAYER_INFO = "NEW_PLAYER_INFO";
    
    #endregion

    #region  string for game management
    public static object CARD_LIST_STRING = "CARD_LIST_STRING";
    #endregion
}
