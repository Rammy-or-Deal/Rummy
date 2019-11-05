using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICalcDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}
