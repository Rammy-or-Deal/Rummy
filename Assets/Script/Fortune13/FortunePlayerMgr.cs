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
    Royal_Flush = 9,
    Straight_Flush = 8,
    Four_Of_A_Kind = 7,
    Full_House = 6,
    Flush = 5,
    Straight = 4,
    Triple = 3,
    Two_Pair = 2,
    Pair = 1,
    High_Card = 0,
    Error = -1,
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

public class FortunePlayerMgr : SeatMgr
{
    // Start is called before the first frame update
    public static FortunePlayerMgr Inst;

    [HideInInspector] bool isFirst;
    void Start()
    {

        if (!Inst)
        {
            Inst = this;
        }
        GameMgr.Inst.seatMgr = this;
        isFirst = true;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void StartFortuneGame()
    {
        base.StartFortuneGame();

        DistributeCards();
    }

    internal void OnUserSit()
    {
        /*
        if (!isFirst) return;

        var seatList = PlayerManagement.Inst.getSeatList();
        if (seatList.Count < 2) return;
        isFirst = false;

        if (!PhotonNetwork.IsMasterClient) return;
        SetAllPlayersStatus((int)FortunePlayerStatus.canStart);
        DistributeCards();
        */
    }


    private void DistributeCards()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        List<List<Card>> cardList = generateRandomCards();
        //var seatList = PlayerManagement.Inst.getSeatList();

        var seatList = GameMgr.Inst.seatMgr.m_playerList;
        string missionString = new FortuneMissionCard().CreateMissionString();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (seatList.Count(x => x.isSeat == true && x.m_playerInfo.m_actorNumber == player.ActorNumber) == 0) continue;
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, -1},
                {Common.PLAYER_STATUS, (int)enumPlayerStatus.Fortune_canStart}
            };
            player.SetCustomProperties(props);
        }

        foreach (var player in seatList)
        {
            string cardString = "";
            cardString = string.Join(",", cardList[seatList.IndexOf(player)].Select(x => x.cardString));
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnCardDistributed},
                {Common.PLAYER_ID, player.m_playerInfo.m_actorNumber},
                {Common.CARD_LIST_STRING, string.Join(",", cardString)},
                {Common.FORTUNE_MISSION_CARD, missionString},
                {Common.FORTUNE_REMAIN_TIME, constantContainer.FortuneChangingTime+1},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    internal void OnFinishedGame()
    {
        StartCoroutine(WaitForRestart(Constants.fortuneWaitTimeForRestart));
    }

    IEnumerator WaitForRestart(float fortuneWaitTimeForRestart)
    {
        yield return new WaitForSeconds(fortuneWaitTimeForRestart);
        SetAllPlayersStatus((int)FortunePlayerStatus.canStart);
        DistributeCards();
    }

    internal void OnOpenCard()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int lineNo = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_OPEN_CARD_LINE];
        lineNo--;
        if (lineNo < 0)
        {
            CalcResult();
        }
        else
        {
            StartCoroutine(SendPlayersCardToAll_Event(lineNo));
        }
    }

    IEnumerator SendPlayersCardToAll_Event(int lineNo)
    {
        yield return new WaitForSeconds(Constants.FortuneWaitTimeForCheckingCard);
        SendPlayersCardToAll(lineNo);
    }

    private void CalcResult()
    {
        LogMgr.Inst.Log("End");
    }

    public List<FortuneUserCardList> userCardList = new List<FortuneUserCardList>();
    internal void OnPlayerDealCard()
    {
        //var seatList = PlayerManagement.Inst.getSeatList();

        // Store Usercards
        FortuneUserCardList newUser = new FortuneUserCardList();
        newUser.actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        newUser.frontCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_FRONT_CARD]);
        newUser.middleCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_MIDDLE_CARD]);
        newUser.backCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_BACK_CARD]);

        if (userCardList.Count(x => x.actorNumber == newUser.actorNumber) == 0)
        {
            userCardList.Add(newUser);
        }

        if (!PhotonNetwork.IsMasterClient) return;
        //if (seatList.Count(x => x.status == (int)FortunePlayerStatus.OnChanging) > 0) return;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            try
            {
                LogMgr.Inst.Log("Player(" + player.ActorNumber + ")'s status=" + ((FortunePlayerStatus)((int)player.CustomProperties[Common.PLAYER_STATUS])));
                if ((int)player.CustomProperties[Common.PLAYER_STATUS] == (int)FortunePlayerStatus.OnChanging)
                    return;
            }
            catch { continue; }
        }

        // If all users dealt card.
        SendPlayersCardToAll(2);
    }

    private void SendPlayersCardToAll(int lineNo)
    {
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnOpenCard},
            {Common.FORTUNE_OPEN_CARD_LINE, lineNo}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    internal void OnUserReady()
    {
        //var seatList = PlayerManagement.Inst.getSeatList();
        //if (PhotonNetwork.PlayerList.Count(x => x.status == (int)FortunePlayerStatus.canStart) > 0) return;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            try
            {
                var status = (int)player.CustomProperties[Common.PLAYER_STATUS];
                if (status == (int)FortunePlayerStatus.canStart) return;
            }
            catch { continue; }
        }

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnGameStarted},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        OnTickTimer();
    }
    Coroutine m_checkingTimer;
    public void OnTickTimer()
    {
        try
        {
            StopCoroutine(m_checkingTimer);
        }
        catch { }

        var remainTime = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_REMAIN_TIME];
        remainTime--;
        if (remainTime > 0)
            m_checkingTimer = StartCoroutine(TickTime(remainTime));
        else
        {
            SendGameOverMessage();
        }
    }

    private void SendGameOverMessage()
    {
        
    }

    IEnumerator TickTime(int remainTime)
    {
        yield return new WaitForSeconds(1);
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnTickTimer},
            {Common.FORTUNE_REMAIN_TIME, remainTime}
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
        /*
        var seatList = PlayerManagement.Inst.getSeatList();
        foreach (var seat in seatList)
        {
            seat.status = status;
        }
        var seatString = string.Join(",", seatList.Select(x => x.seatString));
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        props = new Hashtable{
            {Common.PLAYER_STATUS, status}
        };
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetCustomProperties(props);
        }
        */
    }
}
public class FortuneUserCardList
{
    public int actorNumber;
    public List<Card> frontCard;
    public List<Card> middleCard;
    public List<Card> backCard;

