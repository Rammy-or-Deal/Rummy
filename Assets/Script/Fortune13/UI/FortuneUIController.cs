using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneUIController : GameUIController
{
//Menu
    public static FortuneUIController Inst;
    public UIResultDialog resultDlg;
    public UICalcDialog calcDlg;
    public UIChangeCardDialog changeDlg;
    public UILuckyDialog luckyDlg;
    public UILuckyAlert luckyAlert;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if(!Inst)
            Inst = this;
        StartCoroutine(Init());
    }
   
    IEnumerator Init()
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        calcDlg.gameObject.SetActive(true);
        
        yield return new WaitForFixedUpdate();
        
        calcDlg.gameObject.SetActive(false);
        UIController.Inst.loadingDlg.gameObject.SetActive(false);
    }
}
