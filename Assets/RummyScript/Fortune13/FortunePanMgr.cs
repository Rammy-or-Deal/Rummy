using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortunePanMgr : MonoBehaviour
{
    // Start is called before the first frame update
    public static FortunePanMgr Inst;
    public GameObject centerCard;
    void Start()
    {
        if(!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCardDistributed()
    {
        var playerList = FortunePlayMgr.Inst.m_playerList;
        centerCard.SetActive(true);
        foreach(var player in playerList)
        {
            player.InitCards();
            player.moveDealCard(centerCard.transform.position);
        }
    }
}
