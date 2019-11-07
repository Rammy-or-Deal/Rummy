using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public enum HandSuit
{
    Royal_Flush,
    Straight_Flush,
    Four_Of_A_Kind,
    Full_House,
    Flush,
    Straight,
    Triple,
    Two_Pair,
    Pair,
    High_Card,
}
public enum Lucky
{
    Grand_Dragon,
    Dragon,
    Twelve_Royals,
    Three_Straight_Flushes,
    Three_4_Of_A_Kind,
    All_Small,
    All_Big,
    Same_Colour,
    Four_Triples,
    Five_Pair_Plus_Triple,
    Six_Pairs,
    Three_Straights,
    Three_Flushes,
}

public class FortunePlayMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortunePlayMgr Inst;
    public List<FortuneUserSeat> m_playerList;
    [HideInInspector] bool isFirst;
    void Start()
    {
        if (!Inst)
            Inst = this;
        isFirst = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void OnUserSit()
    {
        if (!isFirst) return;
        isFirst = false;
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
            cardString = string.Join(",", cardList[seatList.IndexOf(seat)].Select(x => x.cardString));
            Hashtable props = new Hashtable{
                {Common.FORTUNE_MESSAGE, FortuneMessages.OnCardDistributed},
                {Common.PLAYER_ID, seat.actorNumber},
                {Common.CARD_LIST_STRING, string.Join(",", cardString)}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    internal void OnUserReady()
    {
        var seatList = PlayerManagement.Inst.getSeatList();
        if (seatList.Count(x => x.status == (int)FortunePlayerStatus.canStart) > 0) return;

        int missionCard = Random.Range(0, Enum.GetNames(typeof(HandSuit)).Length);

        Hashtable props = new Hashtable{
            {Common.FORTUNE_MESSAGE, (int)FortuneMessages.OnGameStarted},
            {Common.FORTUNE_MISSION_CARD, missionCard}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private List<List<Card>> generateRandomCards()
    {
        List<List<Card>> res = new List<List<Card>>();

        for (int i = 0; i < 4; i++)
            res.Add(new List<Card>());

        List<Card> totalCard = new List<Card>();
        for (int i = 0; i < 4 * 13; i++)
        {
            int num = Random.Range(1, 14);
            int col = Random.Range(0, 4);
            while (totalCard.Count(x => x.num == num && x.color == col) != 0)
            {
                num = Random.Range(1, 14);
                col = Random.Range(0, 4);
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
        var seatString = string.Join(",", seatList.Select(x => x.seatString));
        Hashtable props = new Hashtable{
            {Common.FORTUNE_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
