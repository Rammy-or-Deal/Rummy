using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBMessage : MonoBehaviour
{
    public Text text;
    void Start()
    {

    }

    public void Show(string str)
    {
        text.text = str;
        gameObject.SetActive(true);
        GetComponent<EasyTween>().OpenCloseObjectAnimation();
    }

    public void Hide()
    {
        try
        {
            GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        catch { }
    }

    public void OnExitAnimation()
    {
        gameObject.SetActive(false);
    }
}
