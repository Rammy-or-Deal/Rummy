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
    Init, Ready, GiveUp, Thinking, Burnt, Game,
    ReadyToStart
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
    OnUserReadyToStart_M,     // When the user received all cards and showed
    OnPlayerStatusChanged,
    OnAutoPlayer,
    OnShuffleRequest,
    OnShuffleAccept,
    OnGameRestart,
    OffAutoPlayer,
}
public class LamiMgr : GameController
{   
    public override void SendMessage(int messageId, Player p = null)
    {
        LamiLogicMgr.Inst.OnMessageArrived(messageId, p);
    }
}
