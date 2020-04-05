using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public Image m_LoadingProgress;
    public float m_ProgPer = 0;
    public GameObject btns;
    public GameObject loadingObj;

    public void Start()
    {
        //AvataMgr.Instance.StartDownload();
        SetLoading(true);
        StartCoroutine(ProgWork());
    }

    public void SetLoading(bool flag)
    {
        loadingObj.SetActive(flag);
        btns.SetActive(!flag);
    }

    public void LoadTitleScene()
    {
        SetLoading(false);
    }

    private IEnumerator ProgWork()
    {
        while (m_ProgPer < 1)
        {
            m_ProgPer += Time.deltaTime;
            SetProgBar(m_ProgPer);
            yield return null;
        }

        // progress done..
        LoadTitleScene();
    }

    private void SetProgBar(float per)
    {
        if (m_LoadingProgress == null)
            return;
        m_LoadingProgress.fillAmount = per;
    }
}
