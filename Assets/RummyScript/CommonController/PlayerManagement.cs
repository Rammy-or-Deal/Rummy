using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement Inst;

    // Start is called before the first frame update
    public List<MonoBehaviour> m_playerList = new List<MonoBehaviour>();

    public int GameID { get; internal set; }

    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void OnUserEnteredRoom_M(int actorNumber)
    {
        
    }
}
