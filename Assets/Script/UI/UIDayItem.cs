using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDayItem : MonoBehaviour
{
    public int mId;  
    public Text mValue;
    public Image mPic;
    public Image mPan;
    public string mState;
    public Text mPrice;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickItem(GameObject obj)
    {
        if (mState == "active")
        {
            obj.SetActive(true);
        }
    }
}
