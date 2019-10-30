using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaccaratBankerMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratBankerMgr Inst;
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void OnEndPan()
    {
        
    }
}
