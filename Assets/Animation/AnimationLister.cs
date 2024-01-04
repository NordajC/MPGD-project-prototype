using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationLister : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Tools/Animation Lister")]
    private static void ShowWindow()
    {
        var window = GetWindow<AnimationLister>();
        window.titleContent = new GUIContent("Animation Lister");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Animations in Project", EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Retrieve all AnimationClips in the project
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (clip != null)
            {
                GUILayout.Label(clip.name);
            }
        }

        GUILayout.EndScrollView();
    }
}
