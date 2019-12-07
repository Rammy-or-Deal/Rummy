using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummyBotMgr : BotMgr
{
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.Inst.botMgr = this;
        base.CreateBot();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}