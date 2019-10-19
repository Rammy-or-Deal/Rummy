using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIMyCardPanel : MonoBehaviour
{
    //MyCard
    public List<LamiMyCard> myCards;

    [HideInInspector] public List<LamiMyCard> selectedCards;

    // Start is called before the first frame update
    void Start()
    {
        selectedCards = new List<LamiMyCard>();
    }

    public void InitCards(Card[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            LamiMyCard cardEntry = myCards[i];
            cardEntry.num = cards[i].num;
            cardEntry.color = cards[i].color;
            //cardEntry.UpdateValue();
        }

        gameObject.SetActive(true);
    }

    public void DealCards()
    {
        
//        foreach (LamiMyCard card in selectedCards)
//        {
//            LamiGameUIManager.Inst.AddGameCard(card);
//        }



        string cardStr = LamiCardMgr.ConvertSelectedListToString(selectedCards);

        int remainCard = RemainCards();
        Hashtable gameCards = new Hashtable
        {   
            {Common.LAMI_MESSAGE, (int)LamiMessages.OnDealCard},
            {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
            {Common.REMAIN_CARD_COUNT, remainCard},
            {Common.GAME_CARD, cardStr},
            {Common.GAME_CARD_PAN, 0},
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(gameCards);
        LogMgr.Inst.Log("User dealt card: " + cardStr, (int)LogLevels.PlayerLog2);
        RemoveCards();
    }

    public void RemoveCards()
    {
        foreach (LamiMyCard card in selectedCards)
        {
            myCards.Remove(card);
            card.gameObject.SetActive(false);
        }
        selectedCards.Clear();
    }

    public int RemainCards()
    {
        return (myCards.Count - selectedCards.Count);
    }

    public void ArrangeMyCard()
    {
        foreach (LamiMyCard card in myCards)
        {
            
        }
    }

    public void SetPlayButtonState(LamiMyCard clickedCard)
    {
        if (clickedCard.isSelected)
            selectedCards.Add(clickedCard);
        else
            selectedCards.Remove(clickedCard);

        LamiGameUIManager.Inst.playButton.interactable = (selectedCards.Count > 0);
    }
    
    
}