    public static List<Card> stringToCardList(string cardString)
    {
        List<Card> cardList = new List<Card>();
        var strArray = cardString.Split(',');
        foreach (var str in strArray)
        {
            Card card = new Card();
            card.cardString = str;
            cardList.Add(card);
        }
        return cardList;
    }
    public static string cardlistTostring(List<Card> cardList)
    {
        return string.Join(",", cardList.Select(x => x.cardString));
    }
    public FortuneUserCardList()
    {
        frontCard = new List<Card>();
        middleCard = new List<Card>();
        backCard = new List<Card>();
    }

}
public class FortuneMissionCard
{
    public int missionNo;
    public int missionLine;
    public int missionPrice;
    public string CreateMissionString()
    {
        missionNo = Random.Range(0, Enum.GetNames(typeof(HandSuit)).Length - 1);
        missionLine = Random.Range(0, 3);
        missionPrice = Random.Range(2, 4);
        if (missionLine == 0)
        {
            while ((missionNo == (int)HandSuit.Two_Pair) || (missionNo > (int)HandSuit.Triple))
            {
                missionNo = Random.Range(0, Enum.GetNames(typeof(HandSuit)).Length - 1);
            }

        }
        return missionString;
    }
    public string missionString
    {
        get
        {
            return missionNo + ":" + missionLine + ":" + missionPrice;
        }
        set
        {
            var tmp = value.Split(':').Select(Int32.Parse).ToArray();
            missionNo = tmp[0];
            missionLine = tmp[1];
            missionPrice = tmp[2];
        }
    }
}
