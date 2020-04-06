using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string previousScene=Constant.First;
    public static string curScene;
    public static bool isFromFirst = true;
    public static void LoadScene(string sceneName)
    {
        if (sceneName == Constant.First)
            isFromFirst = true;
        previousScene = curScene;
        curScene = sceneName;
        SceneManager.LoadScene(sceneName);
    }
}
