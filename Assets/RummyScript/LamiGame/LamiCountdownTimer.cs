using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class LamiCountdownTimer : MonoBehaviour
{
    public static LamiCountdownTimer Inst;

    public Text m_timer_description;
    public Text turnTime;
    private Coroutine myCoroutine;
    private Coroutine userCoroutine;

    private float countdownValue = 100;
    private float turnTimeValue = 30;
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
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            m_timer_description.text = string.Format("Game starts in {0} seconds", currCountdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        PunController.Inst.LeaveGame();
        Debug.Log("Time out");
    }

    public IEnumerator StartTurnTime(float turnTimeValue = 30)
    {
        Debug.Log("Time StartTurnTime");
        currCountdownValue = turnTimeValue;
        while (currCountdownValue > 0)
        {
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

        LamiGameUIManager.Inst.OnClickTips();
        LamiGameUIManager.Inst.OnClickPlay();

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

    public void StartTurnTimer()
    {
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