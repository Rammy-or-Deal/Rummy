using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UIFCalcPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<FortuneCard> cardList;
    public Image levelImg;
    public Text coinText;
    public Text cardText;
    public int actorNumber;
    public GameObject badArrangeText;
    public GameObject CoinImage;
    public Text specialText;
    public int Score;

    [HideInInspector]
    public bool isBadArranged
    {
        get { return badArrangeText.activeSelf; }
        set { badArrangeText.SetActive(value); }
    }
    [HideInInspector] public bool isDoubled;
    [HideInInspector] public bool isMissioned;
    [HideInInspector] public bool isLucky;
    [HideInInspector] public int specialBonus;
    [HideInInspector] public FortuneMissionCard mission;

    [HideInInspector]
    public bool IsSeat
    {
        get { return this.gameObject.activeSelf; }
        set { this.gameObject.SetActive(value); }
    }
    public int totalCoin;
    public int Coin
    {
        get
        {
            int returnValue = 0;
            try
            {
                returnValue = int.Parse(coinText.text);
            }
            catch
            {
                coinText.text = "0";
            }
            return returnValue;
        }
        set
        {
            coinText.text = value.ToString();
            if (Coin < 0)
            {
                coinText.color = Color.red;
            }
            else if (Coin == 0)
            {
                coinText.color = Color.white;
            }
            else
            {
                coinText.color = Color.green;
            }
        }
    }

    void Start()
    {
        var cards = GetComponentsInChildren<FortuneCard>();
        cardList = new List<FortuneCard>();
        mission = new FortuneMissionCard();
        foreach (var card in cards)
        {
            cardList.Add(card);
        }
        CoinImage = this.gameObject.transform.parent.parent.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "CenterCoin").First().gameObject;
    }

    internal void Init(UserSeat seat)
    {
        var pList = new PlayerInfoContainer();
        pList.GetInfoContainerFromPhoton();
        isBadArranged = false;
        isMissioned = false;
        isDoubled = false;
        isLucky = false;
        try
        {
            var myInfo = pList.m_playerList.Where(x => x.m_actorNumber == seat.m_playerInfo.m_actorNumber).First();
            if (myInfo.m_status == enumPlayerStatus.Fortune_dealtCard ||
                    myInfo.m_status == enumPlayerStatus.Fortune_Doubled ||
                    myInfo.m_status == enumPlayerStatus.Fortune_Lucky)
                IsSeat = seat.isSeat;
            else
                IsSeat = false;

            actorNumber = seat.m_playerInfo.m_actorNumber;
            CheckIfBadArranged();
            isDoubled = false;
            if (myInfo.m_status == enumPlayerStatus.Fortune_Doubled)
                isDoubled = true;
            if (myInfo.m_status == enumPlayerStatus.Fortune_Lucky)
                isLucky = true;                

            coinText.text = "";
            cardText.text = "";
            totalCoin = 0;

            string missionString = (string)PhotonNetwork.CurrentRoom.CustomProperties[Common.FORTUNE_MISSION_CARD];
            mission.missionString = missionString;
        }
        catch
        {
            IsSeat = false;
        }
    }

    private void CheckIfBadArranged()
    {
        //FortunePlayerMgr.Inst.cardList[0].
        isBadArranged = false;
        FortuneUserCardList player = FortunePlayerMgr.Inst.userCardList.Where(x => x.actorNumber == actorNumber).First();

        var backType = FortuneRuleMgr.GetCardType(player.backCard, ref player.backCard);
        var middleType = FortuneRuleMgr.GetCardType(player.middleCard, ref player.middleCard);
        var frontType = FortuneRuleMgr.GetCardType(player.frontCard, ref player.frontCard);

        var backScore = FortuneRuleMgr.GetScore(player.backCard, backType);
        var middleScore = FortuneRuleMgr.GetScore(player.middleCard, middleType);
        var frontScore = FortuneRuleMgr.GetScore(player.frontCard, frontType);

        if (backScore < middleScore || middleScore < frontScore)
        {
            isBadArranged = true;
        }
    }

    internal void ShowCards(List<Card> showList)
    {

        foreach (var card in cardList)
            card.gameObject.SetActive(false);

        for (int i = 0; i < showList.Count; i++)
        {
            cardList[i].SetValue(showList[i]);
        }
    }

    internal async void SendCoin(UIFCalcPlayer tarPlayer, int v)
    {
        //LogMgr.Inst.Log("Send Coin " + v + " to " + tarPlayer.actorNumber + " from" + this.actorNumber);
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber || tarPlayer.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            ShowSendCoinEffect(tarPlayer);
        await Task.Delay(1000);
        Coin -= v;
        tarPlayer.Coin += v;

        totalCoin -= v;
        tarPlayer.totalCoin += v;
    }

    private async void ShowSendCoinEffect(UIFCalcPlayer tarPlayer)
    {
        List<GameObject> coinImages = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(CoinImage, coinText.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            Vector3 scale = obj.transform.localScale;
            scale.y = 1.5f;
            scale.x = 1.5f;
            obj.transform.localScale = scale;
            obj.SetActive(true);
            coinImages.Add(obj);
        }
        await Task.Delay(500);
        float goTime = 1.0f;
        foreach (var img in coinImages)
        {
            iTween.MoveTo(img, tarPlayer.coinText.transform.position, goTime);
            goTime += 0.3f;
        }

        StartCoroutine(DestroyCoins(coinImages));
    }

    IEnumerator DestroyCoins(List<GameObject> coinImages)
    {
        yield return new WaitForSeconds(3.5f);
        foreach (var img in coinImages)
        {
            Destroy(img);
            //img.SetActive(false);
        }
    }

    internal void SetCardType(int lineNo)
    {
        List<Card> cards = new List<Card>();
        foreach (var card in cardList.Where(x => x.gameObject.activeSelf == true))
        {
            cards.Add(card.GetValue());
        }
        var type = FortuneRuleMgr.GetCardType(cards, ref cards);
        Score = FortuneRuleMgr.GetScore(cards, type);
        cardText.text = FortuneRuleMgr.GetCardTypeString(type);

        if(isBadArranged)
            Score = 0;

        if(Score == 0)
            cardText.text = "";

        isMissioned = false;
        if(lineNo == mission.missionLine)
        {
            HandSuit missionSuit = (HandSuit)mission.missionNo;
            if(type == (HandSuit)mission.missionNo)
            {
                isMissioned = true;
            }
        }
        
        specialBonus = 0;
        specialText.text = "";
        if(!isBadArranged)        
            CheckIfSpecialCard(lineNo, type);
    }

    private void CheckIfSpecialCard(int lineNo, HandSuit type)
    {
        specialBonus = 0;
        switch(lineNo)
        {
            case 2:
                if(type == HandSuit.Royal_Flush)    specialBonus = 6;
                if(type == HandSuit.Straight_Flush) specialBonus = 4;
                if(type == HandSuit.Four_Of_A_Kind) specialBonus = 3;
                break;
            case 1:
                if(type == HandSuit.Full_House)     specialBonus = 3;
                if(type == HandSuit.Four_Of_A_Kind) specialBonus = 4;
                if(type == HandSuit.Straight_Flush) specialBonus = 5;
                if(type == HandSuit.Royal_Flush)    specialBonus = 6;
                break;
            case 0:
                if(type == HandSuit.Triple)         specialBonus = 2;
                break;
        }
        if(specialBonus > 0)
        {
            specialText.text = type + " X " + specialBonus;
        }
    }
}
