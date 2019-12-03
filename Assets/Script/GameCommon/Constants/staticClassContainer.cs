using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameRoomInfo{
    public enumGameType m_gameType{get; set;}
    public enumGameTier m_gameTier{get; set;}
    public string m_roomName{get; set;}
    public int m_gameFee{get; set;}
    public int m_playerCount{get; set;}
    public int m_maxPlayer{get; set;}
    public string m_additionalString{get; set;}

    public string roomInfoString{
        get{
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", m_gameType, m_gameTier, m_roomName, m_gameFee, m_playerCount, m_maxPlayer, m_additionalString);
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
