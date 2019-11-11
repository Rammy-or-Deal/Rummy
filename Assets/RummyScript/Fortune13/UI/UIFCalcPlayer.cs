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

    public GameObject CoinImage;

    public int Score;    

    [HideInInspector]
    public bool IsSeat
    {
        get { return this.gameObject.activeSelf; }
        set { this.gameObject.SetActive(value); }
    }

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
        foreach (var card in cards)
        {
            cardList.Add(card);
        }

        CoinImage = this.gameObject.transform.parent.parent.GetComponentsInChildren<Image>(true).Where(x => x.gameObject.name == "CenterCoin").First().gameObject;
    }

    internal void Init(FortuneUserSeat seat)
    {
        IsSeat = seat.IsSeat;
        actorNumber = seat.actorNumber;
        coinText.text = "";
        cardText.text = "";
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
        yield return new WaitForSeconds(4.0f);
        foreach (var img in coinImages)
        {
            Destroy(img);
            //img.SetActive(false);
        }
    }

    internal void SetCardType()
    {
        List<Card> cards = new List<Card>();
        foreach (var card in cardList)
        {
            cards.Add(card.GetValue());
        }
        var type = FortuneRuleMgr.GetCardType(cards, ref cards);
        Score = FortuneRuleMgr.GetScore(cards, type);
        cardText.text = FortuneRuleMgr.GetCardTypeString(type);
    }
}
