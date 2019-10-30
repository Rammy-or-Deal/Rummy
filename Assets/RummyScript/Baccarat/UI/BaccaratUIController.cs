using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaccaratUIController : MonoBehaviour
{
    //Menu
    public GameObject mMenuPanel;
    public GameObject settingDlg;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickBettingArea(int id) // Id=0,1,2,3,4
    {
        if(UIBBetBtnList.Inst.selectedId == -1) return;
        BaccaratMe.Inst.OnClickBettingArea(UIBBetBtnList.Inst.selectedId, id);
    }

    public void OnClickMenu()
    {
        bool active = mMenuPanel.activeSelf == true ? false : true;
        mMenuPanel.SetActive(active);
    }

    public void OnExitClick()
    {
        Debug.Log("Exit clicked");
        PhotonNetwork.LeaveRoom();
        //PunController.Inst.LeaveGame();
    }

    public void OnHelpClick()
    {
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }

    public void OnSettingClick()
    {
        settingDlg.SetActive(true);
    }
    public void OnClickChat()
    {
        Debug.Log("chat clicked ");
        UIController.Inst.chatDlg.gameObject.SetActive(true);
    }
}
