using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMgr : MonoBehaviour
{
    enum LogLevels{
        RoomLog1, 
        RoomLog2, 
        RoomLog3,
        PlayerLog1,
        PlayerLog2,
    }
    public static LogMgr Inst;
    static List<int> avail_logs = new List<int>();
    void Awake(){
        if (!Inst)
            Inst= this;
        avail_logs.Add(0);
        avail_logs.Add(1);
        avail_logs.Add(2);
    }
    // Start is called before the first frame update
    public static void Log(string log,int level=0){
        if (avail_logs.Contains(level))
            Debug.Log(log);
    }
}
