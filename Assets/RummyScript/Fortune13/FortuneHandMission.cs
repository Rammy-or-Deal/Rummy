using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortuneHandMission : MonoBehaviour
{
    public int id;
    public Text mMissionText;
    public Text mMissionValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetMission(FortuneMissionCard mission)
    {
        this.gameObject.SetActive(true);
        id = mission.missionNo;
        mMissionValue.text = "×" + mission.missionPrice;
        mMissionText.text = (((HandSuit)mission.missionNo) + "").Replace('_', ' ');
    }
}
