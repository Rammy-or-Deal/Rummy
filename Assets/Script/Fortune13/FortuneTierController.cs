using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneTierController : MonoBehaviour
{

    public GameObject[] mTiers;
    public UIReadyDialog readyDlg;
    public Transform userInfoPanel;
    public Transform moneyPanel;


    private void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");
    }

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
    public void OnClickFortuneTier(int type)
    {      
        

        readyDlg.mTierPic.sprite = Resources.Load<Sprite>("new_skill/skill_" + (type + 1).ToString());
        readyDlg.mTierText.text = "Tier " + (type + 1).ToString();
        
        readyDlg.gameObject.SetActive(true);
        
        int idx = (int)type;
        if (idx < mTiers.Length)
            UIReadyDialog.Inst.idx = (int)type;
//        Debug.Log("TierButton click:"+type+"  roomCount:"+PunController.Inst.cachedRoomList.Count);        
//        
//        if (idx < mTiers.Length)
//        {
//            PunController.Inst.CreateOrJoinLuckyRoom(idx);
//        }
    }
    public void OnClickBack()
    {
        SceneManager.LoadScene("2_Lobby");
    }

    public void OnClickMoneyPanel()
    {
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }

}
