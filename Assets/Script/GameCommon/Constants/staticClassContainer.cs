using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;


public class GameRoomInfo
{
    public enumGameType m_gameType { get; set; }
    public enumGameTier m_gameTier { get; set; }
    public string m_roomName { get; set; }
    public int m_gameFee { get; set; }
    public int m_playerCount { get; set; }
    public int m_maxPlayer { get; set; }
    public string m_additionalString { get; set; }

    public GameRoomInfo()
    {

    }

    public string roomInfoString
    {
        get
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", (int)m_gameType, (int)m_gameTier, m_roomName, m_gameFee, m_playerCount, m_maxPlayer, m_additionalString);
        }
        set
        {
            var tmpList = value.Split(':');
            m_gameType = (enumGameType)(int.Parse(tmpList[0]));
            m_gameTier = (enumGameTier)(int.Parse(tmpList[1]));
            m_roomName = tmpList[2];
            m_gameFee = int.Parse(tmpList[3]);
            m_playerCount = int.Parse(tmpList[4]);
            m_maxPlayer = int.Parse(tmpList[5]);


            m_additionalString = "";
            for (int i = 0; i <= 5; i++)
            {
                m_additionalString += tmpList[i] + ":";
            }
            m_additionalString = value.Replace(m_additionalString, "");
            //Debug.Log("Test for parsing. "+value.Replace(m_additionalString, ""));
        }
    }
}

public class PlayerInfoContainer
{
    public List<PlayerInfo> m_playerList;
    public string stringForLog{
        get{
            return string.Join(",", m_playerList.Select(x => x.stringForLog));
        }
        set{}
    }
    public string m_playerInfoListString
    {
        get
        {
            return string.Join(",", m_playerList.Select(x => x.playerInfoString));
        }
        set
        {
            var tmpList = value.Split(',');
            foreach (var tmp in tmpList)
            {
                var player = new PlayerInfo();
                player.playerInfoString = tmp;
                m_playerList.Add(player);
            }
        }
    }
    public PlayerInfoContainer()
    {
        m_playerList = new List<PlayerInfo>();
    }
    public PlayerInfoContainer(string playerInfoListString)
    {
        m_playerList = new List<PlayerInfo>();
        m_playerInfoListString = playerInfoListString;
    }

    public PlayerInfoContainer GetInfoContainerFromPhoton()
    {
        try
        {
            string infoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
            this.m_playerInfoListString = infoString;
        }
        catch
        {
            GameMgr.Inst.Log("No player list info.", enumLogLevel.staticClassLog);
            return null;
        }
        return this;
    }
}
public class PlayerInfo
{

    public int m_actorNumber;
    public string m_userName;
    public string m_userPic;
    public int m_coinValue;
    public string m_skillLevel;
    public int m_frameId;
    public enumPlayerStatus m_status;

    public string stringForLog
    {
        get{
            var infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                m_actorNumber,
                m_userName,
                m_userPic,
                m_coinValue,
                m_skillLevel,
                m_frameId,
                m_status
            );
            return infoString;
        }
        set{}
    }
    public string playerInfoString
    {
        get
        {
            var infoString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}",
                m_actorNumber,
                m_userName,
                m_userPic,
                m_coinValue,
                m_skillLevel,
                m_frameId,
                (int)m_status
            );
            return infoString;
        }
        set
        {
            var tmpList = value.Split(':');
            m_actorNumber = int.Parse(tmpList[0]);
            m_userName = tmpList[1];
            m_userPic = tmpList[2];
            m_coinValue = int.Parse(tmpList[3]);
            m_skillLevel = tmpList[4];
            m_frameId = int.Parse(tmpList[5]);
            m_status = (enumPlayerStatus)(int.Parse(tmpList[6]));
        }
    }

    public string setValues(int actorNumber, enumPlayerStatus status, DataController data)
    {
        m_actorNumber = actorNumber;
        m_userName = data.userInfo.name;
        m_userPic = data.userInfo.pic;
        m_coinValue = data.userInfo.coinValue;
        m_skillLevel = data.userInfo.skillLevel;
        m_frameId = data.userInfo.frameId;
        m_status = status;

        return playerInfoString;
    }
}
public class SeatInfo
{
    public class OneSeat
    {
        public int m_actorNumber;
        public int m_seatNo;
        public string m_userName;
        public string oneSeatString
        {
            get
            {
                return string.Format("{0}:{1}:{2}", m_actorNumber, m_seatNo, m_userName);
            }
            set
            {
                var tmpList = value.Split(':');
                m_actorNumber = int.Parse(tmpList[0]);
                m_seatNo = int.Parse(tmpList[1]);
                m_userName = tmpList[2];
            }
        }
        public OneSeat() { }
        public OneSeat(int actorNumber, int seatNo, string userName)
        {
            m_actorNumber = actorNumber;
            m_seatNo = seatNo;
            m_userName = userName;
        }
    }
    public List<OneSeat> seatList;

