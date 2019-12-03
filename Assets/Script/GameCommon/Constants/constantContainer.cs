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
}

public static class PhotonFields{
    public static string RoomInfo = "RoomInfo";

}
