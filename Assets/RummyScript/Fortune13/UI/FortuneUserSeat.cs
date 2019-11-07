using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class FortuneUserSeat : MonoBehaviour
{
    public GameObject MyObject;
    public Image mUserFrame;
    public Image mUserPic;
    public Image mUserSkillPic;
    public Text mUserName;
    public Text mUserSkillName;
    public Text mCoinValue;
    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    internal void InitCards()
    {
        foreach(var card in myCards)
        {
            card.Init(false);
        }
    }

    internal async void moveDealCard(Vector3 srcPos)
    {
        foreach(var card in myCards)
        {
            await Task.Delay(500);
            card.moveDealCard(srcPos);
        }
    }

    //seat state
    //

    public int actorNumber;
    public bool isSeat;
    #region UNITY   

    public void Start()
    {
        
        mUserPic.sprite = Resources.Load<Sprite>(DataController.Inst.userInfo.pic);
        mUserName.text = DataController.Inst.userInfo.name;
        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
        mUserSkillName.text = DataController.Inst.userInfo.skillLevel;
        mCoinValue.text = DataController.Inst.userInfo.coinValue.ToString();
        _status = 0;
        _infostring = "";
        GameObject gameObject1 = this.gameObject;
        MyObject = gameObject1;
    }


    public void LeftRoom() // the number of left user
    {
        isSeat = false;
    }


    #endregion


    public void OnClick()
    {
        UIController.Inst.userInfoMenu.gameObject.SetActive(true);
    }

    #region  Common Property.

    [HideInInspector]
    public bool IsSeat
    {
        get { return MyObject.gameObject.activeSelf; }
        set
        {
            isSeat = value;
            MyObject.gameObject.SetActive(value);
        }
    }

    public int _status;
    [HideInInspector]
    public int Status
    {
        get { return _status; }
        set
        {
            _status = value;
            if (_infostring != "")
            {
                var tmp = _infostring.Split(',');
                tmp[tmp.Length - 1] = value.ToString();
                _infostring = string.Join(",", tmp);
            }
        }
    }

    string _infostring;

    [HideInInspector]
    public string infoString
    {
        get
        {
            return _infostring;
        }
        set
        {
            _infostring = value;
            if (value == "") return;

            var tmp = value.Split(':');
            actorNumber = int.Parse(tmp[0]);
            mUserName.text = tmp[1];
            mUserPic.sprite = Resources.Load<Sprite>((string)tmp[2]);
            mCoinValue.text = tmp[3];
            //mUserSkillPic.sprite = Resources.Load<Sprite>((string)tmp[4]);
            //mUserFrame.sprite = Resources.Load<Sprite>((string)tmp[5]);            
            _status = int.Parse(tmp[6]);
        }
    }
    public void PublishMe()
    {
        foreach (Player p in PhotonNetwork.PlayerList)  // If this isn't a bot
        {
            if (actorNumber == p.ActorNumber)
            {
                Hashtable table = new Hashtable{{Common.PLAYER_INFO, infoString}};
                p.SetCustomProperties(table);
                return;
            }
        }
    }
    internal void SetProperty(int actorNumber)
    {
        bool isTaken = false;
        if (actorNumber >= 0)
        {
            foreach (Player p in PhotonNetwork.PlayerList)  // If this isn't a bot
            {
                if (actorNumber == p.ActorNumber)
                {
                    infoString = (string)p.CustomProperties[Common.PLAYER_INFO];
                    isTaken = true;
                    break;
                }
            }
        }
        else
        {
            try
            {
                infoString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.NEW_BOT_INFO];
                isTaken = true;
            }
            catch { }
        }
        if (isTaken)
        {
            IsSeat = true;
        }
        else
        {
            IsSeat = false;
        }
    }


    #endregion

}
