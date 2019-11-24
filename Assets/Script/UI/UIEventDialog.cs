using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIEventDialog : MonoBehaviour
{

    public Text mAnnounce;
    public UIDayItem[] dayItems;

    private Sprite _mDaypanCollected;
    private Sprite _mDaypanUnCollected;
    private Sprite _mCollected;
    private Sprite _mActive;
    private Sprite _mInActive;
    // Start is called before the first frame update
    void Start()
    {

        dayItems[0].mState = "collected";
        dayItems[1].mState = "collected";
        dayItems[2].mState = "active";
        dayItems[3].mState = "inactive";
        dayItems[4].mState = "inactive";
        dayItems[5].mState = "inactive";
        dayItems[6].mState = "inactive";
        dayItems[7].mState = "inactive";

        _mActive = Resources.Load<Sprite>("new_item/day_active");
        _mInActive = Resources.Load<Sprite>("new_item/day_inactive");
        _mCollected = Resources.Load<Sprite>("new_item/day_collected");
        _mDaypanCollected = Resources.Load<Sprite>("new_item/day_pan_collected");
        _mDaypanUnCollected = Resources.Load<Sprite>("new_item/day_pan_uncollected");
//
//
//        mAnnounce.text = "Announce View";
//        
        for (int i = 0; i < dayItems.Length; i++)
        {
            dayItems[i].mValue.text = "Day" + (i + 1).ToString();     
            dayItems[i].mPrice.text = ((i + 1) * 500).ToString();

            if (dayItems[i].mState == "collected")
            {
                dayItems[i].mPan.sprite = _mDaypanCollected;
                dayItems[i].mPic.sprite = _mCollected;
                dayItems[i].mPrice.color =  new Color32(0, 0, 0, 100);
                dayItems[i].mValue.color = new Color32(0, 0, 0, 100);
            }
            else if(dayItems[i].mState == "active")
            {
                dayItems[i].mPan.sprite = _mDaypanUnCollected;
                dayItems[i].mPic.sprite = _mActive;
            }
            else
            {
                dayItems[i].mPan.sprite = _mDaypanUnCollected;
                dayItems[i].mPic.sprite = _mInActive;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
        
        
    public void OnClose(GameObject obj)
    {
        obj.SetActive(false);
    }
}
