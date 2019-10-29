using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaccaratPanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratPanMgr Inst;
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
