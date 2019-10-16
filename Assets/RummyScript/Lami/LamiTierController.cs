using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LamiTierController : MonoBehaviour
{
    
    public GameObject[] mTiers;
    public GameObject[] mLamiDialogs;
    
    public Transform userInfoPanel;
    public Transform moneyPanel;
    
    // Start is called before the first frame update
    void Start()
    {
//        Debug.Log("roomCount:"+PunController.Inst.cachedRoomList.Count);
//        position = aTransform.localPosition;
//        rotation = aTransform.localRotation;
//        localScale = aTransform.localScale;
        UIController.Inst.loadingDlg.gameObject.SetActive(false);
        UIController.Inst.userInfoPanel.gameObject.SetActive(true);
        UIController.Inst.moneyPanel.gameObject.SetActive(true);
        UIController.Inst.userInfoPanel.transform.position = userInfoPanel.position;
        UIController.Inst.moneyPanel.transform.position= moneyPanel.position;
        UIController.Inst.userInfoPanel.transform.localScale = userInfoPanel.localScale;
        UIController.Inst.moneyPanel.transform.localScale= moneyPanel.localScale;
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("2_Lobby");
    }

    public void OnClickHeader(int type)
    {
        Debug.Log("HeaderButton click");
        int idx = (int)type;
        if (idx < mLamiDialogs.Length)
        {
            mLamiDialogs[idx].SetActive(true);
        }
    }
    
    public void OnClickLamiTier(int type)
    {        
        Debug.Log("TierButton click:"+type+"  roomCount:"+PunController.Inst.cachedRoomList.Count);
        int idx = (int)type;
        if (idx < mTiers.Length)
        {
            
            PunController.Inst.CreateOrJoinRoom(idx);
        }
    }
    public void OnClickMoneyPanel()
    {
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }

}
