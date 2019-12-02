using Photon.Voice.PUN;
using UnityEngine;

public class GameUIManager: MonoBehaviour
{
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
}