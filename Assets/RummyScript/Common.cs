using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{

    public static string LAMI_MESSAGE = "LAMI_MESSAGE";
    public static string PLAYER_ID = "PLAYER_ID";
    public static string SEAT_STRING = "SEAT_STRING";
    public const string PLAYER_STATUS = "PLAYER_STATUS";
    public static string PLAYER_INFO = "PLAYER_COMMON_INFO";
    public static string NEW_PLAYER_INFO = "NEW_PLAYER_COMMON_INFO";
    public static string NEW_PLAYER_STATUS = "NEW_PLAYER_COMMON_STATUS";



    public static string PLAYER_NAME = "PLAYER_NAME";
    public static string PLAYER_PIC = "PLAYER_PICTURE";
    public static string PLAYER_COIN = "PLAYER_COIN";

    public static string REMOVED_BOT_ID = "REMOVED_BOT_ID";
    public static string BOT_LIST_STRING = "BotListString";
    public static string BOT_ID = "Bot_ID";
    public static string BOT_STATUS = "BOT_STATUS";
    /********************************************* */


    public static string CARD_LIST_STRING = "CARD_LIST_STRING";

    public static string eventID_room = "EventID_ROOM";
    //10, player
    // 11: Player Joined
    //20, bot
    // 21: Bot Added
    // 22: Bot List String changed
    // 23: Bot Remove


    public static string eventID_player = "EventID_PLAYER";
    // 10, Click Ready Button

    public static string IS_BOT = "IsBot";

    //player first card
    public static string PLAYER_CARD = "PlayerCard";
    public static string PLAYER_CARD_List = "InitPlayerCardList";
    //player info 

    public static string PLAYER_LEAF = "PlayerCoin";
    public static string PLAYER_LEVEL = "PlayerLevel";
    //player deal card
    public static string GAME_CARD = "GameCard";
    public static string GAME_CARD_PAN = "GameCardPan";
    public static string GAME_CARD_PAN_POS = "GameCardPos";
    // Room Seat
    public static string SEAT_ID = "SeatID";
    public static string Game_START = "IsGameStarted";
    //player my card
    public static string REMAIN_CARD_COUNT = "MyCardCount";
    //player trun
    public static string PLAYER_TURN = "IsTurn";

}
