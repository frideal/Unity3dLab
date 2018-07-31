using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

//
// Unity Github Collection-Tools
//
namespace FastToolsPackage
{
    public class FTP_MissingScriptTools : EditorWindow
    {
        [MenuItem("FTP_Tools/Find Missing Script Tools", false, 4000)]
        public static void MissingScriptTools()
        {
            GetWindow<FTP_MissingScriptTools>();
        }

        protected List<GameObject> m_objectWithMissingScripts;
        protected Vector2 m_ScrollPosition = Vector2.zero;

        private void OnEnable()
        {
            m_objectWithMissingScripts = new List<GameObject>();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Missing Script in Assets"))
                FindInAssets();
            if (GUILayout.Button("Find Missing Script in Current Scene"))
                FindInScenes();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Missing List", MessageType.Error);
            GUILayout.BeginVertical(GUIContent.none, EditorStyles.helpBox);
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            for (int i = 0; i < m_objectWithMissingScripts.Count; ++i)
            {
                if (GUILayout.Button(m_objectWithMissingScripts[i].name))
                {
                    EditorGUIUtility.PingObject(m_objectWithMissingScripts[i]);
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void FindInAssets()
        {
            var assetGUIDs = AssetDatabase.FindAssets("t:GameObject");
            m_objectWithMissingScripts.Clear();
            Debug.Log("Check " + assetGUIDs.Length + " GameObject in Assets");
            foreach (string guid in assetGUIDs)
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                RecursiveDepthSearch(obj);
            }
        }

        private void RecursiveDepthSearch(GameObject root)
        {
            Component[] components = root.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                {
                    if (!m_objectWithMissingScripts.Contains(root))
                        m_objectWithMissingScripts.Add(root);
                }
            }

            foreach (Transform t in root.transform)
            {
                RecursiveDepthSearch(t.gameObject);
            }
        }

        private void FindInScenes()
        {
            m_objectWithMissingScripts.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var rootGos = SceneManager.GetSceneAt(i).GetRootGameObjects();
                Debug.Log("## Check " + rootGos.Length + " GameObjects in scene " + i);

                foreach (GameObject go in rootGos)
                {
                    RecursiveDepthSearch(go);
                }
            }
        }

        //
        // If U want to remove Prefab's missing script in the scene
        // U should break the Prefab connection
        //
        [MenuItem("FTP_Tools/Remove Missing Script in Current Scene", false, 40001)]
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
