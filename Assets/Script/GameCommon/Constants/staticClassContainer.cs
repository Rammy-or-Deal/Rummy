using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;


public class GameRoomInfo{
    public enumGameType m_gameType{get; set;}
    public enumGameTier m_gameTier{get; set;}
    public string m_roomName{get; set;}
    public int m_gameFee{get; set;}
    public int m_playerCount{get; set;}
    public int m_maxPlayer{get; set;}
    public string m_additionalString{get; set;}

    public GameRoomInfo()
    {
        
    }
    public string roomInfoString{
        get{
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", (int)m_gameType, (int)m_gameTier, m_roomName, m_gameFee, m_playerCount, m_maxPlayer, m_additionalString);
        }
        set{
            var tmpList = value.Split(':');
            m_gameType = (enumGameType)(int.Parse(tmpList[0]));
            m_gameTier = (enumGameTier)(int.Parse(tmpList[1]));
            m_roomName = tmpList[2];
            m_gameFee = int.Parse(tmpList[3]);
            m_playerCount = int.Parse(tmpList[4]);
            m_maxPlayer = int.Parse(tmpList[5]);
            m_additionalString = "";
            for(int i = 6; i < tmpList.Length; i++)
            {
                m_additionalString += tmpList[i];
            }
        }
    }
}

public class PlayerInfoContainer{
    public List<PlayerInfo> m_playerList;
    public string m_playerInfoListString{
        get{
            return string.Join(",", m_playerList.Select(x=>x.playerInfoString));
        }
        set{
            var tmpList = value.Split(',');
            foreach(var tmp in tmpList)
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
        try{
            string infoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
            this.m_playerInfoListString = infoString;
        }catch{
            GameMgr.Inst.Log("No player list info.", enumLogLevel.staticClassLog);
            return null;
        }
        return this;
    }
}
public class PlayerInfo{

    public int m_actorNumber;
    public string m_userName;
    public string m_userPic;
    public int m_coinValue;
    public string m_skillLevel;
    public int m_frameId;
    public enumPlayerStatus m_status;

    public string playerInfoString{
        get{
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
        set{
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
public class SeatInfo{

    public class OneSeat{
        public int m_actorNumber;
        public int m_seatNo;
        public string m_userName;
        public string oneSeatString{
            get{
                return string.Format("{0}:{1}:{2}", m_actorNumber, m_seatNo, m_userName);
            }
            set{
                var tmpList = value.Split(':');
                m_actorNumber = int.Parse(tmpList[0]);
                m_seatNo = int.Parse(tmpList[1]);
                m_userName = tmpList[2];
            }
        }
        public OneSeat(){}
        public OneSeat(int actorNumber, int seatNo, string userName){
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
    public string seatString{
        get{
            return string.Join(",", seatList.Select(x=>x.oneSeatString));
        }
        set{
            seatList.Clear();
            var tmpList = value.Split(',');
            foreach(var tmp in tmpList)
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
