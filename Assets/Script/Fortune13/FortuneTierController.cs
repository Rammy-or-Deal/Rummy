using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FortuneTierController : TierController
{
    public UIReadyDialog readyDlg;
    
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

}
