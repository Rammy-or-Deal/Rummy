using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
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
        //MakeWrongPasswordEffect();
        Shake();
    }

    #region  Try_Effect
    // Update is called once per frameprivate Vector3 originPosition;
    private Vector3 originPosition;
    private float temp_shake_intensity = 0;
    float dx = 1;
    void Update()
    {

        if (temp_shake_intensity > 0)
        {

            Vector3 tra = new Vector3(dx, 0, 0);

            temp_shake_intensity -= 1;

            if (temp_shake_intensity % 2 == 0)
                transform.position = originPosition + tra * 5;
            dx = -dx;
        }
    }

    void Shake()
    {
        originPosition = transform.position;
        temp_shake_intensity = 20;
    }
    #endregion
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
            catch (Exception err)
            {
                GameMgr.Inst.Log("Join Room Failed: " + err.Message);
                txtPassword.text = "";
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            txtPassword.text = "";
            Shake();
        }
    }

}
