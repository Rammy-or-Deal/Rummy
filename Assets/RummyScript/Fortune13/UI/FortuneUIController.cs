﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneUIController : MonoBehaviour
{
//Menu
    public static FortuneUIController Inst;
    public GameObject mMenuPanel;
    public GameObject settingDlg;
    public UIResultDialog resultDlg;
    public UICalcDialog calcDlg;
    public UIChangeCardDialog changeDlg;

    // Start is called before the first frame update
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    public void OnClickMenu()
    {
        bool active = mMenuPanel.activeSelf == true ? false : true;
        mMenuPanel.SetActive(active);
    }

    public void OnExitClick()
    {
        Debug.Log("Exit clicked");
        //PunController.Inst.LeaveGame();
        SceneManager.LoadScene("2_Lobby");
    }

    public void OnHelpClick()
    {
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }

    public void OnSettingClick()
    {
        settingDlg.SetActive(true);
    }
    public void OnClickChat()
    {
        Debug.Log("chat clicked ");
        UIController.Inst.chatDlg.gameObject.SetActive(true);
    }
}
