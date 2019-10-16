using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


enum LamiPlayerStatus{
    Init, Ready, GiveUp, Thinking, Burnt, Game
}
enum LamiMessages{
    OnJoinSuccess=10,          // When a user successfully joined the room
    OnUserEnteredRoom_M=11,    // When a user join the room
    OnRoomSeatUpdate=12,       // When the seat is updated    
    OnUserOutRoom_M=13,        // When a user get out the room    
    OnUserDealCard_M=14,       // When a user deal card, the master should set the next player.
    OnDealCard=15,             // When a user deal card
    OnUserFinished=16,         // When a user can't continue to play,        -means: Burnt, GiveUp
    OnUserGamed=17,            // When a user dealt all cards
    OnUserTurnChanged=18,      // When the next player is set
    OnGameFinished=19,         // When all players are unable to play
    OnUserReady=20,            // When a user clicks "Ready" button
    OnBotInfoChanged=21,       // When the bot is changed
    OnRemovedBot = 22,
}

namespace Assets.RummyScript.LamiGame
{
    public class LamiMgr
    {
        public LamiPlayerMgr playerMgr;
        public LamiLogicMgr logicMgr;
        public LamiPanMgr panMgr;
        public LamiMgr()
        {
            playerMgr = new LamiPlayerMgr(this);
            logicMgr = new LamiLogicMgr(this);
            panMgr = new LamiPanMgr(this);
        }
    }
}