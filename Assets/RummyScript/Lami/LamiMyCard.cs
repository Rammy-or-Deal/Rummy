using System.Collections;
using System.Collections.Generic;
using UITween;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LamiMyCard : MonoBehaviour
{
    public Image mCard;
    public bool isSelected;
    public int color;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        UpdateValue();
    }

    public void UpdateValue()
    {
        mCard.sprite = Resources.Load<Sprite>("new_card/" + "card_" + color + "_" + num);
    }
    public void OnClick()
    {
        isSelected = (!isSelected);
        int move = 10 * (isSelected ? -1 : 1);
        LeanTween.moveY(gameObject, transform.position.y - move, 0.1f);
        
        LamiGameUIManager.Inst.myCardPanel.SetPlayButtonState(this);
    }
}
