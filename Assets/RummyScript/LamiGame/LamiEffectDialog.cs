using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageStatus
{
    Flush,Burnt,Game,Giveup,Ready,Set
}

public class LamiEffectDialog : MonoBehaviour
{
    
    public Image mImage;
    public static readonly string[] sprites = new string[] {"flush","burnt","game","giveup","ready","set"};
    public static LamiEffectDialog Inst;

    private void Start()
    {
        Inst = this;
    }

    public void ShowMessage(MessageStatus status)
    {
        mImage.sprite=Resources.Load<Sprite>("new_lami_txt/txt_"+sprites[(int)status]);
        mImage.GetComponent<EasyTween>().OpenCloseObjectAnimation();
    }
}
