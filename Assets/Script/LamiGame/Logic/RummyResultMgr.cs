using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummyResultMgr : ResultMgr
{
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.Inst.resultMgr = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
