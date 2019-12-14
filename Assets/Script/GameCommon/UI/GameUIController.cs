using Photon.Pun;
using UnityEngine;
using Photon.Voice.PUN;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Inst;
    //Menu
    public GameObject mMenuPanel;
    public GameObject settingDlg;
    
    void Awake()
    {
        if (!Inst)
            Inst = this;
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
        UIController.Inst.chatDlg.gameObject.SetActive(true);
    }
    
    public void OnPointerDown()
    {
        Debug.Log("OnPointerDown");
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = true;
    }
    
    public void OnPointerUp()
    {
        Debug.Log("OnPointerUp");
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = false;
    }
}
