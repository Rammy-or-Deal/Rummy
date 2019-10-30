using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBCard : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public Image image;
    
    void Start()
    {
        image = GetComponent<Image>();   
    }
}
