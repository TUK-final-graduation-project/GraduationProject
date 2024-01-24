using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; // Add this line
using UnityEngine;

public class PluginTest : MonoBehaviour
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    const string dll = "NativeUnityPlugin"; // Change this according to your actual DLL name for Windows
#else
    const string dll = "__Internal";
#endif

    [DllImport(dll)]
    private static extern void SeedRandomizer();

    [DllImport(dll)]
    private static extern float Add(float a, float b);

    [DllImport(dll)]
    private static extern float GetRandom(); // Changed the function name to avoid conflicts with Unity's Random class

    // Start is called before the first frame update
    void Start()
    {
        SeedRandomizer();

        Debug.Log(Add(3.2f, 4.5f).ToString());
        Debug.Log(GetRandom().ToString());
    }
}