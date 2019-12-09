using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILamiFinish : MonoBehaviour
{
    public UILamiFinishScorePan[] scorePan;
    public GameObject backBtn;
    public GameObject examineBtn;
    public GameObject victoryObj;
    public GameObject firstTitle;
    public GameObject examineTitle;

    void Start()
    {

    }

    public void SetData()
    {
        int index = 0;

        foreach (LamiUserSeat player in LamiPlayerMgr.Inst.m_playerList.OrderBy(x => ((LamiUserSeat)x).m_point))
        {
            if (player.m_point == 0)    // If the player dealt all cards,
            {
                if(index == 0)  // If this player is Game Player, 
                {
                    player.m_matchWinning = staticFunction_rummy.GetGameBonus(GameMgr.Inst.m_gameTier);
                }
                else
                {
                    player.m_matchWinning = -staticFunction_rummy.GetGameBonus(GameMgr.Inst.m_gameTier);
                }
            }
            else
            {
                player.m_matchWinning = staticFunction_rummy.GetWinningBonus(GameMgr.Inst.m_gameTier, index);

                //LamiUserSeat seat = LamiPlayerMgr.Inst.m_playerList[i];
                // scorePan[index].UpdateInfo(player.mUserPic.sprite, player.mUserSkillName.text,
                //     (index + 1).ToString(),
                //                 player.mUserName.text, (index + 1) + "", player.cardPoint,
                //                 player.mAceValue.text, player.mJokerValue.text,
                //                 50 * (int.Parse(player.mAceValue.text) + int.Parse(player.mJokerValue.text)) - player.cardPoint * 10 - (index * 2 * 100),
                //                 player.cardList);
            }
            
            scorePan[index].UpdateInfo(player);

            index++;
        }
    }

    public void OnReportBtn()
    {

    }

    public void OnExitBtn()
    {
        //gameObject.SetActive(false);
        PunController.Inst.LeaveGame();
    }

    public void OnContinueBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnCloseBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnExamineBtn()
    {
        UpdateObjs(true);
    }

    public void OnBackBtn()
    {
        UpdateObjs(false);
    }

    void UpdateObjs(bool flag)
    {
        firstTitle.SetActive(!flag);
        examineTitle.SetActive(flag);
        examineBtn.SetActive(!flag);
        backBtn.SetActive(flag);

        foreach (var line in scorePan)
        {
            line.cardPoints.gameObject.SetActive(!flag);
            line.aceCount.gameObject.SetActive(!flag);
            line.jockerCount.gameObject.SetActive(!flag);
            line.matchwinningTxt.gameObject.SetActive(!flag);

            line.cardPan.gameObject.SetActive(flag);
        }
    }
}
