using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBBetBtnList : MonoBehaviour
{
    // Start is called before the first frame update
    public int selectedId = -1;
    public UIBBetBtn[] btns;
    public static UIBBetBtnList Inst;
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    public void OnClickBetBtn(int id)
    {
        btns[selectedId].UpdateStatus(false);
        selectedId = id;
        btns[selectedId].UpdateStatus(true);
    }
}
