using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISettingDialog : MonoBehaviour
{
    public Image mMusicPic;
    public Image mSoundPic;

    public Image mEnPic;
    public Image mChPic;

    private Sprite _mOnPic;
    private Sprite _mOffPic;
    private Sprite _mCheckPic;
    private Sprite _mUnCheckPic;

    // Start is called before the first frame update
    void Start()
    {
        _mOnPic = Resources.Load<Sprite>("new_button/on_btn");
        _mOffPic = Resources.Load<Sprite>("new_button/off_btn");
        _mCheckPic = Resources.Load<Sprite>("new_button/check_btn");
        _mUnCheckPic = Resources.Load<Sprite>("new_button/uncheck_btn");
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

    public void OnClickLang()
    {
        if (DataController.Inst.setting.Lang == "en")
        {
            DataController.Inst.setting.Lang = "cn";
            mEnPic.sprite = _mUnCheckPic;
            mChPic.sprite = _mCheckPic;
        }
        else
        {
            DataController.Inst.setting.Lang = "en";
            mEnPic.sprite = _mCheckPic;
            mChPic.sprite = _mUnCheckPic;
        }
    }

    public void OnClose(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OnClickLogout()
    {
        Debug.Log("log out");
        gameObject.SetActive(false);
        SceneManager.LoadScene("1_Title");
    }
}