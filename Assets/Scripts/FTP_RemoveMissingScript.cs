using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace FastToolsPackage
{
    public class FTP_RemoveMissingScript
    {
        [MenuItem("FTP_Tools/Remove Missing Script")]
        static void RemoveMissingScript()
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allObjects)
            {
                var components = gameObject.GetComponents<Component>();
                var serializedObject = new SerializedObject(gameObject);
                var prop = serializedObject.FindProperty("m_Component");
                int r = 0;
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                    {
                        EditorSceneManager.MarkSceneDirty(gameObject.scene);
                        prop.DeleteArrayElementAtIndex(j - r);
                        r++;
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
