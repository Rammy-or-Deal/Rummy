using System.Collections;
using System.Collections.Generic;
using RummyScript.Model;
using UnityEngine;




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
        userInfo.Init();
        sysExchangeItem = new SysExchangeItemModel();
        
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}