using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILuckyPlayer : UIFResultPlayer
{
    // Start is called before the first frame update
    public override void SetProperty(UIFCalcPlayer uIFCalcPlayer)
    {
        
        if (uIFCalcPlayer.totalCoin < 0)
            resultText.text = uIFCalcPlayer.totalCoin.ToString();
        else
            resultText.text = (uIFCalcPlayer.totalCoin * 0.9).ToString();

        if(uIFCalcPlayer.totalCoin == 0) resultText.color = Color.white;
        if(uIFCalcPlayer.totalCoin > 0) resultText.color = Color.green;
        if(uIFCalcPlayer.totalCoin < 0) resultText.color = Color.red;
    }
}
