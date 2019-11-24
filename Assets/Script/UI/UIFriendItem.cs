using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : MonoBehaviour
{
    public int id;
    public Text mFriendName;
    public Image mFriendFrame;
    public Image mFriendPic;
    public Text mFriendState;
    public Text mFriendFind;

    public Image mFriendSkillPic;
    public Text mFriendSkillValue;
    public Image mFriendStarPic;
    public Text mFriendStarValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickGift()
    {
        Debug.Log(id+" item clicked ");
        UIController.Inst.shopDlg.gameObject.SetActive(true);
    }
    public void OnClickChat()
    {
        Debug.Log("chat clicked ");
        UIController.Inst.chatDlg.gameObject.SetActive(true);
    }
}
