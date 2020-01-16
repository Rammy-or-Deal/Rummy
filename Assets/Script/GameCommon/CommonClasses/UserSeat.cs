using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserSeat : MonoBehaviour
{
    public bool canShow;
    public int status;
    public Text mCardNum;
    public Text mClockTime;
    public Image mUserFrame;
    public Image mUserPic;
    public Image mUserSkillPic;
    public Text mUserName;
    public Text mUserSkillName;
    public Text mCoinValue;
    public int frameId;
    public bool isSeat {
        get{
            return this.gameObject.activeSelf;
        }
        set{
            this.gameObject.SetActive(value);
        }
    }

    public PlayerInfo m_playerInfo = null;

    public virtual void SetPlayerInfo(PlayerInfo info)
    {
        if(m_playerInfo == null) 
            m_playerInfo = new PlayerInfo();

        m_playerInfo.playerInfoString = info.playerInfoString;

        name = m_playerInfo.m_userName;
        mUserName.text = m_playerInfo.m_userName;
        
        if (m_playerInfo.m_userPic.Contains("avatar_"))
            mUserPic.sprite = Resources.Load<Sprite>(m_playerInfo.m_userPic);
        else  //facebook Pic
        {
            DataController.Inst.GetFBPicture(m_playerInfo.m_userPic,mUserPic.sprite);
        }
        
        mCoinValue.text = m_playerInfo.m_coinValue.ToString();
        mUserSkillName.text = m_playerInfo.m_skillLevel;
        frameId = m_playerInfo.m_frameId;
        isSeat = true;
    }
    
}
