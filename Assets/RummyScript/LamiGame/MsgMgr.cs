using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.RummyScript.LamiGame
{
    public class MsgMgr
    {
        protected LamiMgr parent;
        protected MsgMgr(LamiMgr parent)
        {
            this.parent = parent;
        }

        public void OnMessageArrived(int message)
        {
            
        }
    }
}