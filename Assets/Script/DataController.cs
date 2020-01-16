using System.Collections;
using RummyScript.Model;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class DataController : MonoBehaviour
{
    public static DataController Inst;
   
    public FrameItemModel frameItem;
    public FriendItemModel friendItem;
    public GiftItemModel giftItem;
    public LoginDayItemModel loginDayItem;
    public SysItemModel sysItem;
    public SysShopItemModel sysShopItem;
    public SysSkillModel sysSkill;
    public TierInfoModel tierInfo;
    public UserInfoModel userInfo;
    public SysExchangeItemModel sysExchangeItem;    
    public SettingModel setting;
    public Sprite facebookSprite;

    void Awake()
    {
        if (Inst)
        {
            Destroy(this.gameObject);
            return;
        }
            
        Inst = this;
        
        setting = new SettingModel();
        frameItem = new FrameItemModel();
        friendItem = new FriendItemModel();
        giftItem = new GiftItemModel();
        loginDayItem = new LoginDayItemModel();
        sysItem = new SysItemModel();
        sysShopItem = new SysShopItemModel();
        sysSkill = new SysSkillModel();
        tierInfo = new TierInfoModel();
        tierInfo.Init();
        userInfo = new UserInfoModel();
        userInfo.Init("User");
        sysExchangeItem = new SysExchangeItemModel();
        
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FB.IsLoggedIn)
        {
            GetNameAndPicture();
        }
    }
    
    public void GetNameAndPicture()
    {
        userInfo.facebookId = AccessToken.CurrentAccessToken.UserId;
        userInfo.pic = userInfo.facebookId;
        StartCoroutine(getFBPicture(userInfo.facebookId));
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.ResultDictionary != null)
            {
                foreach (string key in result.ResultDictionary.Keys)
                {
                    Debug.Log(key + " : " + result.ResultDictionary[key]);
                }
                userInfo.name = result.ResultDictionary["name"].ToString();
                ChatMgr.Inst.chatClient.UserId = userInfo.name;
                UIController.Inst.userInfoPanel.UpdateValue();
            }
        });
    }

    public void GetFBPicture(string facebookId, Sprite pic = null)
    {
        StartCoroutine(getFBPicture(facebookId, pic));

    }

    public IEnumerator getFBPicture(string facebookId,Sprite pic=null)
    {
        var www = new WWW("http://graph.facebook.com/" + facebookId +
                          "/picture?width=250&height=250&type=square&redirect=true");
        Debug.Log("http://graph.facebook.com/" + facebookId +
                  "/picture?width=250&height=250&type=normal&redirect=true" + "\t" + www.error);
        yield return www;

        if (www.isDone)
        {
            Debug.Log("waiting" + www.bytesDownloaded);
            Sprite sprite=Sprite.Create(www.texture,new Rect(0,0, www.texture.width, www.texture.width), new Vector2());
            if (pic)
                pic = sprite;
            else
            {
                userInfo.sprite = sprite;
                UIController.Inst.userInfoPanel.UpdateValue();    
            }
        }
    }
}