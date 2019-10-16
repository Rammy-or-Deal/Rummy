using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UIAlertDialog : MonoBehaviour
{
    public static UIAlertDialog Inst;

    public Text mText;
    // Start is called before the first frame update

    private void Awake()
    {
        if (!Inst) Inst = this;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}