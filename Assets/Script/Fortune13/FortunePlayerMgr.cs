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
    None,
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

        FortuneUIController.Inst.calcDlg.StopAllCoroutines();
        FortuneUIController.Inst.resultDlg.StopAllCoroutines();
        FortuneUIController.Inst.changeDlg.StopAllCoroutines();
        StopAllCoroutines();

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
        if (GameMgr.Inst.m_gameStatus != enumGameStatus.InGamePlay) return;
        if (!PhotonNetwork.IsMasterClient) return;
        GameMgr.Inst.m_gameStatus = enumGameStatus.OnGameStarted;


        List<List<Card>> cardList = generateRandomCards();
        //var seatList = PlayerManagement.Inst.getSeatList();

        var seatList = GameMgr.Inst.seatMgr.m_playerList;
        string missionString = new FortuneMissionCard().CreateMissionString();

        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();

        foreach (var player in pList.m_playerList)
        {
            player.m_status = enumPlayerStatus.Fortune_ReceivedCard;

            string cardString = "";
            cardString = string.Join(",", cardList[pList.m_playerList.IndexOf(player)].Select(x => x.cardString));

            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnCardDistributed},
                {Common.PLAYER_ID, player.m_actorNumber},
                {Common.CARD_LIST_STRING, string.Join(",", cardString)},
                {Common.FORTUNE_MISSION_CARD, missionString},
                {Common.FORTUNE_REMAIN_TIME, constantContainer.FortuneChangingTime+1},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

    }

    internal void OnLucky(Player p)
    {
        var pList = new PlayerInfoContainer();
        pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        if (pList.m_playerList.Count(x => x.m_status == enumPlayerStatus.Fortune_Lucky) > 0) return;

        StopAllCoroutines();
        FortuneUIController.Inst.changeDlg.StopAllCoroutines();

        int actorNumber = p.ActorNumber;
        Lucky lucky = (Lucky)p.CustomProperties[Common.LUCKY_NAME];

        GameMgr.Inst.Log("LUCKY:= " + actorNumber + " : " + lucky);

        if (!PhotonNetwork.IsMasterClient) return;
        pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First().m_status = enumPlayerStatus.Fortune_Lucky;

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, -1},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        StartCoroutine(ShowGoldenResult());
    }

    IEnumerator ShowGoldenResult()
    {
        yield return new WaitForSeconds(3);
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnShowLuckResult},
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void ChangePlayerStatus(int actorNumber, enumPlayerStatus status)
    {
        var pList = new PlayerInfoContainer();
        pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];

        try
        {
            var player = pList.m_playerList.Where(x => x.m_actorNumber == actorNumber).First();
            player.m_status = status;
        }
        catch { }
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, -1},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
    Coroutine restartRoutine;
    internal void OnFinishedGame()
    {
        FortunePanMgr.Inst.OnInitCard();
        restartRoutine = StartCoroutine(WaitForRestart(Constants.fortuneWaitTimeForRestart));
    }

    IEnumerator WaitForRestart(float fortuneWaitTimeForRestart)
    {
        yield return new WaitForSeconds(fortuneWaitTimeForRestart);
        if (PhotonNetwork.IsMasterClient)
        {
            GameMgr.Inst.m_gameStatus = enumGameStatus.InGamePlay;
            SetAllPlayersStatus((int)FortunePlayerStatus.Init);
            DistributeCards();
        }
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
    internal void OnPlayerDealCard(enumPlayerStatus playerStatus)
    {
        //var seatList = PlayerManagement.Inst.getSeatList();
        int actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        // Store Usercards

        UpdatePlayerCardList();

        if (!PhotonNetwork.IsMasterClient) return;
        ChangePlayerStatus(actorNumber, playerStatus);
        //if (seatList.Count(x => x.status == (int)FortunePlayerStatus.OnChanging) > 0) return;
        if (CheckAllPlayerDealCard())
        {
            StopCoroutine(m_checkingTimer);
            //CheckBadArrangedPlayer();
            SendPlayersCardToAll(2);
        }
    }

    private void CheckBadArrangedPlayer()
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        bool isBadFound = false;
        foreach (var player in userCardList)
        {
            var backType = FortuneRuleMgr.GetCardType(player.backCard, ref player.backCard);
            var middleType = FortuneRuleMgr.GetCardType(player.middleCard, ref player.middleCard);
            var frontType = FortuneRuleMgr.GetCardType(player.frontCard, ref player.frontCard);

            var backScore = FortuneRuleMgr.GetScore(player.backCard, backType);
            var middleScore = FortuneRuleMgr.GetScore(player.middleCard, middleType);
            var frontScore = FortuneRuleMgr.GetScore(player.frontCard, frontType);

            if (backScore < middleScore || middleScore < frontScore)
            {
                pList.m_playerList.Where(x => x.m_actorNumber == player.actorNumber).First().m_status = enumPlayerStatus.Fortune_BadArranged;
                isBadFound = true;
            }
        }
        if (isBadFound)
        {
            Hashtable props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, -1},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private bool CheckAllPlayerDealCard()
    {
        bool res = true;
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();

        if (pList.m_playerList.Count(x => x.m_status == enumPlayerStatus.Fortune_OnChanging) > 0)
            res = false;

        return res;
    }

    private void UpdatePlayerCardList()
    {
        FortuneUserCardList newUser = new FortuneUserCardList();
        newUser.actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];
        newUser.frontCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_FRONT_CARD]);
        newUser.middleCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_MIDDLE_CARD]);
        newUser.backCard = FortuneUserCardList.stringToCardList((string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_PLAYER_BACK_CARD]);

        try
        {
            foreach (var p in userCardList.Where(x => x.actorNumber == newUser.actorNumber))
            {
                userCardList.Remove(p);
            }
        }
        catch { }
        userCardList.Add(newUser);
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

        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        int actorNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.PLAYER_ID];

        foreach (var player in pList.m_playerList.Where(x => x.m_actorNumber == actorNumber))
        {
            player.m_status = enumPlayerStatus.Fortune_Ready;
        }

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, -1},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (pList.m_playerList.Count(x => x.m_status == enumPlayerStatus.Fortune_ReceivedCard) == 0)
        {
            foreach (var player in pList.m_playerList.Where(x => x.m_status == enumPlayerStatus.Fortune_Ready))
            {
                player.m_status = enumPlayerStatus.Fortune_OnChanging;
            }

            props = new Hashtable{
                {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_OnGameStarted},
                {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        }
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
        if (remainTime >= 0)
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
        try
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        catch { }
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

    private void SetAllPlayersStatus(enumPlayerStatus status)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        foreach (var player in pList.m_playerList)
        {
            player.m_status = status;
        }

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.OnSeatStringUpdate},
            {PhotonFields.PLAYER_LIST_STRING, pList.m_playerInfoListString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

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
    private int[] bonusList = new int[4] { 2, 3, 4, 6 };
    public string CreateMissionString()
    {
        missionNo = Random.Range(0, Enum.GetNames(typeof(HandSuit)).Length - 1);
        missionLine = Random.Range(0, 3);
        missionPrice = bonusList[Random.Range(0, bonusList.Length)];
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
