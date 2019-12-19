using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Photon.Pun;
using UnityEngine;

public class UIEmojiBtn : MonoBehaviour
{
    public int id;
    public Animator animator;
    public void SetAnimator(int id0)
    {
        id = id0;
        animator.runtimeAnimatorController = Resources.Load("chat/"+Constants.EmojiAnimations[id]) as RuntimeAnimatorController;
    }

    public void OnClickBtn()
    {
        if (PhotonNetwork.InRoom)
        {
            object[] myCustomInitData = new object[]{id};
            PhotonNetwork.Instantiate("Prefabs/chat/emoji", Vector3.zero, Quaternion.identity, 0,myCustomInitData);
            UIController.Inst.chatDlg.gameObject.SetActive(false);
        }
    }
}
