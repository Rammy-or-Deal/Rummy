using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCloseBtn()
    {
        this.gameObject.SetActive(false);
    }

}
