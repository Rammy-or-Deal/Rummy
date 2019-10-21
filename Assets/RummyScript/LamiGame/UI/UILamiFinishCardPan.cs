using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiFinishCardPan : MonoBehaviour
{
    public List<LamiMyCard> mCards;
    private int cardCount=20;
    // Start is called before the first frame update
    void Start()
    {
        LamiMyCard card = mCards[0];
        for (int i = 1; i < cardCount; i++)
        {
            LamiMyCard newCard = Instantiate(card,card.transform.position, Quaternion.identity,this.transform);
            mCards.Add(newCard);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
