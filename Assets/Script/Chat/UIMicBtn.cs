using System.Collections;
using System.Collections.Generic;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMicBtn : MonoBehaviour, IPointerClickHandler
{
    public GameObject muteImage;
    private bool isMuted;
    
    public void OnPointerDown()
    {
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = true;
    }
    
    public void OnPointerUp()
    {
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = false;
    }

    private float lastTimeClick;
    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData)
    {
        float currentTimeClick = eventData.clickTime;
        if(Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f){
            OnDoubleClick();
        }
        lastTimeClick = currentTimeClick;
//        int clickCount = eventData.clickCount;
//        if (clickCount == 1)
//            OnSingleClick();
//        else if (clickCount == 2)
//            
//        else if (clickCount > 2)
//            OnMultiClick();
    }

    void OnSingleClick()
    {
     
    }

    void OnDoubleClick()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0 : 1;
        muteImage.SetActive(isMuted);
    }
    
    void OnMultiClick()
    {
        
    }
}
