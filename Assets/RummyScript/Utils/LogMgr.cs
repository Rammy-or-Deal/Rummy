using System.Collections;
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
    SpecialLog,
    ShowLamiAllList,
    MeLog_Baccarat,
    PanLog,
    CardLog,
}

public class LogMgr : MonoBehaviour
{

    public static LogMgr Inst;
    static List<int> avail_logs = new List<int>();
    void Awake()
    {
        if (!Inst)
            Inst = this;

        if (Constants.LamiBuildMethod == BuildMethod.Develop_Message)
        {
            avail_logs.Add((int)LogLevels.RoomLog1);
            avail_logs.Add((int)LogLevels.RoomLog2);
            avail_logs.Add((int)LogLevels.RoomLog3);
            avail_logs.Add((int)LogLevels.PlayerLog1);
            avail_logs.Add((int)LogLevels.PlayerLog2);
            avail_logs.Add((int)LogLevels.BotLog);
            avail_logs.Add((int)LogLevels.MasterLog);
            avail_logs.Add((int)LogLevels.MeLog);
            avail_logs.Add((int)LogLevels.SpecialLog);
            avail_logs.Add((int)LogLevels.ShowLamiAllList);

            avail_logs.Add((int)LogLevels.BotLog);
            avail_logs.Add((int)LogLevels.PlayerLog1);
            avail_logs.Add((int)LogLevels.PanLog);

            avail_logs.Add((int)LogLevels.CardLog);
        }

    }
    // Start is called before the first frame update
    public void Log(string log, int level = 0)
    {
        if (avail_logs.Contains(level))
            Debug.Log(log);
    }
    public void ShowLog(List<List<Card>> allList, string header = "", int loglevel = (int)LogLevels.SpecialLog)
    {
        foreach (var line in allList)
        {
            string tmp = "";
            foreach (var card in line)
            {
                tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
            }
            tmp += "/";
            LogMgr.Inst.Log(header + "   " + tmp + "-----", loglevel);
        }
    }
    public void ShowLog(List<Card> list, string header = "", int loglevel = (int)LogLevels.SpecialLog)
    {
        string tmp = "";
        foreach (var card in list)
        {
            tmp += card.num + "(" + card.virtual_num + "):" + card.color + ",";
        }
        tmp += "/";
        LogMgr.Inst.Log(header + "   " + tmp + "-----", loglevel);
    }
}