using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class BaccaratBotMgr : BotMgr
{
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.Inst.botMgr = this;
        base.CreateBot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Deal()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        base.Deal();
        try
        {
            if (GameMgr.Inst.seatMgr.m_playerList.Count(x => x.m_playerInfo.m_actorNumber < 0) == 0) return;
            foreach (var bot in GameMgr.Inst.seatMgr.m_playerList.Where(x => x.m_playerInfo.m_actorNumber < 0))
            {
                if (Random.Range(0.0f, 1.0f) > 0.7) continue;
                int moneyId = Random.Range(0, 5);
                int areaId = Random.Range(0, 6);

                if (bot.m_playerInfo.m_coinValue < BaccaratBankerMgr.Inst.getCoinValue(moneyId)) continue;

                bot.m_playerInfo.m_coinValue -= BaccaratBankerMgr.Inst.getCoinValue(moneyId);

                BaccaratMe.Bet(moneyId, areaId, bot.m_playerInfo.m_actorNumber);
            }
        }
        catch { }

    }
}
