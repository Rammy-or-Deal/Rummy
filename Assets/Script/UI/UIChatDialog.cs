using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UIChatDialog : MonoBehaviour
{
    public InputField InputFieldChat; // set in inspector
    public Text CurrentChannelText; // set in inspector
    public GameObject emojiModule;
    public GameObject messageModule;
    
    // Start is called before the first frame update
    void Start()
    {
        ChatMgr.Inst.ShowChannel("Lobby");
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

    public void OnEmoji(bool isShow)
    {
        emojiModule.SetActive(isShow);
        messageModule.SetActive(!isShow);
    }

    public void OnClickEmoji(int id)
    {
        if (PhotonNetwork.InRoom)
        {
            object[] myCustomInitData = new object[]{id};
            PhotonNetwork.Instantiate("Prefabs/chat/emoji", Vector3.zero, Quaternion.identity, 0,myCustomInitData);    
        }
    }
}