﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIReadyDialog : MonoBehaviour
{
    public static UIReadyDialog Inst;

    public Image mTierPic;
    public Text mTierText;
    public Text mWinMoreText;
    public Text mBetterHandText;
    // Start is called before the first frame update
    [HideInInspector] public int idx;
    private void Awake()
    {
        if (!Inst)
        {
            Inst = this;
            idx = -1;
        }
    }

    private void Start()
    {
        mWinMoreText.text = "WinMore(2Gem)";
        mBetterHandText.text = "BetterHand(2Gem)";
    }

    public void OnWinMoreClick()
    {
        mWinMoreText.text = "Activated";
    }

    public void OnBetterHandClick()
    {
        mBetterHandText.text = "Activated";
    }

    public void OnResetClick()
    {
        mWinMoreText.text = "WinMore(2Gem)";
        mBetterHandText.text = "BetterHand(2Gem)";
    }
    public void OnReadyClick()
    {
        if(idx == -1) return;
        
        PunController.Inst.CreateOrJoinLuckyRoom(idx);
        //SceneManager.LoadScene("3_PlayFortune13");
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}