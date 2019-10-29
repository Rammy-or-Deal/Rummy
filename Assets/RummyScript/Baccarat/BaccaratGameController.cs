using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratGameController : MonoBehaviour
{
    public static BaccaratGameController Inst;
    
    void Awake()
    {
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }
    void Start()
    {
        // UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        // UIController.Inst.moneyPanel.gameObject.SetActive(false);
        // seatNumList = new Dictionary<int, int>();
        // ShowPlayers();
    }


    public void SendMessage(int messageId, Player p = null)
    {
        BaccaratMessageMgr.Inst.OnMessageArrived(messageId, p);
    }



    public void ShowPlayers()
    {
        // int id = 0;

        // foreach (Player p in PhotonNetwork.PlayerList)
        // {
        //     Debug.Log("ShowPlayers: " + p);
        //     BaccaratUserSeat entry;
        //     //int mySeatId = ;
        //     Debug.Log("MyseatID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        //     if (p.NickName == DataController.Inst.userInfo.name)
        //     {
        //         entry = userSeats[0];
        //     }
        //     else
        //     {
        //         id++;
        //         entry = userSeats[id];
        //     }

        //     entry.Show(p);
        // }
    }
    public void NewPlayerEnteredRoom(Player newPlayer)
    {
        // Debug.Log("NewPlayerEnteredRoom");
        // foreach (BaccaratUserSeat entry in userSeats)
        // {
        //     if (!entry.isSeat)
        //     {
        //         entry.Show(newPlayer);
        //         return;
        //     }
        // }
    }

    public void OtherPlayerLeftRoom(Player otherPlayer)
    {
        // BaccaratUserSeat entry = userSeats[seatNumList[otherPlayer.ActorNumber]];
        // seatNumList.Remove(otherPlayer.ActorNumber);
        // entry.LeftRoom();
    }

    public void PlayerPropertiesUpdate(Player otherPlayer, Hashtable changedProps)
    {
        // object playerPic;
        // if (otherPlayer.CustomProperties.TryGetValue(Common.PLAYER_PIC, out playerPic))
        // {
        //     BaccaratUserSeat entry = userSeats[seatNumList[otherPlayer.ActorNumber]];
        //     entry.Show(otherPlayer);
        // }
    }

}
