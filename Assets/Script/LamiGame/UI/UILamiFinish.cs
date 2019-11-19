using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILamiFinish : MonoBehaviour
{
    public UILamiFinishScorePan[] scorePan;
    void Start()
    {
        SetData();
    }

    public void SetData()
    {
        for (int i = 0; i < scorePan.Length; i++)
        {
            LamiUserSeat seat = LamiPlayerMgr.Inst.m_playerList[i];
            seat.cardPoint = 0;
            int cardPoint = 0;
            foreach (var card in seat.cardList.Where(x=>x.MyCardId != 1).ToList())
            {
                int addVal = 0;
                switch (card.num)
                {
                    case 1:
                        addVal = 15; break;
                    case 15:
                        addVal = 0; break;
                    case 11:
                    case 12:
                    case 13:
                        addVal = 10;
                        break;
                    default:
                        addVal = card.num;
                        break;
                }
                cardPoint += addVal;
                seat.cardPoint++;
            }
            seat.score = cardPoint;
            
        }

        for (int i = 0; i < scorePan.Length; i++)
        {
            var seat = LamiPlayerMgr.Inst.m_playerList.OrderBy(x=>x.cardPoint).ToList()[i];

            //LamiUserSeat seat = LamiPlayerMgr.Inst.m_playerList[i];
            scorePan[i].UpdateInfo(seat.mUserPic.sprite, seat.mUserSkillName.text,
                (i+1).ToString	(),
                            seat.mUserName.text, (i+1) + "", seat.cardPoint,
                            seat.mAceValue.text, seat.mJokerValue.text, 
                            50*(int.Parse(seat.mAceValue.text) + int.Parse(seat.mJokerValue.text)) - seat.cardPoint*10 - (i*2*100),
                            seat.cardList);
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
}
