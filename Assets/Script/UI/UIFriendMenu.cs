using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriendMenu : MonoBehaviour
{
    public UIFriendItem[] friendItems;
    public UIFriendItem[] totalItems;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < friendItems.Length; i++)
        {
            friendItems[i].mFriendPic.sprite = Resources.Load<Sprite>("new_avatar/avatar_" + (i + 3).ToString());
//            friendItems[i].mFriendStarPic.sprite = Resources.Load<Sprite>("new_symbol/star");
            friendItems[i].mFriendFrame.sprite = Resources.Load<Sprite>("new_frame/frame_spring");
//            friendItems[i].mFriendStarValue.text = ((i + 1) * 2000).ToString();
//            friendItems[i].mFriendSkillPic.sprite = Resources.Load<Sprite>("new_skill/skill_" + (i + 1).ToString());
//            friendItems[i].mFriendSkillValue.text = "Novice" + " " + (i + 1).ToString();
        }
//        for (int i = 0; i < totalItems.Length; i++)
//        {
//            totalItems[i].mFriendPic.sprite = Resources.Load<Sprite>("new_avatar/avatar_" + (i + 3).ToString());
//            totalItems[i].mFriendStarPic.sprite = Resources.Load<Sprite>("new_symbol/star");
//            totalItems[i].mFriendFrame.sprite = Resources.Load<Sprite>("new_frame/frame_spring");
//            totalItems[i].mFriendStarValue.text = ((i + 1) * 2000).ToString();
//            totalItems[i].mFriendSkillPic.sprite = Resources.Load<Sprite>("new_skill/skill_" + (i + 1).ToString());
//            totalItems[i].mFriendSkillValue.text = "Novice" + " " + (i + 1).ToString();
//        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClose(GameObject obj)
    {
        Debug.Log("close click");
        obj.SetActive(false);
    }
}
