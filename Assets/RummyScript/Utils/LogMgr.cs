﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LogLevels
{
    RoomLog1,
    RoomLog2,
    RoomLog3,
    PlayerLog1,
    PlayerLog2,
    BotLog,
    MasterLog,
    MeLog,
}

public class LogMgr : MonoBehaviour
{

    public static LogMgr Inst;
    static List<int> avail_logs = new List<int>();
    void Awake()
    {
        if (!Inst)
            Inst = this;
        avail_logs.Add((int)LogLevels.RoomLog1);
        avail_logs.Add((int)LogLevels.RoomLog2);
        avail_logs.Add((int)LogLevels.RoomLog3);
        avail_logs.Add((int)LogLevels.PlayerLog1);
        avail_logs.Add((int)LogLevels.PlayerLog2);
        avail_logs.Add((int)LogLevels.BotLog);
        avail_logs.Add((int)LogLevels.MasterLog);
        avail_logs.Add((int)LogLevels.MeLog);
    }
    // Start is called before the first frame update
    public void Log(string log, int level = 0)
    {
        if (avail_logs.Contains(level))
            Debug.Log(log);
    }
}