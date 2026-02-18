using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorScript : MonoBehaviour
{
#if UNITY_EDITOR
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/StartMenu.unity", new LoadSceneParameters(LoadSceneMode.Single));
    }
#endif
}
