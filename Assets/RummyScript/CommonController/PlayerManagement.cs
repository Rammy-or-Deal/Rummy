using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement Inst;

    // Start is called before the first frame update
    public List<CommonSeat> m_playerList = new List<CommonSeat>();

    public int GameID { get; internal set; }

    [HideInInspector] public Dictionary<int, int> seatNumList;
    void Start()
    {
        if (!Inst)
        {
            Inst = this;
            seatNumList = new Dictionary<int, int>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public string GetMessageString()
    {
        string res = "";

        switch (GameID)
        {
            case (int)Game_Identifier.Fortune14:
                res = Common.FORTUNE_MESSAGE;
                break;
            case (int)Game_Identifier.Lami:
                res = Common.LAMI_MESSAGE;
                break;
            case (int)Game_Identifier.Baccarat:
                res = Common.BACCARAT_MESSAGE;
                break;
        }
        return res;
    }

    internal void OnUserLeave_M(int actorNumber)
    {
        List<RoomManagement_Seat> seatList = getSeatList();
        int posOfplayer = -1;
        try
        {
            posOfplayer = seatList.Where(x => x.actorNumber == actorNumber).Select(x => x.seatNo).First();
        }
        catch
        {
            return;
        }
        seatList.RemoveAt(posOfplayer);

        string seatString = string.Join(",", seatList);
        Hashtable props = new Hashtable{
            {GetMessageString(), RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
    }

    public List<RoomManagement_Seat> getSeatList()
    {
        
        string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        Debug.Log("getSeatList function called.seatString:=" + seatString);
        List<RoomManagement_Seat> seatList = new List<RoomManagement_Seat>();
        foreach (var str in seatString.Split(','))
        {
            RoomManagement_Seat seat = new RoomManagement_Seat(str);
            seatList.Add(seat);
        }
        return seatList;
    }

    internal void OnRoomSeatUpdate()
    {
        List<RoomManagement_Seat> seatList = getSeatList();

        // Make seatNumList
        seatNumList.Clear();
        foreach (var seat in seatList)
        {
            seatNumList.Add(seat.actorNumber, seat.seatNo);
        }

        // Make all players to hide.
        for (int i = 0; i < m_playerList.Count; i++)
        {
            m_playerList[i].IsSeat = false;
        }

        // Hide/Show the players by seatList
        foreach (var seat in seatList)
        {
            m_playerList[GetUserSeat(seat.seatNo)].SetProperty(seat.actorNumber);
        }

        // Send User Sit Message to master
        if (!PhotonNetwork.IsMasterClient) return;
        Hashtable table = new Hashtable { { GetMessageString(), (int)RoomManagementMessages.OnUserSit } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    internal void OnUserEnteredRoom_M(int actorNumber)
    {
        string seatString = "";

        try
        {
            seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_STRING];
        }
        catch
        {
            seatString = "";
        }
        int maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;

        if (seatString == "" || seatString == null)    // This is new room.
        {
            //seatString += actorNumber + ":0";
            RoomManagement_Seat seat = new RoomManagement_Seat();
            seat.actorNumber = actorNumber;
            seat.seatNo = 0;
            seat.status = 0;
            seatString = seat.seatString;
        }
        else
        {
            List<RoomManagement_Seat> seatList = getSeatList();

            int omittedSeat = -1;
            for (int i = 0; i < maxPlayer; i++)
            {
                if (seatList.Count(x => x.seatNo == i) == 0)
                {
                    omittedSeat = i;
                    break;
                }
            }

            if (omittedSeat == -1)   // means, there's no seat. so check bot seat and replace it with bot's seat
            {
                if (actorNumber < 0) return; // If this is also bot, return;

                var bot = seatList.Where(x => x.actorNumber < -10).First();
                // Send Bot removed Message
                Hashtable hashtable = new Hashtable
                {
                    {GetMessageString(), RoomManagementMessages.OnBotRemoved},
                    {Common.BOT_ID, bot.actorNumber}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                bot.actorNumber = actorNumber;
            }
            else    // If there's empty seat, add new seat;
            {
                RoomManagement_Seat seat = new RoomManagement_Seat();
                seat.actorNumber = actorNumber;
                seat.seatNo = omittedSeat;
                seatList.Add(seat);
            }

            // Make SeatString
            seatString = string.Join(",", seatList.Select(x => x.seatString));
        }
        Debug.Log("Send OnRoomSeatUpdate: " + seatString);
        Hashtable props = new Hashtable{
            {GetMessageString(), RoomManagementMessages.OnRoomSeatUpdate},
            {Common.SEAT_STRING, seatString},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private int GetUserSeat(int seatNo_in_seatString)
    {
        int seatPos;
        if (seatNo_in_seatString == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = 0;
        }
        else if (seatNo_in_seatString > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = seatNo_in_seatString - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
        }
        else
        {
            seatPos = 4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNo_in_seatString;
        }

        return seatPos;
    }
}
public class RoomManagement_Seat
{
    public int seatNo;
    public int actorNumber;
    public int status;
    private string str;

    public RoomManagement_Seat() { }
    public RoomManagement_Seat(string str)
    {
        seatString = str;
    }

    public string seatString
    {
        get
        {
            return actorNumber + ":" + seatNo + ":" + status;
        }
        set
        {
            var tmp = value.Split(':').Select(Int32.Parse).ToArray();
            actorNumber = tmp[0];
            seatNo = tmp[1];
            status = tmp[2];
        }
    }
}
