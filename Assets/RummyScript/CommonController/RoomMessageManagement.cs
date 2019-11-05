using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RoomMessageManagement : MonoBehaviour
{
    public static RoomMessageManagement Inst;
    
    [HideInInspector]public string prefix;
    public int GameID;
    // Start is called before the first frame update
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMessageArrived(string message, Player player=null)
    {
        
    }
}
