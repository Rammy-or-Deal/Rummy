using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class LamiCountdownTimer : MonoBehaviour
{
    public static LamiCountdownTimer Inst;

    public Text m_timer_description;
    public Text turnTime;
    private Coroutine myCoroutine;
    private Coroutine userCoroutine;

    public bool isMe;

    private float countdownValue = Constants.waitTime_Develop;
    public void Awake()
    {
        if (!Inst)
        {
            Inst = this;
        }
    }

    public void Start()
    {

    }


    float currCountdownValue;

    public IEnumerator StartCountdown()
    {
        if (Constants.LamiBuildMethod == BuildMethod.Product)
            countdownValue = Constants.waitTime_Product;

        currCountdownValue = countdownValue;

        while (currCountdownValue > 0)
        {
            m_timer_description.gameObject.SetActive(true);
            m_timer_description.text = string.Format("Game starts in {0} seconds", currCountdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        PunController.Inst.LeaveGame();
        Debug.Log("Time out");
    }

    public IEnumerator StartTurnTime()
    {
        float turnTimeValue = Constants.turnTime_Develop;

        if (Constants.LamiBuildMethod == BuildMethod.Product)
            turnTimeValue = Constants.turnTime_Product;

        Debug.Log("Time StartTurnTime");
        currCountdownValue = turnTimeValue;
        if (LamiMe.Inst.isAuto && isMe)
            currCountdownValue = Constants.turnTime_AutoPlay;

        while (currCountdownValue > 0)
        {
            turnTime.gameObject.SetActive(true);
            turnTime.text = string.Format("{0}", currCountdownValue.ToString());

            try
            {
                m_timer_description.text = string.Format("You have to deal in {0}, or will be auto played.", currCountdownValue.ToString());
            }
            catch
            {
            }

            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }

        if (isMe)
        {
            LamiMe.Inst.SelectTipFirstCard();
            LamiGameUIManager.Inst.myCardPanel.InitPanList();
            if (LamiGameUIManager.Inst.myCardPanel.m_machedList.Count > 0)
            {
                //            LamiGameUIManager.Inst.myCardPanel.m_machedList.Sort((a, b) => b.list.Count(x => x.num == 15) - b.list.Count(x => x.num == 15));
                LamiGameUIManager.Inst.myCardPanel.SendDealtCard(LamiGameUIManager.Inst.myCardPanel.m_machedList[0].lineNo, LamiGameUIManager.Inst.myCardPanel.m_machedList[0].list);
                LamiMe.Inst.isAuto = true;
                Hashtable table = new Hashtable{
                    {Common.LAMI_MESSAGE, (int)LamiMessages.OnAutoPlayer},
                    {Common.PLAYER_ID, (int)PhotonNetwork.LocalPlayer.ActorNumber}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
                LamiGameUIManager.Inst.uiSelectCardList.Hide();
                LamiGameUIManager.Inst.autoOffBtn.SetActive(true);
            }
        }
    }

    public void StartTimer()
    {
        try
        {
            StopCoroutine(myCoroutine);
        }
        catch { }
        myCoroutine = StartCoroutine(StartCountdown());
    }

    public void StartTurnTimer(bool _isMe)
    {
        isMe = _isMe;
        try
        {
            StopCoroutine(userCoroutine);
        }
        catch { }
        userCoroutine = StartCoroutine(StartTurnTime());
    }
    public void StopTurnTimer()
    {
        Debug.Log("Turn Timer stopped");
        try
        {
            StopCoroutine(userCoroutine);
            //m_timer_description.gameObject.SetActive(false);            
        }
        catch { }
    }

    public void StopTimer()
    {
        Debug.Log("Timer stopped");
        try
        {
            StopCoroutine(myCoroutine);
            m_timer_description.gameObject.SetActive(false);
        }
        catch { }

    }
}