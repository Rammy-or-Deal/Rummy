using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RummySeatMgr : SeatMgr
{
    public string totalCardString { get; private set; }

    private string totalRemainString;
    private string totalPayString;

    // Start is called before the first frame update
    private void Start()
    {
        GameMgr.Inst.seatMgr = this;
    }

    // Update is called once per frame

    public override void OnSeatStringUpdate()
    {
        base.OnSeatStringUpdate();

        if (!PhotonNetwork.IsMasterClient) return;

        GameMgr.Inst.Log("Check if all players are ready.", LogLevel.RummySeatMgrLog);
        // Update User Seat

        var userListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        PlayerInfoContainer pList = new PlayerInfoContainer(userListString);

        GameMgr.Inst.Log("userListString = " + userListString + ",  seatNumList=" + string.Join(",", seatNumList), LogLevel.RummySeatMgrLog);

        bool isAllReady = true;
        foreach (var seat in seatNumList)
        {

            var user = pList.m_playerList.Where(x => x.m_actorNumber == seat.Key).First();
            if (user.m_status != enumPlayerStatus.Rummy_Ready)
            {
                GameMgr.Inst.Log(user.m_userName + " isn't ready.", LogLevel.RummySeatMgrLog);
                isAllReady = false;
                break;
            }
            else
            {
                GameMgr.Inst.Log(user.m_userName + " is ready.", LogLevel.RummySeatMgrLog);
            }
        }
        Debug.Log(seatNumList.Count + " = " + GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer + "  / " + isAllReady);

        if (isAllReady && seatNumList.Count == GameMgr.Inst.roomMgr.m_currentRoom.m_maxPlayer)
        {
            GameMgr.Inst.Log("All Users are ready.", LogLevel.RoomLog);

            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnGameStarted_Rummy}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    internal void OnGameStarted_Rummy()
    {
        // foreach (var player in m_playerList)
        // {
        //     ((LamiUserSeat)player).OnGameStarted_Rummy();
        // }

        // Send card

        RummyGameMgr.Inst.cardMgr.SendCardsToPlayers();
    }

    internal void OnCardDistributed_Rummy()
    {
        var cardListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.CARD_LIST_STRING];
        totalCardString = cardListString;
        totalRemainString = cardListString;
        totalPayString = "";

        GameMgr.Inst.Log("Card Accepted: " + cardListString, LogLevel.RummySeatMgrLog);

        var tmp = cardListString.Split('/');
        for (int i = 0; i < tmp.Length; i++)
        {
            int tmpActor = int.Parse(tmp[i].Split(':')[0]);
            if (tmpActor == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                RummyGameMgr.Inst.meMgr.ShowMyCards(tmp[i]);
            }
            else
            {
                var user = (LamiUserSeat)m_playerList.Where(x => x.m_playerInfo.m_actorNumber == tmpActor).First();
               // user.SetMyCards(tmp[i]);
            }
        }        
    }
}
