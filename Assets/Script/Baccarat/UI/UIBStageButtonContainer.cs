using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBStageButtonContainer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<UIBStageButton> m_stageButtonList;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void OnClickStage(enumGameTier m_gameTier, bool onlyView)
    {
        foreach (var btn in m_stageButtonList)
        {
            btn.isSelected = false;
        }
        m_stageButtonList.Where(x => x.m_gameTier == m_gameTier).First().isSelected = true;

        if (onlyView)   // This is for only view
        {
            BaccaratRoomController.Inst.OnClickRoomViewDialog((int)m_gameTier);
        }
        else    // This is for creating table
        {
            BaccaratRoomController.Inst.CreateBaccaratRoom_Type((int)m_gameTier);
        }

    }
}
