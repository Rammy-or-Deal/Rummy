using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiSelectCardList : MonoBehaviour
{
    public UILamiCardSelect listObj;
    public LamiGameCard gameCard;
    public List<List<Card>> mList;
    public void Show(List<List<Card>> list)
    {
        transform.parent.gameObject.SetActive(true);
        mList = list;
        for (int i = 0; i < mList.Count; i ++)
        {
            UILamiCardSelect cardSelect = Instantiate(listObj,transform);
            cardSelect.id = i;
            foreach (var card in list[i])
            {
                LamiGameCard mCard = Instantiate(gameCard,cardSelect.transform);
                mCard.UpdateCard(card);
            }
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        transform.parent.gameObject.SetActive(false);
    }
}
