using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif



public class PermissionsTest : MonoBehaviour
{
#if PLATFORM_ANDROID
    string[] mPermissions = { Permission.Camera, Permission.Microphone };

    void Start()
    {


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


