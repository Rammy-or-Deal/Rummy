using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LamiTierController : TierController
{
    public GameObject[] mLamiDialogs;

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

    public void OnClickLamiTier(int type)
    {
        Debug.Log("TierButton click:" + type + "  roomCount:" + PunController.Inst.cachedRoomList.Count);
        int idx = (int)type;
        if (idx < mTiers.Length)
        {

            PunController.Inst.CreateOrJoinRoom(idx);
        }
    }
}
