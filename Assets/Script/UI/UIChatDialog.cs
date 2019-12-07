using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;
using UnityEngine.UI;

public class UIChatDialog : MonoBehaviour
{
    public InputField InputFieldChat; // set in inspector
    public Text CurrentChannelText; // set in inspector
    
    // Start is called before the first frame update
    void Start()
    {
        ChatMgr.Inst.ShowChannel("Lobby");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            ChatMgr.Inst.SendChatMessage(this.InputFieldChat.text);
            this.InputFieldChat.text = "";
        }
    }

    public void OnClickSend()
    {
        if (this.InputFieldChat != null)
        {
            ChatMgr.Inst.SendChatMessage(this.InputFieldChat.text);
            this.InputFieldChat.text = "";
        }
    }

    public void OnClose(GameObject obj)
    {
        obj.SetActive(false);
    }
}