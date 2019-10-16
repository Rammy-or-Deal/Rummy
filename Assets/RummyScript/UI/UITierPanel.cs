using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITierPanel : MonoBehaviour
{
    public GameObject[] mTiers;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnClickLamiTier(int type)
    {        
        Debug.Log("TierButton click:"+type+"  roomCount:"+PunController.Inst.cachedRoomList.Count);
        int idx = (int)type;
        if (idx < mTiers.Length)
        {
            PunController.Inst.CreateOrJoinRoom(idx);
        }
    }
    
   

    public void OnUpClick()
    {
        Debug.Log("UPClick");
        for (int i = 0; i < mTiers.Length; i++)
        {
            if (mTiers[i].activeSelf)
            {
                
                if (i == 5 )
                {
                    mTiers[i].SetActive(false);
                    mTiers[0].SetActive(true);
                }
                else
                {
                    mTiers[i].SetActive(false);
                    mTiers[i + 1].SetActive(true);
                }

                return;
            }
        }
    }
    
    public void OnDownClick()
    {
        Debug.Log("DownClick");
        for (int i = mTiers.Length - 1; i > -1; i--)
        {
            if (mTiers[i].activeSelf)
            {
                
                if (i == 0 )
                {
                    mTiers[i].SetActive(false);
                    mTiers[5].SetActive(true);
                }
                else
                {
                    mTiers[i].SetActive(false);
                    mTiers[i - 1].SetActive(true);
                }

                return;
            }
        }
    }
}
