using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBBetBtn : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public GameObject coverObj;
    public UIBBetBtnList parent;
    void Start()
    {
        
    }

    public void UpdateStatus(bool isClicked)
    {
        coverObj.SetActive(isClicked);
        int size = isClicked ? 90 : 70;
        GetComponent<RectTransform>().sizeDelta=new Vector2	(size,size);
    }
    
    public void OnClickBetBtn()
    {
        parent.OnClickBetBtn(id);
    }
}
