using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BaccaratPanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static BaccaratPanMgr Inst;
    public Text m_panTime;
    public GameObject m_panClock;
    public UIBBetPanel betPanel;
    void Start()
    {
        if (!Inst)
            Inst = this;
        if (PhotonNetwork.IsMasterClient)
        {
            StartNewPan();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal async void StartNewPan()
    {
        await Task.Delay(1000);
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Hashtable prop = new Hashtable{
                {Common.PLAYER_BETTING_LOG, ""},
                {Common.NOW_BET, ""},
            };
            player.SetCustomProperties(prop);
        }

        Hashtable table = new Hashtable{
            {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnStartNewPan},
            {Common.BACCARAT_CURRENT_TIME, Constants.BaccaratCurrentTime}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnStartNewPan()
    {
        m_panClock.gameObject.SetActive(true);
        StartCoroutine(WaitFor1Second());
    }

    public IEnumerator WaitFor1Second()
    {
        int time = (int)PhotonNetwork.CurrentRoom.CustomProperties[Common.BACCARAT_CURRENT_TIME];
        m_panTime.text = time + "";
        yield return new WaitForSeconds(1.0f);
        time--;
        if (PhotonNetwork.IsMasterClient)
        {
            if (time >= 0)
            {
                Hashtable table = new Hashtable{
                    {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnPanTimeUpdate},
                    {Common.BACCARAT_CURRENT_TIME, time}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
            else
            {
                Hashtable table = new Hashtable{
                    {Common.BACCARAT_MESSAGE, (int)BaccaratMessages.OnEndPan},
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
        }
    }

    internal void OnPanTimeUpdate()
    {
        m_panClock.gameObject.SetActive(true);
        StartCoroutine(WaitFor1Second());
    }

    internal void OnPlayerBet(float x, float y, int moneyId, int areaId)
    {
        LogMgr.Inst.Log(string.Format("Player Bet. x={0}, y={1}, moneyId={2}, areaId={3}", x, y, moneyId, areaId), (int)LogLevels.PanLog); 
        betPanel.OnPlayerBet(x, y, moneyId, areaId);
    }
}