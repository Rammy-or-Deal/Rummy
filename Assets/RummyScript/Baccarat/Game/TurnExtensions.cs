using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class TurnExtensions
    {
        /// <summary>
        /// currently ongoing turn number
        /// </summary>
        public static readonly string TurnPropKey = "Turn";

        /// <summary>
        /// start (server) time for currently ongoing turn (used to calculate end)
        /// </summary>
        public static readonly string TurnStartPropKey = "TStart";

        /// <summary>
        /// Finished Turn of Actor (followed by number)
        /// </summary>
        public static readonly string FinishedTurnPropKey = "FToA";

        /// <summary>
        /// Sets the turn.
        /// </summary>
        /// <param name="room">Room reference</param>
        /// <param name="turn">Turn index</param>
        /// <param name="setStartTime">If set to <c>true</c> set start time.</param>
        public static void SetTurn(Room room, int turn, bool setStartTime = false)
        {
            if (room == null || room.CustomProperties == null)
            {
                return;
            }

            Hashtable turnProps = new Hashtable();
            turnProps[TurnPropKey] = turn;
            if (setStartTime)
            {
                turnProps[TurnStartPropKey] = PhotonNetwork.ServerTimestamp;
            }

            room.SetCustomProperties(turnProps);
        }

        /// <summary>
        /// Gets the current turn from a RoomInfo
        /// </summary>
        /// <returns>The turn index </returns>
        /// <param name="room">RoomInfo reference</param>
        public static int GetTurn(RoomInfo room)
        {
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
            {
                return 0;
            }

            return (int) room.CustomProperties[TurnPropKey];
        }


        /// <summary>
        /// Returns the start time when the turn began. This can be used to calculate how long it's going on.
        /// </summary>
        /// <returns>The turn start.</returns>
        /// <param name="room">Room.</param>
        public static int GetTurnStart(RoomInfo room)
        {
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnStartPropKey))
            {
                return 0;
            }

            return (int) room.CustomProperties[TurnStartPropKey];
        }

        /// <summary>
        /// gets the player's finished turn (from the ROOM properties)
        /// </summary>
        /// <returns>The finished turn index</returns>
        /// <param name="player">Player reference</param>
        public static int GetFinishedTurn (Player player)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
            {
                return 0;
            }

            string propKey = FinishedTurnPropKey + player.ActorNumber;
            return (int) room.CustomProperties[propKey];
        }

        /// <summary>
        /// Sets the player's finished turn (in the ROOM properties)
        /// </summary>
        /// <param name="player">Player Reference</param>
        /// <param name="turn">Turn Index</param>
        public static void SetFinishedTurn( Player player, int turn)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room == null || room.CustomProperties == null)
            {
                return;
            }

            string propKey = FinishedTurnPropKey + player.ActorNumber;
            Hashtable finishedTurnProp = new Hashtable();
            finishedTurnProp[propKey] = turn;

            room.SetCustomProperties(finishedTurnProp);
        }
    }