    public SeatInfo()
    {
        seatList = new List<OneSeat>();
    }
    public void AddOneSeat(int actorNumber, int seatNo, string userName)
    {
        OneSeat seat = new OneSeat(actorNumber, seatNo, userName);
        seatList.Add(seat);
    }
    public string seatString
    {
        get
        {
            return string.Join(",", seatList.Select(x => x.oneSeatString));
        }
        set
        {
            seatList.Clear();
            var tmpList = value.Split(',');
            foreach (var tmp in tmpList)
            {
                OneSeat seat = new OneSeat();
                seat.oneSeatString = tmp;
                seatList.Add(seat);
            }
        }
    }
}

public class Card
{
    public int num;
    public int color;

    public int MyCardId = -1;
    public int virtual_num;
    public List<Card> children;
    public Card parent = null;
    public byte byteValue
    {
        get
        {
            int tmp = num; if (num == 1) tmp = 14;
            return (byte)(tmp + 16 * color);
        }
        set
        {
            color = (int)(value / 16);
            num = (int)value % 16;
        }
    }
    public string cardString
    {
        get
        {
            return num + ":" + color;
        }
        set
        {
            var tmp = value.Split(':').Select(Int32.Parse).ToArray();
            num = tmp[0];
            color = tmp[1];
        }
    }
    public Card(int num0, int color0)
    {
        num = num0;
        color = color0;
        virtual_num = num;
    }
    public Card()
    {

    }


    #region For only baccarat
    public List<Card> Children_Set(List<Card> m_orgList)
    {
        children = new List<Card>();
        foreach (var card in m_orgList.Where(x => x.MyCardId > MyCardId && x.num == num && x.num != 15).ToList())
        {
            var tmpCard = new Card();
            tmpCard.num = card.num;
            tmpCard.color = card.color;
            tmpCard.MyCardId = card.MyCardId;
            tmpCard.virtual_num = card.virtual_num;

            tmpCard.parent = this;
            children.Add(tmpCard);
        }
        return children;
    }
    public List<Card> Children_Flush(List<Card> m_orgList)
    {
        children = new List<Card>();
        foreach (var card in m_orgList.Where(x => x.MyCardId != MyCardId && x.num == num + 1 && x.color == color && x.num != 15))
        {
            card.parent = this;
            children.Add(card);
        }
        return children;
    }
    #endregion
}

public static class staticFunction_rummy
{
    public static int GetGameBonus(enumGameTier tier)
    {
        int bonus = 0;
        switch (tier)
        {
            case enumGameTier.LamiNewbie:
                bonus = 1200;
                break;
            case enumGameTier.LamiBeginner:
                bonus = 3600;
                break;
            case enumGameTier.LamiVeteran:
                bonus = 12000;
                break;
            case enumGameTier.LamiIntermediate:
                bonus = 30000;
                break;
            case enumGameTier.LamiAdvanced:
                bonus = 60000;
                break;
            case enumGameTier.LamiMaster:
                bonus = 120000;
                break;
        }
        return bonus;
    }

