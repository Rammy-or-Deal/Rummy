using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeCardDialog : MonoBehaviour
{
    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    public FortuneHandMission[] handMissions;

    public Text frontText;
    public Text middleText;
    public Text backText;

    //
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        for(int i = 0; i < myCards.Length; i++)
        {
            myCards[i].Init(true);
        }
    }

    public void OnTipClick()
    {

    }
    public void OnDoubleDownClick()
    {

    }

    public void OnConfirmClick()
    {

    }
}
