using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string previousScene=Constant.First;
    public static string curScene;
    public static void LoadScene(string sceneName)
    {
        previousScene = curScene;
        curScene = sceneName;
        SceneManager.LoadScene(sceneName);
    }
}
