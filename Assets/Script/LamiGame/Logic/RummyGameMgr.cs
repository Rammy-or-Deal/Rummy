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
    public RummyPanMgr panMgr;
    public RummyResultMgr resultMgr;
    public RummySeatMgr seatMgr;
    public RummyCardMgr cardMgr;
    internal bool isFirstTurn;

    void Start()
    {
        if(!Inst)
        {
            Inst = this;
            isFirstTurn = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}