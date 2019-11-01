using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBRoomItem : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public string tableName;
    [HideInInspector] public bool isPrivate;
    [HideInInspector] public int minBet;
    [HideInInspector] public int maxBet;
    [HideInInspector] public string status;
    [HideInInspector] public int playersNum;
    [HideInInspector] public int totalPlayers;
    [HideInInspector] public int roomNo;

    public Text UI_tableName;
    public Image UI_isPrivate;
    public Text UI_minBet;
    public Text UI_maxBet;
    public Button UI_status;
    public Text UI_players;
    public Button UI_join;

    #region  Unity
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    public void ShowMe()
    {
        UI_tableName.text = tableName;
        if (isPrivate)
        {
            UI_isPrivate.gameObject.SetActive(false);
        }else{
            UI_isPrivate.gameObject.SetActive(true);
        }
        UI_minBet.text = minBet.ToString();
        UI_maxBet.text = maxBet.ToString();
        UI_players.text = playersNum + " / " + totalPlayers;
    }

    internal void SetMe(BaccaratRoomInfo room)
    {
        
        
    }
}
