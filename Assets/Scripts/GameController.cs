using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;

public class GameController : MonoBehaviour
{
    private IRtcEngine mRtcEngine;
    [SerializeField]
    string mVendorKey = "";
    [SerializeField]
    string mChannelName = "";

    // Use this for initialization
    void Start()
    {
        Debug.Assert(!string.IsNullOrEmpty(mVendorKey), "API key is empty, please assign it in the gameobject");
        if (string.IsNullOrEmpty(mVendorKey)) { OnQuit(); }

        GameObject g = GameObject.Find("Join");
        Text text = g.GetComponentInChildren<Text>(true);
        text.text = "Join";
        Button button = g.GetComponent<Button>();
        button.onClick.AddListener(onJoinButtonClicked);

        g = GameObject.Find("Quit");
        if (g != null)
        {
            button = g.GetComponent<Button>();
            button.onClick.AddListener(OnQuit);
        }
    }


    private void OnApplicationQuit()
    {
        Debug.Log("Application quit safely.");
        mRtcEngine = IRtcEngine.getEngine(mVendorKey);
        if (mRtcEngine != null)
        {
            endCall();
        }
    }

    
    void onJoinButtonClicked()
    {
        GameObject g = GameObject.Find("Join");
        Text text = g.GetComponentInChildren<Text>(true);
        if (ReferenceEquals(mRtcEngine, null))
        {
            startCall();
            text.text = "Leave";
        }
        else
        {
            endCall();
            text.text = "Join";
        }
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void startCall()
    {
        // init engine
        mRtcEngine = IRtcEngine.getEngine(mVendorKey);
        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;

        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(mChannelName, null, 0);
    }

    void endCall()
    {
        Debug.Log("End call.");
        mRtcEngine.EnableLocalVideo(false);
        mRtcEngine.DisableVideo();
        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();

        IRtcEngine.Destroy();
        mRtcEngine = null;
    }

    // Callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " +uid);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " +uid);
        // this is called in the main thread

        // find a game object to render the video stream from ‘uid’
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign it to this new user
        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if (!ReferenceEquals(go, null))
        {
            go.name = uid.ToString();

            // configure videoSurface
            VideoSurface o = go.AddComponent<VideoSurface>();
            o.SetForUser(uid);
            o.mAdjustTransfrom += onTransformDelegate;
            o.SetEnable(true);
            //o.transform.Rotate(90.0f, 0.0f, 0.0f);
            o.transform.Rotate(-90.0f, 0.0f, 0.0f);
            float r = Random.Range(-5.0f, 5.0f);
            o.transform.position = new Vector3(0f, r, 0f);
            o.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }
    }

    // When a remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove the video stream
        Debug.Log("onUserOffline: uid = " +uid);
        // this is called in the main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }

    // Delegate: Adjust the transform for the game object ‘objName’ connected with the user ‘uid’
    // You can save information for ‘uid’ (e.g. which GameObject is attached)
    private void onTransformDelegate(uint uid, string objName, ref Transform transform)
    {
        if (uid == 0)
        {
            transform.position = new Vector3(0f, 2f, 0f);
            transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
            transform.Rotate(0f, 1f, 0f);
        }
        else
        {
            transform.Rotate(0.0f, 1.0f, 0.0f);
        }
    }
}
