using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardDialog : MonoBehaviour
{
    public Text mRewardView;
    // Start is called before the first frame update
    void Start()
    {
        mRewardView.text = "Do want to watch VIDEO?";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickConfirm()
    {
        mRewardView.text = "You earn 500 Gold !";
        DataController.Inst.userInfo.coinValue += 500;
        UIController.Inst.moneyPanel.UpdateValue();
    }
    
    public void OnClose(GameObject obj)
    {
        Debug.Log("close click");
        obj.SetActive(false);
        mRewardView.text = "Do want to watch VIDEO?";
    }
}
