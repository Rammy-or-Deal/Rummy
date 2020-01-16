using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiTierButton : MonoBehaviour
{
    // Start is called before the first frame update
    public enumGameTier m_Tier;
    
    public void OnClick()
    {
        LamiTierController.Inst.OnClickTier(m_Tier);
    }
}
