using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaccaratUIController : GameUIController
{
    public void OnClickBettingArea(int id) // Id=0,1,2,3,4
    {
//        LogMgr.Inst.Log("Clicked Betting Area. id="+id, (int)LogLevels.PlayerLog1);

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
