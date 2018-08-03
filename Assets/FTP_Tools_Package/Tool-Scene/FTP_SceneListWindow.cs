using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace FastToolsPackage
{
    public static class CreateScenePreviewList
    {
        [MenuItem("Assets/Create/Scene Preview List")]
        public static FTP_ScenePreviewData Create()
        {
            FTP_ScenePreviewData asset = ScriptableObject.CreateInstance<FTP_ScenePreviewData>();
            asset.m_sceneList = new List<ScenePreviewItemData>();

            if (!AssetDatabase.IsValidFolder("Assets/_ScenePreview"))
            {
                AssetDatabase.CreateFolder("Assets", "_ScenePreview");
                AssetDatabase.Refresh();
            }
            AssetDatabase.CreateAsset(asset, "Assets/_ScenePreview/ScenePreviewData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
    }

    public class FTP_SceneListWindow : EditorWindow
    {
        public FTP_ScenePreviewData m_sceneListData;
        public Vector2 m_PreviewSize = new Vector2(160, 90);

        private static string s_savePath = "scenepreviewpreviewPath";
        private static string s_width = "scenelaytoutwidth";
        [MenuItem("FTP_Tools/Scene Preview List")]
        static void ShowWindow()
        {
            EditorWindow wnd = EditorWindow.GetWindow(typeof(FTP_SceneListWindow));
            wnd.minSize = new Vector2(200, 300);
        }

        void OnUpdate()
        {
            Repaint();
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(s_savePath))
            {
                m_sceneListData = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString(s_savePath), typeof(FTP_ScenePreviewData)) as FTP_ScenePreviewData;
                m_PreviewSize.x = EditorPrefs.GetFloat(s_width, 160);
            }
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorPrefs.SetFloat(s_width, m_PreviewSize.x);
            EditorApplication.update -= OnUpdate;
        }

        private Vector2 m_scrollView = Vector2.zero;
        private void OnGUI()
        {
            float width = position.width;
            FTP_ScenePreviewData data = EditorGUILayout.ObjectField(m_sceneListData, typeof(FTP_ScenePreviewData), false) as FTP_ScenePreviewData;
            if (data != m_sceneListData)
            {
                // need to repaint
                m_sceneListData = data;
                string relPath = AssetDatabase.GetAssetPath(m_sceneListData);
                EditorPrefs.SetString(s_savePath, relPath);
                Repaint();
            }

            float f1 = EditorGUILayout.Slider("Width", m_PreviewSize.x, 160, 320);
            m_PreviewSize.x = Mathf.Clamp(f1, 160, 320);
            m_PreviewSize.y = m_PreviewSize.x / 16 * 9;

            // add all scenes to build settings
            if (GUILayout.Button("Add to Build Settings"))
            {
                if (m_sceneListData != null)
                {
                    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                    List<EditorBuildSettingsScene> listScene = new List<EditorBuildSettingsScene>();
                    listScene.AddRange(scenes);

                    foreach (var tmpData in m_sceneListData.m_sceneList)
                    {
                        if (tmpData.m_targetScene == null)
                            continue;

                        bool find = false;
                        string scenePath = AssetDatabase.GetAssetPath(tmpData.m_targetScene);
                        foreach (var scene in scenes)
                        {
                            if (scene.path.Equals(scenePath))
                            {
                                find = true;
                                break;
                            }
                        }
                        if (!find)
                        {
                            listScene.Add(new EditorBuildSettingsScene(scenePath, true));
                        }
                    }

                    EditorBuildSettings.scenes = listScene.ToArray();
                    Debug.Log("### Add scene to Build settings ###");
                }
            }

            m_scrollView = EditorGUILayout.BeginScrollView(m_scrollView);
            if (m_sceneListData != null)
            {
                int column = (int)(width / m_PreviewSize.x);
                int row = column != 0 ? m_sceneListData.m_sceneList.Count / column + 1 : 1;
                float preWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80;
                for (int i = 0; i < row; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < column; j++)
                    {
                        int index = i * column + j;
                        if (index >= m_sceneListData.m_sceneList.Count)
                            continue;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        ScenePreviewItemData sceneData = m_sceneListData.m_sceneList[index];
                        sceneData.m_title = EditorGUILayout.TextField("Title", sceneData.m_title);
                        sceneData.m_des = EditorGUILayout.TextField("Description", sceneData.m_des);
                        sceneData.m_targetScene = EditorGUILayout.ObjectField(sceneData.m_targetScene, typeof(SceneAsset), false) as SceneAsset;
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(sceneData.m_previewTexture, GUILayout.MaxHeight(m_PreviewSize.y), GUILayout.MaxWidth(m_PreviewSize.x)))
                        {
                            if (sceneData.m_targetScene != null)
                            {
                                Scene activeScene = EditorSceneManager.GetActiveScene();
                                string scenePath = AssetDatabase.GetAssetPath(sceneData.m_targetScene);

                                // check if in build settings
                                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                                bool find = false;
                                foreach (var scene in scenes)
                                {
                                    if (scene.path.Equals(scenePath))
                                    {
                                        find = true;
                                        break;
                                    }
                                }
                                if (!find)
                                {
                                    List<EditorBuildSettingsScene> listScenes = new List<EditorBuildSettingsScene>(scenes);
                                    listScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                                    EditorBuildSettings.scenes = listScenes.ToArray();
                                    Debug.Log("## Add to build settings scene list##");
                                }


                                if (!scenePath.Equals(activeScene.path))
                                {
                                    if (!Application.isPlaying)
                                    {
                                        bool open = EditorUtility.DisplayDialog("Open New Scene", "Want to Change Scene", "OK", "Cancle");
                                        if (open)
                                        {
                                            EditorSceneManager.OpenScene(scenePath);
                                            Debug.Log("# Load new scene:" + scenePath);
                                        }
                                    }
                                    else
                                    {
                                        // EditorSceneManager.LoadScene(scenePath, LoadSceneMode.Single);
                                        // Debug.Log("# Load new scene:" + scenePath);
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogError("## Unassign scene ##");
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUIUtility.labelWidth = preWidth;
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_sceneListData);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
