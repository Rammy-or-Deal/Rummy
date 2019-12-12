using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBPasswordVerificationDlg : MonoBehaviour
{
    // Start is called before the first frame update
    public static UIBPasswordVerificationDlg Inst;
    public InputField txtPassword;
    [HideInInspector] public BaccaratRoomInfo baccaratRoomInfo = null;
    [HideInInspector] public GameRoomInfo commonRoomInfo = null;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            if (commonRoomInfo == null)
                commonRoomInfo = new GameRoomInfo();
            if (baccaratRoomInfo == null)
                baccaratRoomInfo = new BaccaratRoomInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckPassword(string roomInfoString)
    {
        this.gameObject.SetActive(true);
        commonRoomInfo.roomInfoString = roomInfoString;
        baccaratRoomInfo.roomString = commonRoomInfo.m_additionalString;
    }

    public void OnClickCloseButton()
    {
        txtPassword.text = "";
        this.gameObject.SetActive(false);
    }
    public void OnClickJoinButton()
    {
        Debug.Log("Current typed password:=" + txtPassword.text);

        if (txtPassword.text == baccaratRoomInfo.password)
        {
            try
            {
                GameMgr.Inst.roomMgr.JoinRoom(commonRoomInfo.m_roomName);
            }
            catch(Exception err)
            {
                GameMgr.Inst.Log("Join Room Failed: " + err.Message);
                txtPassword.text = "";
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            txtPassword.text = "";
        }
    }
}
