using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public string oneSeatString{
            get{
                return string.Format("{0}:{1}", m_actorNumber, m_seatNo);
            }
            set{
                var tmpList = value.Split(':');
                m_actorNumber = int.Parse(tmpList[0]);
                m_seatNo = int.Parse(tmpList[1]);
            }
        }
        public OneSeat(){}
        public OneSeat(int actorNumber, int seatNo){
            m_actorNumber = actorNumber;
            m_seatNo = seatNo;
        }
    }
    public List<OneSeat> seatList;

    public SeatInfo()
    {
        seatList = new List<OneSeat>();
    }
    public void AddOneSeat(int actorNumber, int seatNo)
    {
        OneSeat seat = new OneSeat(actorNumber, seatNo);
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
