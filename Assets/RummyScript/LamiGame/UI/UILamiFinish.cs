using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiFinish : MonoBehaviour
{
    public UILamiFinishScorePan[] scorePan;
    void Start()
    {
        
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
