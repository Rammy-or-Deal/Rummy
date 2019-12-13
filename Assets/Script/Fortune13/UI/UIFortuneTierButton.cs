using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFortuneTierButton : MonoBehaviour
{
    // Start is called before the first frame update
    public enumGameTier m_Tier;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        FortuneTierController.Inst.OnClickTier(m_Tier);
    }
}
