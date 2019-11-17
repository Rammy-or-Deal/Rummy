using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class IPunTurnManagerCallbacks
{
    /// <summary>
    /// Called the turn begins event.
    /// </summary>
    /// <param name="turn">Turn Index</param>
    public void OnTurnBegins(int turn)
    {
        
    }

    /// <summary>
    /// Called when a turn is completed (finished by all players)
    /// </summary>
    /// <param name="turn">Turn Index</param>
    public void OnTurnCompleted(int turn)
    {
        
    }

    /// <summary>
    /// Called when a player moved (but did not finish the turn)
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="turn">Turn Index</param>
    /// <param name="move">Move Object data</param>
    public void OnPlayerMove(Player player, int turn, object move)
    {
        
    }

    /// <summary>
    /// When a player finishes a turn (includes the action/move of that player)
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="turn">Turn index</param>
    /// <param name="move">Move Object data</param>
    public void OnPlayerFinished(Player player, int turn, object move)
    {
        
    }

    /// <summary>
    /// Called when a turn completes due to a time constraint (timeout for a turn)
    /// </summary>
    /// <param name="turn">Turn index</param>
    public void OnTurnTimeEnds(int turn)
    {
        
    }
}
