using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Realtime;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;

#endif

public class ChatMgr : MonoBehaviour, IChatClientListener
{
    public UIChatDialog chatDlg;
    public string[] ChannelsToJoinOnConnect; // set in inspector. Demo channels to join automatically.

    public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    private string selectedChannelName; // mainly used for GUI/input

    public ChatClient chatClient;

#if !PHOTON_UNITY_NETWORKING
    [SerializeField]
#endif
    protected internal AppSettings chatAppSettings;
    
    private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

    public static ChatMgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    public void Start()
    {

#if PHOTON_UNITY_NETWORKING
        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
#endif

        bool appIdPresent = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
        
        if (!appIdPresent)
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
        }
        Connect();
    }

    public void Connect()
    {
        this.chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        this.chatClient.UseBackgroundWorkerForSending = true;
#endif

        this.chatClient.Connect(this.chatAppSettings.AppIdChat, "1.0",
            new Photon.Chat.AuthenticationValues(DataController.Inst.userInfo.name));
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    public void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient
                .Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }

    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];

    public void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }

        if ("test".Equals(inputLine))
        {
            if (this.TestLength != this.testBytes.Length)
            {
                this.testBytes = new byte[this.TestLength];
            }

            this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
        }


        bool doingPrivateChat = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
        string privateChatTarget = string.Empty;
        if (doingPrivateChat)
        {
            // the channel name for a private conversation is (on the client!!) always composed of both user's IDs: "this:remote"
            // so the remote ID is simple to figure out

            string[] splitNames = this.selectedChannelName.Split(new char[] {':'});
            privateChatTarget = splitNames[1];
        }
        //UnityEngine.Debug.Log("selectedChannelName: " + selectedChannelName + " doingPrivateChat: " + doingPrivateChat + " privateChatTarget: " + privateChatTarget);


        if (inputLine[0].Equals('\\'))
        {
            string[] tokens = inputLine.Split(new char[] {' '}, 2);
            Debug.Log(tokens);
            if (tokens[0].Equals("\\state"))
            {
                int newState = 0;


                List<string> messages = new List<string>();
                messages.Add("i am state " + newState);
                string[] subtokens = tokens[1].Split(new char[] {' ', ','});

                if (subtokens.Length > 0)
                {
                    newState = int.Parse(subtokens[0]);
                }

                if (subtokens.Length > 1)
                {
                    messages.Add(subtokens[1]);
                }

                this.chatClient.SetOnlineStatus(newState,
                    messages.ToArray()); // this is how you set your own state and (any) message
            }
            else if ((tokens[0].Equals("\\subscribe") || tokens[0].Equals("\\s")) && !string.IsNullOrEmpty(tokens[1]))
            {
                this.chatClient.Subscribe(tokens[1].Split(new char[] {' ', ','}));
            }
            else if ((tokens[0].Equals("\\unsubscribe") || tokens[0].Equals("\\u")) && !string.IsNullOrEmpty(tokens[1]))
            {
                this.chatClient.Unsubscribe(tokens[1].Split(new char[] {' ', ','}));
            }
            else if (tokens[0].Equals("\\clear"))
            {
                if (doingPrivateChat)
                {
                    this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
                }
                else
                {
                    ChatChannel channel;
                    if (this.chatClient.TryGetChannel(this.selectedChannelName, doingPrivateChat, out channel))
                    {
                        channel.ClearMessages();
                    }
                }
            }
            else if (tokens[0].Equals("\\msg") && !string.IsNullOrEmpty(tokens[1]))
            {
                string[] subtokens = tokens[1].Split(new char[] {' ', ','}, 2);
                if (subtokens.Length < 2) return;

                string targetUser = subtokens[0];
                string message = subtokens[1];
                this.chatClient.SendPrivateMessage(targetUser, message);
            }
            else if ((tokens[0].Equals("\\join") || tokens[0].Equals("\\j")) && !string.IsNullOrEmpty(tokens[1]))
            {
                string[] subtokens = tokens[1].Split(new char[] {' ', ','}, 2);

                // If we are already subscribed to the channel we directly switch to it, otherwise we subscribe to it first and then switch to it implicitly
                if (this.channelToggles.ContainsKey(subtokens[0]))
                {
                    this.ShowChannel(subtokens[0]);
                }
                else
                {
                    this.chatClient.Subscribe(new string[] {subtokens[0]});
                }
            }
            else
            {
                Debug.Log("The command '" + tokens[0] + "' is invalid.");
            }
        }
        else
        {
            if (doingPrivateChat)
            {
                this.chatClient.SendPrivateMessage(privateChatTarget, inputLine);
            }
            else
            {
                this.chatClient.PublishMessage(this.selectedChannelName, inputLine);
            }
        }
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnConnected()
    {
        if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
        {
            this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
        }

        this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }

    public void OnDisconnected()
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        // use OnConnected() and OnDisconnected()
        // this method might become more useful in the future, when more complex states are being used.
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
        Debug.Log("OnSubscribed: " + string.Join(", ", channels));

        /*
        // select first subscribed channel in alphabetical order
        if (this.chatClient.PublicChannels.Count > 0)
        {
            var l = new List<string>(this.chatClient.PublicChannels.Keys);
            l.Sort();
            string selected = l[0];
            if (this.channelToggles.ContainsKey(selected))
            {
                ShowChannel(selected);
                foreach (var c in this.channelToggles)
                {
                    c.Value.isOn = false;
                }
                this.channelToggles[selected].isOn = true;
                AddMessageToSelectedChannel(WelcomeText);
            }
        }
        */

        // Switch to the first newly created channel
        this.ShowChannel(channels[0]);
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (string channelName in channels)
        {
            if (this.channelToggles.ContainsKey(channelName))
            {
                Toggle t = this.channelToggles[channelName];
                Destroy(t.gameObject);

                this.channelToggles.Remove(channelName);

                Debug.Log("Unsubscribed from channel '" + channelName + "'.");

                // Showing another channel if the active channel is the one we unsubscribed from before
                if (channelName == this.selectedChannelName && this.channelToggles.Count > 0)
                {
                    IEnumerator<KeyValuePair<string, Toggle>> firstEntry = this.channelToggles.GetEnumerator();
                    firstEntry.MoveNext();

                    this.ShowChannel(firstEntry.Current.Key);

                    firstEntry.Current.Value.isOn = true;
                }
            }
            else
            {
                Debug.Log("Can't unsubscribe from channel '" + channelName +
                          "' because you are currently not subscribed to it.");
            }
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(this.selectedChannelName))
        {
            // update text
            this.ShowChannel(this.selectedChannelName);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // as the ChatClient is buffering the messages for you, this GUI doesn't need to do anything here
        // you also get messages that you sent yourself. in that case, the channelName is determinded by the target of your msg


        byte[] msgBytes = message as byte[];
        if (msgBytes != null)
        {
            Debug.Log("Message with byte[].Length: " + msgBytes.Length);
        }

        if (this.selectedChannelName.Equals(channelName))
        {
            this.ShowChannel(channelName);
        }
    }

    /// <summary>
    /// New status of another user (you get updates for users set in your friends list).
    /// </summary>
    /// <param name="user">Name of the user.</param>
    /// <param name="status">New status of that user.</param>
    /// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a
    /// message (keep any you have).</param>
    /// <param name="message">Message that user set.</param>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void AddMessageToSelectedChannel(string msg)
    {
        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(this.selectedChannelName, out channel);
        if (!found)
        {
            Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
            return;
        }

        if (channel != null)
        {
            channel.Add("Bot", msg, 0); //TODO: how to use msgID?
        }
    }


    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        this.selectedChannelName = channelName;
        chatDlg.CurrentChannelText.text = channel.ToStringMessages();
        Debug.Log("ShowChannel: " + this.selectedChannelName);

        foreach (KeyValuePair<string, Toggle> pair in this.channelToggles)
        {
            pair.Value.isOn = pair.Key == channelName ? true : false;
        }
    }
}