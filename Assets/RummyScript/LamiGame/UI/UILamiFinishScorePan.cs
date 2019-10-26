using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILamiFinishScorePan : MonoBehaviour
{
    public Image background;
    
    public Image avatar;
    public Image frame;
    public Image skill;
    public Text username;

    public Text rankTxt;

    public UILamiFinishCardPan cardPan;
    public Text cardPoints;
    public Text aceCount;
    public Text jockerCount;
    public Text score;
    
    void Start()
    {
        
    }

    public void UpdateInfo(Sprite picSprite,string frameName,string skillName,string userName,string rankText,int cardPoint,string aceCnt,string jockerCnt,int scoreVal,List<Card> cards)
    {
        avatar.sprite=picSprite;
        frame.sprite=Resources.Load<Sprite>("new_frame/frame_"+frameName);
        skill.sprite=Resources.Load<Sprite>("new_skill/"+skillName);
        username.text =userName;
        rankTxt.text = rankText;

        cardPoints.text = cardPoint.ToString();
        aceCount.text = aceCnt.ToString();
        jockerCount.text = jockerCnt.ToString();
        score.text = scoreVal.ToString();
        
        cardPan.UpdateCards(cards);
    }
}
