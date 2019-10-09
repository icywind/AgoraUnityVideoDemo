using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PermissionsTest : MonoBehaviour
{
    string[] mPermissions = { Permission.Camera, Permission.Microphone };

    void Start()
    {

#if PLATFORM_ANDROID
        foreach (var permit in mPermissions)
        {
            if (!Permission.HasUserAuthorizedPermission(permit))
            {

                Debug.LogWarning("Start: requesting permission " + permit);
                Permission.RequestUserPermission(permit);

            }
        }
#endif
    }
}


