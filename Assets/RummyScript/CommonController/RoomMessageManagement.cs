using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RoomMessageManagement : MonoBehaviour
{
    public static RoomMessageManagement Inst;

    [HideInInspector] public string prefix;
    public int GameID;
    // Start is called before the first frame update
    void Start()
    {
        if (!Inst)
            Inst = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMessageArrived(int message, Player player = null)
    {
        PlayerManagement.Inst.GameID = GameID;
        switch (message)
        {
            case (int)RoomManagementMessages.OnJoinSuccess:

                break;
            case (int)RoomManagementMessages.OnUserEnteredRoom_M:
                PlayerManagement.Inst.OnUserEnteredRoom_M(player.ActorNumber);
                break;
            case (int)RoomManagementMessages.OnRoomSeatUpdate:
                PlayerManagement.Inst.OnRoomSeatUpdate();
                break;
            case (int)RoomManagementMessages.OnUserLeave:
                PlayerManagement.Inst.OnUserLeave_M(player.ActorNumber);
                break;
        }
    }
}