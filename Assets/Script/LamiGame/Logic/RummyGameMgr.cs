using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummyGameMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static RummyGameMgr Inst;

    public RummyBotMgr botMgr;
    public RummyMasterMgr masterMgr;
    public RummyMeMgr meMgr;
    public RummyMessageMgr messageMgr;
    public RummyPanMgr panMgr;
    public RummyResultMgr resultMgr;
    public RummySeatMgr seatMgr;
    
    void Start()
    {
        if(!Inst)
        {
            Inst = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}