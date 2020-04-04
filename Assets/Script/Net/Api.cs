using System.Collections.Generic;
using Models;
using Proyecto26;
using RummyScript.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Api : MonoBehaviour
{
    public static Api Inst;
    private readonly string basePath = "http://52.221.195.64/api/";
    private RequestHelper currentRequest;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        
    }

    public UserInfoModel GetUserbyUdid(string udid)
    {
        RestClient.Get<UserInfoModel>(basePath + "/users/udid/"+udid)
            .Then(res =>
            {
                this.LogMessage(res.ToString());
                return res;
            })
            .Catch(err => this.LogMessage(err.Message));
        return null;
    }
    
    public void GetUserbyFacebook(string fbId)
    {
        RestClient.Get<UserInfoModel>(basePath + "/users/facebook/"+fbId)
            .Then(res =>
            {
                this.LogMessage(res.ToString());
                DataController.Inst.userInfo = res;
                SceneManager.LoadScene(Constant.LobbyScene);
            })
            .Catch(err => this.LogMessage(err.Message));
        return;
    }
    public void PostUser()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "/users/update",
            Body = DataController.Inst.userInfo
        };
        RestClient.Post<UserInfoModel>(currentRequest)
            .Then(res => this.LogMessage(JsonUtility.ToJson(res, true)))
            .Catch(err => this.LogMessage(err.Message));
    }
    
    public void PostUserFacebook()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "/users/facebook",
            Body = DataController.Inst.userInfo
        };
        RestClient.Post<UserInfoModel>(currentRequest)
            .Then(res =>
            {
                this.LogMessage(JsonUtility.ToJson(res, true));
                DataController.Inst.userInfo = res;
                SceneManager.LoadScene(Constant.LobbyScene);
            })
            .Catch(err => this.LogMessage(err.Message));
    }
    
    public void AbortRequest()
    {
        if (currentRequest != null)
        {
            currentRequest.Abort();
            currentRequest = null;
        }
    }

    public void DownloadFile()
    {
        var fileUrl = "https://raw.githubusercontent.com/IonDen/ion.sound/master/sounds/bell_ring.ogg";
        var fileType = AudioType.OGGVORBIS;

        RestClient.Get(new RequestHelper
        {
            Uri = fileUrl,
            DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType)
        }).Then(res =>
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = ((DownloadHandlerAudioClip) res.Request.downloadHandler).audioClip;
            audio.Play();
        }).Catch(err => { this.LogMessage(err.Message); });
    }

    public void LogMessage(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
}