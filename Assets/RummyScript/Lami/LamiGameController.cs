using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.RummyScript.LamiGame;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.Cockpit;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LamiGameController : MonoBehaviour
{
    public static LamiGameController Inst;

    //Game
    public LamiUserSeat[] userSeats;
    public Dictionary<int, int> seatNumList;
    public bool isStartedGame = false;
    private int botWaitTime = 15;

    #region UNITY

    void Awake()
    {
        return;
        if (!DataController.Inst)
            SceneManager.LoadScene("2_Lobby");

        if (!Inst)
            Inst = this;
    }

    void Start()
    {
        return;
        UIController.Inst.userInfoPanel.gameObject.SetActive(false);
        UIController.Inst.moneyPanel.gameObject.SetActive(false);
        seatNumList = new Dictionary<int, int>();
//        ShowPlayers();
        LamiCountdownTimer.Inst.StartTimer();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartCoroutine(CreateBot());
        }
    }

    #endregion UINITY

    #region Creating Bot

    public IEnumerator CreateBot()
    {
        botWaitTime = UnityEngine.Random.RandomRange(5, 10);

        while (true)
        {
            yield return new WaitForSeconds(botWaitTime);

            string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
            var seatArray = seatString.Split(',');
            var idList = new List<int>();
            for (int j = 0; j < seatArray.Length; j++)
            {
                var tmp = seatArray[j].Split(':');
                idList.Add(int.Parse(tmp[1]));
            }
            for (int j = 0; j < 4; j++)
            {
                if (!idList.Contains(j))
                {
                    AddBot(j);
                    break;
                }
            }
        }
    }

    public void AddBot(int user_id)
    {
        LamiBot mBot = new LamiBot();
        mBot.Init();

        // Add SEAT_ID to currentRoom property
        var seat_id_string = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
        while (seat_id_string.Contains("-" + mBot.id))
        {
            mBot.id = UnityEngine.Random.Range(1000, 9999);
        }
        seat_id_string += ",-" + mBot.id + ":" + user_id;

        // Publish bot lists to BOT_LIST_STRING
        string botString = "";
        try
        {
            botString =(string) PhotonNetwork.CurrentRoom.CustomProperties[Common.BOT_LIST_STRING];
        }
        catch { }
        
        botString += string.Format(",{0}:{1}:{2}:{3}:{4}:{5}",
            -mBot.id, mBot.skillLevel, mBot.pic, mBot.coinValue, mBot.name, user_id);
        botString = botString.Trim(',');

        // Send bot info to server
        Hashtable props = new Hashtable
        {
           {Common.eventID_room, 22 },
           {Common.BOT_LIST_STRING, botString },
           {Common.SEAT_ID, seat_id_string }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    List<LamiBot> botList = new List<LamiBot>();

    public void OnBotListChanged(Hashtable properties)
    {
        botList.Clear();
        object value;
        if (properties.TryGetValue(Common.BOT_LIST_STRING, out value))
        {
            string botString = (string)value;

            var bots = botString.Split(',');
            for(int i = 0; i < bots.Length; i++)
            {
                LamiBot mBot = new LamiBot();
                mBot.Init();

                var items = bots[i].Split(':');
                
                mBot.id = int.Parse(items[0]);
                mBot.skillLevel = items[1];
                mBot.pic = items[2];
                mBot.coinValue = int.Parse(items[3]);
                mBot.name = items[4];
                int user_seat = int.Parse(items[5]);

                var botUserSeat = userSeats[GetUserSeat(user_seat)];
                botUserSeat.mUserPic.sprite = Resources.Load<Sprite>(mBot.pic);
                botUserSeat.mUserName.text = mBot.name;
                botUserSeat.mUserSkillName.text = mBot.skillLevel;
                botUserSeat.mCoinValue.text = mBot.coinValue.ToString();
                botUserSeat.gameObject.SetActive(true);
                botList.Add(mBot);                
            }           
        }

        //LamiBot mBot1 = new LamiBot();
        //mBot.Init();
        
        //if (properties.TryGetValue(Common.PLAYER_LEVEL, out value)) mBot.skillLevel = (string)value;
        //if (properties.TryGetValue(Common.PLAYER_PIC, out value)) mBot.pic = (string)value;
        //if (properties.TryGetValue(Common.PLAYER_COIN, out value)) mBot.coinValue = (int)value;
        //if (properties.TryGetValue(Common.PLAYER_NAME, out value)) mBot.name = (string)value;
        //if (properties.TryGetValue(Common.BOT_ID, out value))
        //{
        //    var botUserSeat = userSeats[GetUserSeat((int)value)];
        //    botUserSeat.mUserPic.sprite = Resources.Load<Sprite>(mBot.pic);
        //    botUserSeat.mUserName.text = mBot.name;
        //    botUserSeat.mUserSkillName.text = mBot.skillLevel;
        //    botUserSeat.mCoinValue.text = mBot.coinValue.ToString();
        //    botUserSeat.gameObject.SetActive(true);

        //    string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
        //    while (seatString.Contains("-" + mBot.id))
        //    {
        //        mBot.id = UnityEngine.Random.Range(1000, 9999);
        //    }
        //    seatString += ",-" + mBot.id + ":" + (int)value;
        //    Hashtable props = new Hashtable{
        //        {Common.SEAT_ID, seatString },
        //        {Common.eventID_room, 21}
        //    };

        //    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        //}
    }
    // Bot Event
    public void OnBotAdd(Hashtable properties)
    {
        LamiBot mBot = new LamiBot();
        mBot.Init();
        object value;
        if (properties.TryGetValue(Common.PLAYER_LEVEL, out value)) mBot.skillLevel = (string)value;
        if (properties.TryGetValue(Common.PLAYER_PIC, out value)) mBot.pic = (string)value;
        if (properties.TryGetValue(Common.PLAYER_COIN, out value)) mBot.coinValue = (int)value;
        if (properties.TryGetValue(Common.PLAYER_NAME, out value)) mBot.name = (string)value;
        if (properties.TryGetValue(Common.REMOVED_BOT_ID, out value))
        {
            var botUserSeat = userSeats[GetUserSeat((int)value)];
            botUserSeat.mUserPic.sprite = Resources.Load<Sprite>(mBot.pic);
            botUserSeat.mUserName.text = mBot.name;
            botUserSeat.mUserSkillName.text = mBot.skillLevel;
            botUserSeat.mCoinValue.text = mBot.coinValue.ToString();
            botUserSeat.gameObject.SetActive(true);

            string seatString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
            while(seatString.Contains("-"+ mBot.id))
            {
                mBot.id = UnityEngine.Random.Range(1000, 9999);
            }
            seatString += ",-"+mBot.id+":" + (int)value;
            Hashtable props = new Hashtable{
                {Common.SEAT_ID, seatString },
                {Common.eventID_room, 21}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

    }
    #endregion

    #region GAME

    public void StartLamiGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        Debug.Log("Game started");
        isStartedGame = true;
        CardManager.Inst.GenerateCard();
    }


    public void ShowPlayers()
    {
        int id = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log("ShowPlayers: " + p);
            LamiUserSeat entry;
            //int mySeatId = ;
            Debug.Log("MyseatID: " + PhotonNetwork.LocalPlayer.ActorNumber);
            if (p.NickName == DataController.Inst.userInfo.name)
            {
                entry = userSeats[0];
            }
            else
            {
                id++;
                entry = userSeats[id];
            }

            entry.Show(p);
        }
    }

    #endregion GAME

    public void NewPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("NewPlayerEnteredRoom");
//        foreach (LamiUserSeat entry in userSeats)
//        {
//            if (!entry.isSeat)
//            {
//                entry.Show(newPlayer);
//                return;
//            }
//        }
    }

    public void OtherPlayerLeftRoom(Player otherPlayer)
    {
        LamiUserSeat entry = GetUserSeat(otherPlayer);
        seatNumList.Remove(otherPlayer.ActorNumber);
        entry.LeftRoom();

        Hashtable turnProps = new Hashtable();
        object seatId = PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
        string commonSeatId = "";

        Debug.Log(seatNumList.ToStringFull());
        List<string> temp = new List<string>();
        List<KeyValuePair<int, int>> sorted = (from kv in seatNumList orderby kv.Value select kv).ToList();
        foreach (KeyValuePair<int, int> kv in sorted)
        {
            temp.Add(kv.Key.ToString() + ":" + kv.Value.ToString());
        }

        commonSeatId = string.Join(",", temp);
        Debug.Log(commonSeatId);
        turnProps[Common.SEAT_ID] = commonSeatId;        
        PhotonNetwork.CurrentRoom.SetCustomProperties(turnProps);
    }

    public bool CheckAllPlayersReady()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(Common.PLAYER_STATUS, out isPlayerReady))
            {
                if (!(bool) isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (PhotonNetwork.PlayerList.Count() != 2)
        {
            Debug.Log("PhotonNetwork.PlayerList.Count : " + PhotonNetwork.PlayerList.Count() + " ,so return false");
            return false;
        }

        return true;
    }

    public void OnPlayerReadyClicked(Player otherPlayer, Hashtable props)
    {
        if (PhotonNetwork.IsMasterClient && !isStartedGame && CheckAllPlayersReady())
        {
            Debug.Log("All players ready! start game");
            StartLamiGame();
        }

        LamiUserSeat entry = GetUserSeat(otherPlayer);
        entry.Show(otherPlayer);
    }

    public void PlayerPropertiesUpdate(Player otherPlayer, Hashtable changedProps)
    {
        Debug.Log(changedProps);
        object isPlayerReady;
        object cardList;


        if (changedProps.TryGetValue(Common.PLAYER_STATUS, out isPlayerReady))
        {
            Debug.Log("CustomProperties.TryGetValue(PLAYER_READY : " + isPlayerReady);
            if (PhotonNetwork.IsMasterClient && !isStartedGame && CheckAllPlayersReady())
            {
                Debug.Log("All players ready! start game");
                StartLamiGame();
            }

            LamiUserSeat entry = GetUserSeat(otherPlayer);

            entry.Show(otherPlayer);
        }

        if (changedProps.TryGetValue(Common.PLAYER_CARD_List, out cardList))
        {
            Debug.Log("cardList" + (string) cardList);
            int seat_id = seatNumList[otherPlayer.ActorNumber];
            Card[] cards =
                CardManager.Inst.ReceivedCardList(seat_id, CardManager.ConvertCardStrToCardList((string) cardList));

            if (seat_id == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
            {
                LamiGameUIManager.Inst.myCardPanel.InitCards(cards);
                LamiGameUIManager.Inst.InitButtonsFirst();

                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("IsMasterClient");
                    LamiGameUIManager.Inst.playButton.gameObject.SetActive(true);
                    LamiGameUIManager.Inst.playButton.interactable = false;
                    LamiGameUIManager.Inst.tipButton.gameObject.SetActive(true);
                    GetUserSeat(PhotonNetwork.LocalPlayer).mClock.SetActive(true);
                    LamiCountdownTimer.Inst.StartTurnTimer();
                }
            }

            LamiUserSeat seat = GetUserSeat(otherPlayer);
            seat.mCardNum.text = cards.Length.ToString();
            Debug.Log("seat.mCardNum.text=" + cards.Length.ToString());
            seat.playerReadyImage.gameObject.SetActive(false);
            seat.mAceJokerPanel.SetActive(true);
            seat.mAceValue.text = CardManager.Inst.GetACount(seat_id).ToString();
            seat.mJokerValue.text = CardManager.Inst.GetJokerCount(seat_id).ToString();
        }
    }


    public void RoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        object isGameStarted;
        object seatId;
        object value;
        if (propertiesThatChanged.TryGetValue(Common.Game_START, out isGameStarted))
        {
        }

        if(propertiesThatChanged.TryGetValue(Common.REMOVED_BOT_ID, out value))
        {
            int removed_bot_id = (int)value;
            for(int i = 0; i < botList.Count; i++)
            {
                // remove bot in BotList
                if(botList[i].id == removed_bot_id)
                {
                    botList.Remove(botList[i]);
                }
            }
        }
        if (propertiesThatChanged.TryGetValue(Common.SEAT_ID, out seatId))
        {
            seatNumList.Clear();
            Debug.Log("RoomPropertiesUpdate: " + (string) seatId);

            seatId = PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID];
            string[] str = seatId.ToString().Split(',');
            Dictionary<int, int> seatInfo = new Dictionary<int, int>();

            for (int i = 0; i < str.Length; i++)
            {
                int[] seat = str[i].Split(':').Select(Int32.Parse).ToArray();
                Debug.Log(seat[0].ToString() + "/" + seat[1].ToString());

                seatNumList.Add(seat[0], seat[1]);
            }

            Debug.Log(seatNumList.ToStringFull());
            
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log(p.ActorNumber + "//" + p.NickName);
                LamiUserSeat userSeat = GetUserSeat(p);               
                userSeat.Show(p);
            }
            for (int i = 0; i < str.Length; i++)
            {
                var tmp = int.Parse(str[i].Split(':')[1]);
                userSeats[GetUserSeat(tmp)].gameObject.SetActive(true);
            }
        }
    }

    public LamiUserSeat GetUserSeat(Player p)
    {
        LamiUserSeat userSeat;

        Debug.Log("GetUserSeat; seat_id:" + PhotonNetwork.CurrentRoom.CustomProperties[Common.SEAT_ID]);
        Debug.Log("GetUserSeat; seatNumList:" + seatNumList.ToStringFull());
        
        if (seatNumList[p.ActorNumber] == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            Debug.Log(p.NickName + "///" + p.ActorNumber);
            userSeat = userSeats[0];
        }
        else if (seatNumList[p.ActorNumber] > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            userSeat = userSeats[seatNumList[p.ActorNumber] - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber]];
        }
        else
        {
            userSeat = userSeats[4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + seatNumList[p.ActorNumber]];
        }

        return userSeat;
    }

    public int GetUserSeat(int id)
    {
        int seatPos;
        if (id == seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = 0;
        }
        else if (id > seatNumList[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            seatPos = id - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber];
        }
        else
        {
            seatPos = 4 - seatNumList[PhotonNetwork.LocalPlayer.ActorNumber] + id;
        }

        return seatPos;
    }
}