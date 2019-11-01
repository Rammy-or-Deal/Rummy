using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBHistory : MonoBehaviour
{
    public static UIBHistory Inst;
    public GameObject layout;
    public UIBHistoryCell[] cells;
    public int[] typeCnt;
    public int num;
    private bool isShow = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Inst)
            Inst = this;
        else
        {
            Destroy	(this.gameObject);
        }
    }

    void Start()
    {
        cells=layout.GetComponentsInChildren<UIBHistoryCell>();
    }

    public void OnClickShowBtn()
    {
        isShow = !isShow;
        layout.transform.parent.gameObject.SetActive(isShow);
    }

    public void AddCell(int type)
    {
        cells[num].UpdateInfo(type);
        typeCnt[type]++;
        num++;
    }
}
