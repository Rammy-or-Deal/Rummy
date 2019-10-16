﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneGameController : MonoBehaviour
{
    public static FortuneGameController Inst;
    // Start is called before the first frame update
    void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
        UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        UIController.Inst.moneyPanel.gameObject.SetActive(false);
    }
}
