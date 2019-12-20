using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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
        StartCoroutine(WaitforUserSeat());
    }
    
    IEnumerator WaitforUserSeat()
    {
        yield return new WaitForSeconds(1);
        int actorNr = GetComponent<PhotonView>().OwnerActorNr;
        speakerImage.transform.SetParent(GameMgr.Inst.seatMgr.GetUserSeat(actorNr).transform);
        speakerImage.transform.localPosition=Vector3.zero;
    }
}
