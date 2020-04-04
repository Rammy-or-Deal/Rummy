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
        
        sysExchangeItem = new SysExchangeItemModel();
        DontDestroyOnLoad(this.gameObject);
        userInfo.udid=SystemInfo.deviceUniqueIdentifier;
    }

    // Start is called before the first frame update
    void Start()
    {
        userInfo.facebook_id=PlayerPrefs.GetString("facebook_id", "null");
        if (userInfo.facebook_id=="null")
        {
            userInfo = Api.Inst.GetUserbyUdid(userInfo.udid);    
        }
        else
        {
            Api.Inst.GetUserbyFacebook(userInfo.facebook_id);
        }
    }

    public void UpdateAvatar()
    {
        if (userInfo.facebook_id != null)
        {
            StartCoroutine(getFBPicture(userInfo.facebook_id));
        }
        else
        {
            UIController.Inst.userInfoPanel.SetAvatar();   
        }
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
                UIController.Inst.userInfoPanel.mUserPic.sprite = sprite;
                Debug.Log("facebook pic updated");
            }
        }
    }

    public void SetFbId(string fbId)
    {
        userInfo.facebook_id = fbId;
        userInfo.pic = fbId;
        PlayerPrefs.SetString("facebook_id", fbId);
    }
}