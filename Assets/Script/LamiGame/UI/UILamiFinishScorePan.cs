using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILamiFinishScorePan : MonoBehaviour
{
    
    public Image avatar;
    public Image frame;
    public Image skill;
    public Text username;

    public Text rankTxt;
    public Text matchwinningTxt;
    public UILamiFinishCardPan cardPan;
    public Text cardPoints;
    public Text aceCount;
    public Text jockerCount;
    public Text score;
    
    void Start()
    {
        
    }

    public void UpdateInfo(LamiUserSeat seat)
    {
        avatar.sprite=seat.mUserPic.sprite;
        frame.sprite=seat.mUserFrame.sprite;
        //Debug.Log("skill name:"+skillName);
        skill.sprite=Resources.Load<Sprite>("new_skill/skill_"+seat.mUserSkillName.text);
        username.text =seat.mUserName.text;

        cardPoints.text = seat.cardPoint.ToString();
        aceCount.text = seat.aCount.ToString();
        jockerCount.text = seat.jokerCount.ToString();
        score.text = (seat.score + seat.AddScore + seat.aCount * Constants.lamiAMultiply + seat.jokerCount*Constants.lamiJokerMultiply).ToString();
        
        string ss = "CreatedCardList("+seat.id+") := ";
        foreach (var card in seat.cardList)
        {
            ss += string.Format("{0}:{1}/{2}, ", card.num, card.color, card.MyCardId);
        }
        //Debug.Log(ss);

        cardPan.UpdateCards(seat.cardList);
    }
}
