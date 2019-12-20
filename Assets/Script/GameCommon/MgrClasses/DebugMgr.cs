using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMgr : MonoBehaviour
{
    // Start is called before the first frame update
    List<enumLogLevel> avail_logs;

    private void Awake() {
        avail_logs = new List<enumLogLevel>();

        if (constantContainer.buildMethod == enumBuildMethod.Development_Debug || constantContainer.buildMethod == enumBuildMethod.Product_Debug)
        {
            //avail_logs.Add(enumLogLevel.initLog);
            avail_logs.Add(enumLogLevel.RoomLog);  //amg  
            // avail_logs.Add(enumLogLevel.MeLog);
            // avail_logs.Add(enumLogLevel.BotLog);
            // avail_logs.Add(enumLogLevel.ControllerMessage);
            // avail_logs.Add(enumLogLevel.RummySeatMgrLog);
            // avail_logs.Add(enumLogLevel.RummyCardMgrLog);
            //avail_logs.Add(enumLogLevel.BaccaratLogicLog);            
//            avail_logs.Add(enumLogLevel.FortuneLuckyLog);
        }
    }
    
    public void Log(string log, enumLogLevel level = enumLogLevel.initLog)
    {
        enumGameType gameType = GameMgr.Inst.m_gameType;
        if (!avail_logs.Contains(level)) return;
        Debug.Log(gameType + ": " + log);
    }

}
