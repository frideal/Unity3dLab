using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class FTP_FBXTools : EditorWindow {

    [MenuItem("FTP_Tools/Force To Bind Pose", false, 2008)]
    public static void DoWindow()
    {
        FTP_FBXTools window = CreateInstance<FTP_FBXTools>();
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400, 600);
        window.Show();
    }

    private GameObject _targetObject;

    private void OnGUI()
    {
        _targetObject = EditorGUILayout.ObjectField("Target", _targetObject, typeof(GameObject), true) as GameObject;
        GUILayout.Space(3);
        if (GUILayout.Button("Set to Bind Pose", GUILayout.MinHeight(50)) && _targetObject != null)
        {
            ReflectionRestoreToBindPose(_targetObject);
        }
    }

    // 
    // Use Unity3d reflection method to restore bind pose
    //
    private void ReflectionRestoreToBindPose(GameObject target)
    {
        if (target == null)
            return;

        Type type = Type.GetType("UnityEditor.AvatarSetupTool, UnityEditor");
        if (type != null)
        {
            MethodInfo info = type.GetMethod("SampleBindPose", BindingFlags.Static | BindingFlags.Public);
            if (info != null)
            {
                info.Invoke(null, new object[] { target });
            }
        }
    }
}
