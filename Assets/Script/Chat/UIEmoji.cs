using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIEmoji : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        Debug.Log("created");
        
    }
}
