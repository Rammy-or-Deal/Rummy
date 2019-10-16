using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{

    public Image m_LoadingProgress;
    public float m_ProgPer = 0;

    public void Start()
    {
        //AvataMgr.Instance.StartDownload();
        StartCoroutine(ProgWork());
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadSceneAsync("1_Title");
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