    public static int GetWinningBonus(enumGameTier tier, int rank)
    {
        int bonus = 0;
        switch (tier)
        {
            case enumGameTier.LamiNewbie:
                switch (rank)
                {
                    case 0: bonus = 600; break;
                    case 1: bonus = -100; break;
                    case 2: bonus = -200; break;
                    case 3: bonus = -300; break;
                }
                break;
            case enumGameTier.LamiBeginner:
                switch (rank)
                {
                    case 0: bonus = 1800; break;
                    case 1: bonus = -300; break;
                    case 2: bonus = -600; break;
                    case 3: bonus = -900; break;
                }
                break;
            case enumGameTier.LamiVeteran:
                switch (rank)
                {
                    case 0: bonus = 6000; break;
                    case 1: bonus = -1000; break;
                    case 2: bonus = -2000; break;
                    case 3: bonus = -3000; break;
                }
                break;
            case enumGameTier.LamiIntermediate:
                switch (rank)
                {
                    case 0: bonus = 15000; break;
                    case 1: bonus = -2500; break;
                    case 2: bonus = -5000; break;
                    case 3: bonus = -7500; break;
                }
                break;
            case enumGameTier.LamiAdvanced:
                switch (rank)
                {
                    case 0: bonus = 30000; break;
                    case 1: bonus = -5000; break;
                    case 2: bonus = -10000; break;
                    case 3: bonus = -15000; break;
                }
                break;
            case enumGameTier.LamiMaster:
                switch (rank)
                {
                    case 0: bonus = 60000; break;
                    case 1: bonus = -10000; break;
                    case 2: bonus = -20000; break;
                    case 3: bonus = -30000; break;
                }
                break;
        }
        return bonus;
    }

    internal static int GetAceBonus(enumGameTier tier)
    {
        int bonus = 0;
        switch (tier)
        {
            case enumGameTier.LamiNewbie:
                bonus = 30;
                break;
            case enumGameTier.LamiBeginner:
                bonus = 90;
                break;
            case enumGameTier.LamiVeteran:
                bonus = 300;
                break;
            case enumGameTier.LamiIntermediate:
                bonus = 750;
                break;
            case enumGameTier.LamiAdvanced:
                bonus = 1500;
                break;
            case enumGameTier.LamiMaster:
                bonus = 3000;
                break;
        }
        return bonus;
    }

    internal static int GetJokerBonus(enumGameTier tier)
    {
        int bonus = 0;
        switch (tier)
        {
            case enumGameTier.LamiNewbie:
                bonus = 30;
                break;
            case enumGameTier.LamiBeginner:
                bonus = 90;
                break;
            case enumGameTier.LamiVeteran:
                bonus = 300;
                break;
            case enumGameTier.LamiIntermediate:
                bonus = 750;
                break;
            case enumGameTier.LamiAdvanced:
                bonus = 1500;
                break;
            case enumGameTier.LamiMaster:
                bonus = 3000;
                break;
        }
        return bonus;
    }
}

public static class staticFunction_Baccarat
{
    public static BaccaratRoomInfo GetBaccaratRoomInfoFromTier(enumGameTier tier)
    {
        BaccaratRoomInfo info = new BaccaratRoomInfo();
        switch (tier)
        {
            case enumGameTier.BaccaratRegular:
                info.minBet = 100;
                info.maxBet = 12500;
                info.totalPlayers = 9;
                info.coin = 1500;
                info.gem = 1;
                break;
            case enumGameTier.BaccaratSilver:
                info.minBet = 1000;
                info.maxBet = 125000;
                info.totalPlayers = 9;
                info.coin = 15000;
                info.gem = 10;
                break;
            case enumGameTier.BaccaratGold:
                info.minBet = 5000;
                info.maxBet = 625000;
                info.totalPlayers = 9;
                info.coin = 75000;
                info.gem = 50;
                break;
            case enumGameTier.BaccaratPlatinum:
                info.minBet = 10000;
                info.maxBet = 1250000;
                info.totalPlayers = 9;
                info.coin = 150000;
                info.gem = 100;
                break;
        }
        return info;
    }
}



