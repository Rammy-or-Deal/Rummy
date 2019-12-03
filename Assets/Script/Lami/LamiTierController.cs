using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LamiTierController : TierController
{
    public GameObject[] mLamiDialogs;
    public static LamiTierController Inst;

    private void Awake() {
        if(!Inst)
            Inst = this;
    }

    // Start is called before the first frame update
    public void OnClickHeader(int type)
    {
        Debug.Log("HeaderButton click");
        int idx = (int)type;
        if (idx < mLamiDialogs.Length)
        {
            mLamiDialogs[idx].SetActive(true);
        }
    }

    
}
