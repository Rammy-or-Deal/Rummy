using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiFinishCardPan : MonoBehaviour
{
    public List<LamiMyCard> mCards;
    private int cardCount = 20;

    public void UpdateCards(List<Card> cards)
    {

            for(int i = 1; i < mCards.Count; i++)
                Destroy(mCards[i].gameObject);

            LamiMyCard card = mCards[0];
            card.UpdateFinishCard(cards[0]);
            for (int i = 1; i < cardCount; i++)
            {
                LamiMyCard newCard = Instantiate(card, card.transform.position, Quaternion.identity, this.transform);
                newCard.UpdateFinishCard(cards[i]);
                mCards.Add(newCard);
            }

    }
}
