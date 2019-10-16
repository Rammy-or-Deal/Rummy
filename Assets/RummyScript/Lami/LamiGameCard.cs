using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LamiGameCard : MonoBehaviour
{
    public Image mCard;
    public int color;
    public int num;
    // Start is called before the first frame update

    void Start()
    {
        UpdateValue();
    }

    public void UpdateValue()
    {
        mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
