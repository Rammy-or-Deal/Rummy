using Photon.Pun;
using UnityEngine;
using Photon.Voice.PUN;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Inst;
    //Menu
    public GameObject mMenuPanel;
    public GameObject settingDlg;
    
    public void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }
    
    protected void Start()
    {
        UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        UIController.Inst.moneyPanel.gameObject.SetActive(false);
        
        // Create Voice View Component  when joined Room.
        PhotonNetwork.Instantiate("Prefabs/VoiceView", Vector3.zero, Quaternion.identity, 0);
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
}
