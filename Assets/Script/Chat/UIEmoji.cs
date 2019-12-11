﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIEmoji : MonoBehaviour, IPunInstantiateMagicCallback
{
    private string[] animationList = {"cow_cry","cow_pray","cow_taunt"};
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) //called before Start()
    {
        object[] instantiationData = info.photonView.InstantiationData;
        int id = (int) instantiationData[0];
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load("chat/"+animationList[id]) as RuntimeAnimatorController;
        gameObject.transform.SetParent(GameMgr.Inst.seatMgr.GetUserSeat(GetComponent<PhotonView>().OwnerActorNr).transform);
        transform.localPosition=Vector3.zero;
        transform.localScale=Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (GetComponent<PhotonView>().IsMine)
            StartCoroutine(Destroy());
    }
}
