using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

enum LamiPlayerStatus
{
    Init, Ready, GiveUp, Thinking, Burnt, Game
}
enum LamiMessages
{
    OnJoinSuccess,          // When a user successfully joined the room
    OnUserEnteredRoom_M,    // When a user join the room
    OnRoomSeatUpdate,       // When the seat is updated    
    OnUserOutRoom_M,        // When a user get out the room    
    OnUserDealCard_M,       // When a user deal card, the master should set the next player.
    OnDealCard,             // When a user deal card
    OnUserFinished,         // When a user can't continue to play,        -means: Burnt, GiveUp
    OnUserGamed,            // When a user dealt all cards
    OnUserTurnChanged,      // When the next player is set
    OnGameFinished,         // When all players are unable to play
    OnUserReady,            // When a user clicks "Ready" button
    OnUserReady_BOT,            // When a user clicks "Ready" button -------BOT
    OnBotInfoChanged,       // When the bot is changed
    OnRemovedBot,          // When the bot is removed
    OnUserLeave_M,           // When the user left the room
    OnStartGame,           // When this game can start
    OnCardDistributed,     // When the card is distributed
}
public class LamiMgr : MonoBehaviour
{
    public static LamiMgr Inst;

    private void Awake()
    {
        Debug.Log("LamiMgr Created.");

        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");
        if (!Inst)
            Inst = this;
    }
    private void Start() {
        LogMgr.Inst.Log("LamiMgr is called", (int)LogLevels.RoomLog1);
        SendMessage((int)LamiMessages.OnJoinSuccess);
    }
    public void SendMessage(int messageId, Player p = null)
    {
        LamiLogicMgr.Inst.OnMessageArrived(messageId, p);
    }
}
