using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class FortunePlayMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortunePlayMgr Inst;
    public List<FortuneUserSeat> m_playerList;
    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void OnUserSit()
    {
        var seatList = PlayerManagement.Inst.getSeatList();
        //if (seatList.Count > 2) return;

        SetAllPlayersStatus((int)FortunePlayerStatus.canStart);

        DistributeCards();

    }

    private void DistributeCards()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        List<List<Card>> cardList = generateRandomCards();
        var seatList = PlayerManagement.Inst.getSeatList();
        foreach (var seat in seatList.Where(x => x.status == (int)FortunePlayerStatus.canStart))
        {
            string cardString = "";
            cardString = string.Join(",", cardList[seatList.IndexOf(seat)]);
            Hashtable props = new Hashtable{
                {Common.FORTUNE_MESSAGE, FortuneMessages.OnCardDistributed},
                {Common.PLAYER_ID, seat.actorNumber},
                {Common.CARD_LIST_STRING, string.Join(",", cardString)}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private List<List<Card>> generateRandomCards()
    {
        List<List<Card>> res = new List<List<Card>>();

        for (int i = 0; i < 4; i++)
            res.Add(new List<Card>());

        List<Card> totalCard = new List<Card>();
        for (int i = 0; i < 4 * 13; i++)
        {
            int num = Random.Range(1, 13);
            int col = Random.Range(0, 3);
            while (totalCard.Count(x => x.num == num && x.color == col) == 0)
            {
                num = Random.Range(1, 13);
                col = Random.Range(0, 3);
            }

            totalCard.Add(new Card(num, col));
        }

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                res[j].Add(totalCard[i * 4 + j]);
            }
        }

        return res;
    }

    private void SetAllPlayersStatus(int status)
    {
        var seatList = PlayerManagement.Inst.getSeatList();
        foreach (var seat in seatList)
        {
            seat.status = status;
        }
        var seatString = string.Join(",", seatList);
        Hashtable props = new Hashtable{
            {Common.FORTUNE_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
