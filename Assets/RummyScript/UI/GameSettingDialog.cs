using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingDialog : MonoBehaviour
{
    public Image mMusicPic;
    public Image mSoundPic;


    private Sprite _mOnPic;
    private Sprite _mOffPic;


    // Start is called before the first frame update
    void Start()
    {
        _mOnPic = Resources.Load<Sprite>("new_button/on_btn");
        _mOffPic = Resources.Load<Sprite>("new_button/off_btn");
    }

    public void OnClickMusic()
    {
        Debug.Log("music button clicked");
        if (DataController.Inst.setting.IsMusicOn == true)
        {
            DataController.Inst.setting.IsMusicOn = false;
            mMusicPic.sprite = _mOffPic;
        }
        else
        {
            DataController.Inst.setting.IsMusicOn = true;
            mMusicPic.sprite = _mOnPic;
        }
    }

    public void OnClickSound()
    {
        Debug.Log("sound button clicked");
        if (DataController.Inst.setting.IsSoundOn == true)
        {
            DataController.Inst.setting.IsSoundOn = false;
            mSoundPic.sprite = _mOffPic;
        }
        else
        {
            DataController.Inst.setting.IsSoundOn = true;
            mSoundPic.sprite = _mOnPic;
        }
    }


    public void OnClose()
    {
        gameObject.SetActive(false);
    }

}