using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiFinish : MonoBehaviour
{
    public UILamiFinishScorePan[] scorePan;
    void Start()
    {
        SetData();
    }

    public void SetData()
    {
        for (int i = 0; i < scorePan.Length; i++)
        {
            LamiUserSeat seat=LamiPlayerMgr.Inst.m_playerList[i];
            scorePan[i].UpdateInfo(seat.mUserPic.sprite,"king",seat.mUserSkillName.text,seat.mUserName.text,i.ToString(),23,seat.mAceValue.text,seat.mJokerValue.text,230);
        } 
    }

    public void OnReportBtn()
    {
        
    }
    
    public void OnExitBtn()
    {
        //gameObject.SetActive(false);
        PunController.Inst.LeaveGame();
    }
    
    public void OnContinueBtn()
    {
        gameObject.SetActive(false);
    }
}
