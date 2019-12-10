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
        
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) //called before Start()
    {
        object[] instantiationData = info.photonView.InstantiationData;
        int id = (int) instantiationData[0];
        
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load("chat/"+animationList[id]) as RuntimeAnimatorController;
        gameObject.transform.parent = UIController.Inst.chatDlg.transform.parent;
        transform.localPosition=Vector3.zero;
        
        Debug.Log("created: actorNumber:"+ info.photonView.ControllerActorNr);
        Debug.Log("created: actorNumber:"+ info.photonView.OwnerActorNr);
        Debug.Log("created: actorNumber:"+ info.photonView.CreatorActorNr);
        
        Debug.Log("created: actorNumber:"+ GetComponent<PhotonView>().ControllerActorNr);
        Debug.Log("created: actorNumber:"+ GetComponent<PhotonView>().OwnerActorNr);
        Debug.Log("created: actorNumber:"+ GetComponent<PhotonView>().CreatorActorNr);
    }
}
