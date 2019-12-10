using System.Collections;
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
        StartCoroutine(Destroy());
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) //called before Start()
    {
        object[] instantiationData = info.photonView.InstantiationData;
        int id = (int) instantiationData[0];
        
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load("chat/"+animationList[id]) as RuntimeAnimatorController;
        gameObject.transform.parent = GameMgr.Inst.seatMgr.GetUserSeat(GetComponent<PhotonView>().OwnerActorNr).transform;
        transform.localPosition=Vector3.zero;
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.Destroy(gameObject);
    }
}
