using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIReadyButton : MonoBehaviour
{
    // Start is called before the first frame update

    public static UIReadyButton Inst;
    public bool isIamReady
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            //isIamReady = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickReady()
    {
        GameMgr.Inst.m_playerStatus = enumPlayerStatus.Init_Ready;

        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_InitReady},            
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        isIamReady = false;        
    }
}
