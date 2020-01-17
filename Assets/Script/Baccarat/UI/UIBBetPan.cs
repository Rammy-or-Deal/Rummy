using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBBetPan : MonoBehaviour
{
    public GameObject winObj;
    public Text val;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetPrize(int prize)
    {
        val.gameObject.SetActive(true);
        val.text = "+ " + prize;
    }

    internal void Init()
    {
        try{
        val.gameObject.SetActive(false);
        winObj.SetActive(false);
        }catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
