using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollectionFrameItem : MonoBehaviour
{
    public int id;
    public Image pic;
    public new Text name;
    public Image frame;
    public string stats;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickBuy()
    {
        Debug.Log(id+" item clicked ");
        UIController.Inst.noticeDlg.gameObject.SetActive(true);
    }
}
