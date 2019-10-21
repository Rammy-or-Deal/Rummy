using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingDialog : MonoBehaviour
{
    public Image loadingBar;
    public int zSpeed = 100;

    // Update is called once per frame
    void Start () {
        // Initialize onProgressComplete and set a basic callback
    }
    
   public void Update()
    {
        loadingBar.transform.Rotate(
            0,0,
            -(zSpeed * Time.deltaTime)
        );
    }
}
