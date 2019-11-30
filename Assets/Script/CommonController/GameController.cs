using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Inst;
    // Start is called before the first frame update
    void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }

    private void Start()
    {
        // Create Voice View Component  when joined Room.
        PhotonNetwork.Instantiate("Prefabs/VoiceView", Vector3.zero, Quaternion.identity, 0);
        
        UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        UIController.Inst.moneyPanel.gameObject.SetActive(false);
    }
    public virtual void SendMessage(int messageId, Player p = null)
    {
        
    }
}
