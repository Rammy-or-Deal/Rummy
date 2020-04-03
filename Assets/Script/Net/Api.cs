﻿using System.Collections.Generic;
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
                SceneManager.LoadScene("2_Lobby");
            })
            .Catch(err => this.LogMessage(err.Message));
        return;
    }

    public void Post()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "/posts",
            Body = new Post
            {
                title = "foo",
                body = "bar",
                userId = 1
            }
        };
        RestClient.Post<Post>(currentRequest)
            .Then(res => this.LogMessage(JsonUtility.ToJson(res, true)))
            .Catch(err => this.LogMessage(err.Message));
    }

    public void Put()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "/posts/1",
            Body = new Post
            {
                title = "foo",
                body = "bar",
                userId = 1
            },
            Retries = 5,
            RetrySecondsDelay = 1,
            RetryCallback = (err, retries) =>
            {
                Debug.Log(string.Format("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
            }
        };
        RestClient.Put<Post>(currentRequest, (err, res, body) =>
        {
            if (err != null)
            {
                this.LogMessage(err.Message);
            }
            else
            {
                this.LogMessage(JsonUtility.ToJson(body, true));
            }
        });
    }

    public void Delete()
    {
        RestClient.Delete(basePath + "/posts/1", (err, res) =>
        {
            if (err != null)
            {
                this.LogMessage(err.Message);
            }
            else
            {
                this.LogMessage("Status: " + res.StatusCode.ToString());
            }
        });
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