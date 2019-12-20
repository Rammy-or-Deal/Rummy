using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

public class UIVoiceView : MonoBehaviour, IPunInstantiateMagicCallback
{
    public Image speakerImage;
    private PhotonVoiceView photonVoiceView;
    
    void Start()
    {
        photonVoiceView = GetComponentInParent<PhotonVoiceView>();
    }

    // Update is called once per frame
    void Update()
    {
        speakerImage.enabled = photonVoiceView.IsSpeaking;
    }
    
    public void OnPhotonInstantiate(PhotonMessageInfo info) //called before Start()
    {
        speakerImage.transform.SetParent(GameMgr.Inst.seatMgr.GetUserSeat(GetComponent<PhotonView>().OwnerActorNr).transform);
        speakerImage.transform.localPosition=Vector3.zero;
    }
}
