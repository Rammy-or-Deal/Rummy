using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TierController : MonoBehaviour
{
    public GameObject[] mTiers;

    public Transform userInfoPanel;
    public Transform moneyPanel;


    void Start()
    {
        try
        {
            UIController.Inst.loadingDlg.gameObject.SetActive(false);
        }
        catch { }
        UIController.Inst.userInfoPanel.gameObject.SetActive(true);
        UIController.Inst.moneyPanel.gameObject.SetActive(true);
        UIController.Inst.userInfoPanel.transform.position = userInfoPanel.position;
        UIController.Inst.moneyPanel.transform.position = moneyPanel.position;
        UIController.Inst.userInfoPanel.transform.localScale = userInfoPanel.localScale;
        UIController.Inst.moneyPanel.transform.localScale = moneyPanel.localScale;
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene(Constant.LobbyScene);
    }

    public void OnClickMoneyPanel()
    {
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }

    public void OnClickTier(enumGameTier type)
    {        
        GameMgr.Inst.EnterTier(type);
    }
}
