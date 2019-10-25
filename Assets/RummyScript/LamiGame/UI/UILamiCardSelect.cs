using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiCardSelect : MonoBehaviour
{
    public int id;
    // Start is called before the first frame update
    
    public void OnClickList()
    {
        LamiGameUIManager.Inst.OnSelectedCardList(id);
        LamiGameUIManager.Inst.uiSelectCardList.Hide();
    }

}
