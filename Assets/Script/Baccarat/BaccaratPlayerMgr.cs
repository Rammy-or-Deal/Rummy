using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BaccaratPlayerMgr : MonoBehaviour
{
    public static BaccaratPlayerMgr Inst;
    public List<BaccaratUserSeat> m_playerList;
    // Start is called before the first frame update

    void Start()
    {
        if (!Inst)
            Inst = this;
    }
    
    internal void OnJoinSuccess()
    {
        try
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(false);
        }
        catch { }

        BaccaratMe.Inst.PublishMe();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) continue;

            if (m_playerList.Count(x => x.isSeat == false) > 0)
                m_playerList.Where(x => x.isSeat == false).First().SetMe((string)player.CustomProperties[Common.PLAYER_INFO]);
        }
    }

    internal async void OnUserEnteredRoom()
    {
        var infostring = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NEW_PLAYER_INFO];

        if (int.Parse(infostring.Split(':')[0]) == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            BaccaratPlayerMgr.Inst.m_playerList[0].SetMe(infostring);

            BaccaratPanMgr.Inst.message.Show("Please wait until this pan is completed.");
            await Task.Delay(3000);
            BaccaratPanMgr.Inst.message.Hide();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) continue;

                if (m_playerList.Count(x => x.isSeat == false) > 0)
                    m_playerList.Where(x => x.isSeat == false).First().SetMe((string)player.CustomProperties[Common.PLAYER_INFO]);
            }
        }
        else
        {
            if (m_playerList.Count(x => x.isSeat == false) > 0)
                m_playerList.Where(x => x.isSeat == false).First().SetMe(infostring);
        }

        //BaccaratMe.Inst.PublishMe();
    }

    internal void OnPlayerBet(int actorNumber)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            BaccaratMe.Inst.OnPlayerBet();
        }
        else
        {
            try
            {
                m_playerList.Where(x => x.id == actorNumber).First().OnPlayerBet();
            }
            catch { }
        }
    }

    internal void OnUserLeave(Player player)
    {
        if (m_playerList.Count(x => x.id == player.ActorNumber) > 0)
        {
            m_playerList.Where(x => x.id == player.ActorNumber).First().OnUserLeave();
        }
    }

    internal void OnUpdateMe(Player player)
    {
        var info = (string)player.CustomProperties[Common.PLAYER_INFO];
        m_playerList.Where(x=>x.id == player.ActorNumber).First().SetMe(info);
    }

    public void MoveCardEffect(GameObject obj,Vector3 pos)
    {
        iTween.MoveTo(obj, iTween.Hash("position", pos, "time", 0.5));    
    }
}
