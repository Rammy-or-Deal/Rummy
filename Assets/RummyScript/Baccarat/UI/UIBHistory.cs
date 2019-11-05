using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        cells = layout.GetComponentsInChildren<UIBHistoryCell>();
        typeCnt = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        InitHistoryPan();
    }

    public void InitHistoryPan()
    {
        for (int i = 0; i < typeCnt.Length; i++)
        {
            typeCnt[i] = 0;
        }

        for (int i = 0; i < cells.Length; i++)
            cells[i].UpdateInfo(-1);

        num = 0;
    }

    public void OnClickShowBtn()
    {
        isShow = !isShow;
        layout.transform.parent.gameObject.SetActive(isShow);
    }

    public void AddCell(int type)
    {
        Debug.Log("Added Victory area. " + type);
        cells[num].UpdateInfo(type);

        typeCnt[type]++;
        num++;
    }

    public void ParseStatusString(string statusString)
    {
        statusString = statusString.Trim(',');
        InitHistoryPan();
        if(statusString == "") return;
        foreach (var status in statusString.Split(',').Select(Int32.Parse).ToArray())
        {
            AddCell(status);
        }
    }
}