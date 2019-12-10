using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBStageButton : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject coverImage;
    public enumGameTier m_gameTier;
    public bool isOnlyView;
    public UIBStageButtonContainer parent;
    public bool isSelected{
        get{ return coverImage.activeSelf;}
        set{ coverImage.SetActive(value);}
    }
    void Start()
    {
        if(m_gameTier == enumGameTier.BaccaratRegular)
        {
            isSelected = true;
            //coverImage.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickStage()
    {
        GameMgr.Inst.m_gameTier = m_gameTier;
        parent.OnClickStage(m_gameTier, isOnlyView);
    }
}
