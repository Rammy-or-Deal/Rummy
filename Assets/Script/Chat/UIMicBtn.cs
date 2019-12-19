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
        Debug.Log("OnPointerDown");
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = true;
    }
    
    public void OnPointerUp()
    {
        Debug.Log("OnPointerUp");
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = false;
    }
    
    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;
        if (clickCount == 1)
            OnSingleClick();
        else if (clickCount == 2)
            OnDoubleClick();
        else if (clickCount > 2)
            OnMultiClick();
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