public static class staticFunction_Fortune
{
    public static int GetBasePrice(enumGameTier tier)
    {
        int bonus = 0;
        switch (tier)
        {
            case enumGameTier.FortuneNewbie:
                bonus = 100;
                break;
            case enumGameTier.FortuneBeginner:
                bonus = 200;
                break;
            case enumGameTier.FortuneVeteran:
                bonus = 500;
                break;
            case enumGameTier.FortuneIntermediate:
                bonus = 1000;
                break;
            case enumGameTier.FortuneAdvanced:
                bonus = 5000;
                break;
            case enumGameTier.FortuneMaster:
                bonus = 10000;
                break;
        }
        return bonus;
    }

    internal static int GetPenaltyFromLucky(Lucky luckName)
    {
        int res = 0;
        switch (luckName)
        {
            case Lucky.Grand_Dragon: res = 60; break;
            case Lucky.Dragon: res = 50; break;
            case Lucky.Twelve_Royals: res = 40; break;
            case Lucky.Three_Straight_Flushes: res = 35; break;
            case Lucky.Three_4_Of_A_Kind: res = 35; break;
            case Lucky.All_Small: res = 30; break;
            case Lucky.All_Big: res = 30; break;
            case Lucky.Same_Colour: res = 25; break;
            case Lucky.Four_Triples: res = 25; break;
            case Lucky.Five_Pair_Plus_Triple: res = 20; break;
            case Lucky.Six_Pairs: res = 15; break;
            case Lucky.Three_Straights: res = 15; break;
            case Lucky.Three_Flushes: res = 10; break;
            default:
                res = 0;
                break;
        }
        return res;
    }


    internal static List<Card> GetLuckyList(int luckyNo)
    {
        List<List<Card>> luckyList = new List<List<Card>>();

        List<Card> list = new List<Card>();
        list.Add(new Card(1, 0)); list.Add(new Card(2, 0)); list.Add(new Card(3, 0));
        list.Add(new Card(4, 0)); list.Add(new Card(5, 0)); list.Add(new Card(6, 0)); list.Add(new Card(7, 0)); list.Add(new Card(8, 0));
        list.Add(new Card(9, 0)); list.Add(new Card(10, 0)); list.Add(new Card(11, 0)); list.Add(new Card(12, 0)); list.Add(new Card(13, 0));
        luckyList.Add(list);

        list = new List<Card>();
        list.Add(new Card(1, 0)); list.Add(new Card(2, 0)); list.Add(new Card(3, 2));
        list.Add(new Card(4, 0)); list.Add(new Card(5, 2)); list.Add(new Card(6, 3)); list.Add(new Card(7, 0)); list.Add(new Card(8, 0));
        list.Add(new Card(9, 1)); list.Add(new Card(10, 3)); list.Add(new Card(11, 2)); list.Add(new Card(12, 0)); list.Add(new Card(13, 1));
        luckyList.Add(list);

        list = new List<Card>();
        list.Add(new Card(11, 0)); list.Add(new Card(11, 1)); list.Add(new Card(1, 0));
        list.Add(new Card(12, 0)); list.Add(new Card(12, 1)); list.Add(new Card(12, 2)); list.Add(new Card(11, 2)); list.Add(new Card(11, 3));
        list.Add(new Card(13, 0)); list.Add(new Card(13, 1)); list.Add(new Card(13, 2)); list.Add(new Card(13, 3)); list.Add(new Card(12, 3));
        luckyList.Add(list);

        try
        {
            return luckyList[luckyNo];
        }
        catch
        {
            return luckyList[luckyList.Count-1];
        }
    }
}


public class staticClassContainer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
