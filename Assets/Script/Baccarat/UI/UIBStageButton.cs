using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBStageButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Image coverImage;
    public enumGameTier m_gameTier;
    public bool isOnlyView;
    public UIBStageButtonContainer parent;
    private bool _isSelected = false;
    public bool isSelected{
        get{ return _isSelected;}
        set{ 
            _isSelected = value;
            string imgPath = "";
            if(_isSelected)
            {
                imgPath = "baccarat/LVL/btn0";                
            }
            else
            {
                imgPath = "baccarat/LVL/btn1";
            }
            coverImage.sprite = Resources.Load<Sprite>(imgPath);
            Debug.Log(imgPath);
            //coverImage.SetActive(value);
        }
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
        if (m_gameTier != enumGameTier.BaccaratCreateRoom)
            GameMgr.Inst.m_gameTier = m_gameTier;
        parent.OnClickStage(m_gameTier, isOnlyView);
    }
}
