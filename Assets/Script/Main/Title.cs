using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    void Awake ()
    {
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void GuestBtnClick()
    {
        Debug.Log(DataController.Inst.userInfo.ToString());
//        SceneManager.LoadScene(Constant.LobbyScene);
    }
    public void FaceBookBtnClick()
    {
        var perms = new List<string>(){"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    
    private void AuthCallback (ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            DataController.Inst.SetFbId(aToken.UserId);
            GetFbName();
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
//                Debug.Log(perm);
            }
        } else {
            Debug.Log("User cancelled login");
        }
    }
    
    public void GetFbName()
    {
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            Debug.Log(result );
            if (result.ResultDictionary != null)
            {
                Debug.Log(result.ResultDictionary);
                foreach (string key in result.ResultDictionary.Keys)
                {
                    Debug.Log(key + " : " );
//                    Debug.Log(key + " : " + result.ResultDictionary[key]);
                }
                DataController.Inst.userInfo.name = result.ResultDictionary["name"].ToString();
                Api.Inst.PostUserFacebook();
            }
        });
    }
    
    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
}

