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

    public Text Text;
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
            Text.text = string.Format("Game starts in {0} seconds", currCountdownValue.ToString());
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
            Text.text =  string.Format("{0}", currCountdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }  
        
    }

    public void StartTimer()
    {
        myCoroutine = StartCoroutine(StartCountdown());
    }
    
    public void StartTurnTimer()
    {
        Text.gameObject.SetActive(true);
        userCoroutine = StartCoroutine(StartTurnTime());
        
    }
    public void StopTurnTimer()
    {
        Debug.Log("Turn Timer stopped");
        StopCoroutine(userCoroutine);
        Text.gameObject.SetActive(false);
    }

    public void StopTimer()
    {
        Debug.Log("Timer stopped");
        StopCoroutine(myCoroutine);
        Text.gameObject.SetActive(false);
    }
}