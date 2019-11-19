using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILamiSelectCardList : MonoBehaviour
{
    public UILamiCardSelect listObj;
    public LamiGameCard gameCard;
    public List<List<Card>> mList;

    private const int cardWidth = 26;
    private const int padding = 10; 
    public void Show(List<List<Card>> list, List<int> matchNoList)
    {
        transform.parent.gameObject.SetActive(true);
        mList = list;
        for (int i = 0; i < mList.Count; i ++)
        {
            UILamiCardSelect cardSelect = Instantiate(listObj,transform);
            cardSelect.id = matchNoList[i];
            foreach (var card in list[i])
            {
                LamiGameCard mCard = Instantiate(gameCard,cardSelect.transform);
                mCard.UpdateCard(card);
            }
            RectTransform rt = cardSelect.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(padding+cardWidth*list[i].Count,rt.sizeDelta.y);
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
