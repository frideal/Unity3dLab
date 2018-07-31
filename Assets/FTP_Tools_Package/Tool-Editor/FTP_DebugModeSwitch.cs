using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace FastToolsPackage
{
    public class FTP_DebugModeSwitch
    {
        [MenuItem("FTP_Tools/Inspector Mode/To Normal")]
        public static void SetInspectorToNormal()
        {
            SetInspectorModel(InspectorMode.Normal);
        }

        [MenuItem("FTP_Tools/Inspector Mode/To Debug")]
        public static void SetInspectorToDebug()
        {
            SetInspectorModel(InspectorMode.Debug);
        }

        [MenuItem("FTP_Tools/Inspector Mode/To Debug Interal")]
        public static void SetInspectorToDebugInternal()
        {
            SetInspectorModel(InspectorMode.DebugInternal);
        }

        public static void SetInspectorModel(InspectorMode mode)
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow window = EditorWindow.GetWindow(type);
            MethodInfo info = null;
            if (mode == InspectorMode.Normal)
            {
                info = type.GetMethod("SetNormal", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else if (mode == InspectorMode.Debug)
            {
                info = type.GetMethod("SetDebug", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else if (mode == InspectorMode.DebugInternal)
            {
                info = type.GetMethod("SetDebugInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            if (info != null)
                info.Invoke(window, null);
        }
    }
}
