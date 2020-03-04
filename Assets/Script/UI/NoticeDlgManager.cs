using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoticeDlgManager : MonoBehaviour
{
    public GameObject[] mSelectedBtns;
    public GameObject[] mNoSelectedBtns;
    public GameObject[] mPanViews;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onBackgroundButtonclicked(GameObject obj)
    {
        OnClose(obj);
    }

    public void OnClose(GameObject obj)
    {
        obj.SetActive(false);
    }
    
    public void OnClickShopBtn(int type)
    {
        Debug.Log("Shop notice button click");

         for (int i = 0; i < mNoSelectedBtns.Length; i++)
        {
            mNoSelectedBtns[i].SetActive(true);
            mSelectedBtns[i].SetActive(false);
            mPanViews[i].SetActive(false);
        }

        int idx = (int)type;
        if (idx < mSelectedBtns.Length)
        {
            mSelectedBtns[idx].SetActive(true);
            mPanViews[idx].SetActive(true);
        }
    }
    
    public void OnClickDialogBtn(int type)
    {
        Debug.Log("button click");

        for (int i = 0; i < mNoSelectedBtns.Length; i++)
        {
            mNoSelectedBtns[i].SetActive(true);
            mSelectedBtns[i].SetActive(false);
            mPanViews[i].SetActive(false);
        }

        int idx = (int)type;
        if (idx < mSelectedBtns.Length)
        {
            mSelectedBtns[idx].SetActive(true);
            mPanViews[idx].SetActive(true);
        }
    }
}
