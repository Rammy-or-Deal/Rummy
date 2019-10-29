using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public Transform userInfoPanel;
    public Transform moneyPanel;
    public GameObject uiFriendMenu;

    // Start is called before the first frame update
    void Start()
    {
        UIController.Inst.userInfoPanel.gameObject.SetActive(true);
        UIController.Inst.moneyPanel.gameObject.SetActive(true);
        UIController.Inst.userInfoPanel.transform.position = userInfoPanel.position;
        UIController.Inst.moneyPanel.transform.position = moneyPanel.position;
        UIController.Inst.userInfoPanel.transform.localScale = userInfoPanel.localScale;
        UIController.Inst.moneyPanel.transform.localScale = moneyPanel.localScale;
    }

    public void OnClickLami()
    {
        LoadGameScene("2_Lami");
    }

    public void OnClickBaccarat()
    {
        LoadGameScene("2_Baccarat");
//        PunController.Inst.CreateOrJoinBaccaratRoom();
    }

    public void OnClickFortune()
    {
        LoadGameScene("2_Fortune13");
    }

    void LoadGameScene(string scene)
    {
        UIController.Inst.loadingDlg.gameObject.SetActive(true);
        PhotonNetwork.LoadLevel(scene);
    }

    public void OnClikBack()
    {
        Debug.Log("back pressed");
        SceneManager.LoadScene("1_Title");
    }
    public void OnClickLobbyBottomBtn(int type)
    {
        Debug.Log("Lobby bottom click");

        switch (type)
        {
            case 0:
                UIController.Inst.eventDlg.gameObject.SetActive(true);
                break;
            case 1:
                UIController.Inst.challengeDlg.gameObject.SetActive(true);
                break;
            case 2:
                UIController.Inst.chatDlg.gameObject.SetActive(true);
                break;
            case 3:
                OnClickFriend(uiFriendMenu);
                break;
            case 4:
                UIController.Inst.rewardDlg.gameObject.SetActive(true);
                break;
            case 5:
                UIController.Inst.emailDlg.gameObject.SetActive(true);
                break;
            case 6:
                UIController.Inst.settingDlg.gameObject.SetActive(true);
                break;
            case 7:
                UIController.Inst.spinDlg.gameObject.SetActive(true);
                break;
            case 8:
                UIController.Inst.shopDlg.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnClickUserPanel()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }

    public void OnClickMoneyPanel()
    {
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }

    public void OnClickFriend(GameObject obj)
    {
        obj.SetActive(true);
    }

}
