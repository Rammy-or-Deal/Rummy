using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

        int aceBonus = staticFunction_rummy.GetAceBonus(GameMgr.Inst.m_gameTier);
        int jokerBonus = staticFunction_rummy.GetJokerBonus(GameMgr.Inst.m_gameTier);

        cardPoints.text = (seat.m_point).ToString();
        aceCount.text = seat.m_aceCount.ToString();
        jockerCount.text = seat.m_jokerCount.ToString();
        matchwinningTxt.text = seat.m_matchWinning.ToString();
        //int m_score = -seat.m_point + seat.m_matchWinning + seat.m_aceCount * aceBonus + seat.m_jokerCount*jokerBonus;
        int m_score = seat.m_matchWinning + seat.m_aceScore * aceBonus + seat.m_jokerScore*jokerBonus;
        score.text = m_score.ToString();
        
        string ss = "Finish Panel CreatedCardList("+seat.id+") := ";
        foreach (var card in seat.cardList)
        {
            ss += string.Format("{0}:{1}/{2}, ", card.num, card.color, card.MyCardId);
        }
        Debug.Log(ss);
        

        cardPan.UpdateCards(seat.cardList);

        if(PhotonNetwork.IsMasterClient)
        {
            GameMgr.Inst.seatMgr.AddGold(seat.m_playerInfo.m_actorNumber, m_score);
        }
    }
}
