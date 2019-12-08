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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void GuestBtnClick()
    {
        SceneManager.LoadScene("2_Lobby");
//        PunController.Inst.Login();
    }
    public void FaceBookBtnClick()
    {
        var perms = new List<string>(){"public_profile", "email","user_friends"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    
    private void AuthCallback (ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
//                Debug.Log(perm);
            }
//            GetName();
//            GetPicture();
            SceneManager.LoadScene("2_Lobby");
        } else {
            Debug.Log("User cancelled login");
        }
    }
    
    public void GetName()
    {
        FB.API("me?fields=name,picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.ResultDictionary != null)
            {
                foreach (string key in result.ResultDictionary.Keys)
                {
                    Debug.Log(key + " : " + result.ResultDictionary[key].ToString());
                    if (key == "name")
                        Debug.Log(result.ResultDictionary[key].ToString());
                }
            }
        });
    }

    public void GetPicture()
    {
        FB.API("me/picture?type=med", Facebook.Unity.HttpMethod.GET, GetPicture);
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
    
    private void GetPicture(IGraphResult result)
    {
        Debug.Log(result.ToString());
        if (result.Error == null)
        {
            Debug.Log(result.Texture.ToString());
//            Image img = UIFBProfilePic.GetComponent<Image>();
//            img.sprite = Sprite.Create(result.Texture, new Rect(0,0, 128, 128), new Vector2());         
        }

    }
}
