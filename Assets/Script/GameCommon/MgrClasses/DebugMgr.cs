using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMgr : MonoBehaviour
{
    // Start is called before the first frame update
    List<LogLevel> avail_logs;

    private void Awake() {
        avail_logs = new List<LogLevel>();

        if (constantContainer.buildMethod == enumBuildMethod.Development_Debug || constantContainer.buildMethod == enumBuildMethod.Product_Debug)
        {
            //avail_logs.Add(enumLogLevel.initLog);
            //avail_logs.Add(enumLogLevel.RoomLog);  //amg  code  new            // avail_logs.Add(enumLogLevel.MeLog);
            // avail_logs.Add(enumLogLevel.BotLog);
            // avail_logs.Add(enumLogLevel.ControllerMessage);
            // avail_logs.Add(enumLogLevel.RummySeatMgrLog);
            // avail_logs.Add(enumLogLevel.RummyCardMgrLog);
            //avail_logs.Add(enumLogLevel.BaccaratLogicLog);            
            //avail_logs.Add(enumLogLevel.FortuneLuckyLog);
           avail_logs.Add(LogLevel.BaccaratDistributeCardLog);
        }
    }
    
    public void Log(string log, LogLevel level = LogLevel.initLog)
    {
        enumGameType gameType = GameMgr.Inst.m_gameType;
        if (!avail_logs.Contains(level)) return;
        Debug.Log(gameType + ": " + log);
    }

}
