using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public Transform userInfoPanel;
    public Transform moneyPanel;

    // Start is called before the first frame update
    void Start()
    {
        PunController.Inst.Login();
        UIController.Inst.userInfoPanel.gameObject.SetActive(true);
        UIController.Inst.moneyPanel.gameObject.SetActive(true);
        UIController.Inst.userInfoPanel.transform.position = userInfoPanel.position;
        UIController.Inst.moneyPanel.transform.position = moneyPanel.position;
        UIController.Inst.userInfoPanel.transform.localScale = userInfoPanel.localScale;
        UIController.Inst.moneyPanel.transform.localScale = moneyPanel.localScale;
    }

    public void OnClickLami()
    {
        //LoadGameScene("2_Lami");
        GameMgr.Inst.LoadGameScene2(enumGameType.Lami);
    }

    public void OnClickBaccarat()
    {
        //LoadGameScene("2_Baccarat");
        GameMgr.Inst.LoadGameScene2(enumGameType.Baccarat);
        //PunController.Inst.CreateOrJoinBaccaratRoom();
    }

    public void OnClickFortune()
    {
        //LoadGameScene("2_Fortune13");
        GameMgr.Inst.LoadGameScene2(enumGameType.Fortune13);
    }

    public void OnClikBack()
    {
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
}
