using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace FastToolsPackage
{
    //
    // Auto Upgrade Legacy Material to Standard Material
    // 1. Open Scene
    // 2. Open Window
    // 3. Select Legacy material in the scene
    // 4. Start Process
    //


    // When Upgrade legacy material you should Use MaterialEditor to set the Standard Shader
    // if We directly set shader with .shader = standard shader
    // you need to select the material and Expand the material editor to force Unity refresh "something".
    // Unknown why, if you not expand the material editor inspector you will find your mesh texture will be wrong.

    public class FTP_UpgradeToStandardShaderTools : EditorWindow
    {
        [MenuItem("FTP_Tools/Upgrade To Standard Shader Tools", false, 3000)]
        public static void DoWindow()
        {
            FTP_UpgradeToStandardShaderTools window = GetWindow<FTP_UpgradeToStandardShaderTools>(false, "Upgradge To Standard Shader");
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400, 600);
            window.Show();
        }

        public List<Material> m_ListMaterials = new List<Material>();
        private Material curMaterial;
        private MaterialEditor curEditor = null;

        private void OnEnable()
        {
            EditorApplication.update += onUpdateContent;
            m_lastEditorTime = Time.realtimeSinceStartup;
        }

        private void OnDisable()
        {
            EditorApplication.update -= onUpdateContent;
        }

        private void onUpdateContent()
        {
            float dTime = Time.realtimeSinceStartup - m_lastEditorTime;
            Repaint();
            if (m_workingChange)
            {
                m_intervalTime -= dTime;
            }

            if (m_workingChange)
            {
                if (m_workIndex >= m_ListMaterials.Count)
                {
                    m_workingChange = false;
                    Debug.Log("<color=green> Process Over </color>");
                }
                else
                {
                    if (m_intervalTime <= 0.0f)
                    {
                        if (curEditor != null)
                        {
                            DestroyImmediate(curEditor);
                            curEditor = null;
                        }
                        Material mat = m_ListMaterials[m_workIndex];
                        ChangeEditor(mat);
                        m_workIndex++;
                        m_intervalTime = intervalTime;
                        Debug.Log(" # start to process " + m_workIndex);
                    }
                }
            }

            m_lastEditorTime = Time.realtimeSinceStartup;
        }

        private Vector2 m_scrollPos = Vector2.zero;

        private bool m_workingChange = false;
        private int m_workIndex = 0;
        private float m_lastEditorTime;
        private float m_intervalTime = 1.0f;
        private static float intervalTime = 0.2f;

        // change Legacy Shader
        Shader shaderLegacyBumped;
        Shader shaderLegacyBumpedDiffuse;
        Shader shaderLegacySpecular;
        Shader shaderLegacyTransparentBumped;
        Shader shaderLegacyTransparentDiffuse;
        Shader shaderLegacyReflectiveBumpedSpecular;
        Shader shaderLegacySelfEmission;
        Shader shaderLegacyReflectiveSpecular;

        Shader standard;
        Shader standardSpecular;

        void FindShader()
        {
            shaderLegacyBumped = Shader.Find("Legacy Shaders/Bumped Specular");
            shaderLegacyBumpedDiffuse = Shader.Find("Legacy Shaders/Bumped Diffuse");
            shaderLegacySpecular = Shader.Find("Legacy Shaders/Specular");
            shaderLegacyTransparentBumped = Shader.Find("Legacy Shaders/Transparent/Bumped Specular");
            shaderLegacyReflectiveBumpedSpecular = Shader.Find("Legacy Shaders/Reflective/Bumped Specular");
            shaderLegacyTransparentDiffuse = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            shaderLegacySelfEmission = Shader.Find("Legacy Shaders/Self-Illumin/Specular");
            shaderLegacyReflectiveSpecular = Shader.Find("Legacy Shaders/Reflective/Specular");
            standard = Shader.Find("Standard");
            standardSpecular = Shader.Find("Standard (Specular setup)");
        }

        private void OnGUI()
        {
            GUI.enabled = !m_workingChange;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
            for (int i = 0; i < m_ListMaterials.Count; i++)
            {
                m_ListMaterials[i] = EditorGUILayout.ObjectField(m_ListMaterials[i], typeof(Material), true) as Material;
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Select All Scene Target Legacy Material"))
            {
                FindShader();
                m_ListMaterials.Clear();
                List<GameObject> sceneList = new List<GameObject>();
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                        continue;

                    if (EditorUtility.IsPersistent(go.transform.root.gameObject))
                        continue;

                    MeshRenderer meshRender = go.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer skinMeshRender = go.GetComponent<SkinnedMeshRenderer>();

                    if (meshRender == null && skinMeshRender == null)
                    {
                        continue;
                    }

                    Material[] mats = meshRender == null ? skinMeshRender.sharedMaterials : meshRender.sharedMaterials;
                    foreach (Material mat in mats)
                    {
                        string matPath = AssetDatabase.GetAssetPath(mat);
                        Material diskMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                        if (!m_ListMaterials.Contains(diskMaterial))
                        {
                            bool needProcess = CheckLegacyShader(diskMaterial);
                            if (!needProcess)
                                continue;
                            m_ListMaterials.Add(diskMaterial);
                        }
                    }
                }
            }

            if (GUILayout.Button("Upgrade To Standard Material"))
            {
                m_workIndex = 0;
                m_workingChange = true;
                m_intervalTime = intervalTime;
            }

            GUI.enabled = m_workingChange;
            if (GUILayout.Button("Stop Upgrade"))
            {
                m_workingChange = false;
                m_workIndex = 0;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (curEditor != null)
            {
                curEditor.DrawPreview(new Rect(20, 20, 64, 64));
                curEditor.PropertiesGUI();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private bool CheckLegacyShader(Material material)
        {
            if (material == null)
                return false;
            Shader shader = material.shader;
            if (shader == null)
                return false;

            return (
                shader == shaderLegacyBumped ||
                shader == shaderLegacyBumpedDiffuse ||
                shader == shaderLegacyReflectiveBumpedSpecular ||
                shader == shaderLegacyReflectiveSpecular ||
                shader == shaderLegacySelfEmission ||
                shader == shaderLegacySpecular ||
                shader == shaderLegacyTransparentBumped ||
                shader == shaderLegacyTransparentDiffuse);
        }

        private void ChangeEditor(Material material)
        {
            FindShader();
            if (material == null)
                return;
            Shader legacyShader = material.shader;

            curMaterial = material;
            curEditor = Editor.CreateEditor(material) as MaterialEditor;

            if (legacyShader == shaderLegacyTransparentBumped)
            {
                curEditor.SetShader(standardSpecular, false);
            }
            else if (legacyShader == shaderLegacyBumpedDiffuse || legacyShader == shaderLegacyBumped)
            {
                curEditor.SetShader(standard, false);
                curEditor.SetFloat("_Metallic", 0.5f);
                curEditor.SetFloat("_SmoothnessTextureChannel", 1);
            }
            else if (legacyShader == shaderLegacySpecular || legacyShader == shaderLegacyTransparentDiffuse)
            {
                curEditor.SetShader(standardSpecular, false);
                curEditor.SetFloat("_SmoothnessTextureChannel", 1);
            }
            else if (legacyShader == shaderLegacyReflectiveSpecular)
            {
                curEditor.SetShader(standardSpecular, false);
                curEditor.SetFloat("_Smoothness", 0.5f);
            }
            else if (legacyShader == shaderLegacyReflectiveBumpedSpecular)
            {
                curEditor.SetShader(standard, false);
                curEditor.SetFloat("_Metallic", 0.75f);
                curEditor.SetFloat("_SmoothnessTextureChannel", 1);
            }
            else if (legacyShader == shaderLegacySelfEmission)
            {
                bool mixedTex = false;
                Texture emission = curEditor.GetTexture("_MainTex", out mixedTex);
                curEditor.SetShader(standard, false);
                curEditor.SetFloat("_SmoothnessTextureChannel", 1);

                curMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                curMaterial.EnableKeyword("_EMISSION");
                curEditor.SetTexture("_EmissionMap", emission);
                curEditor.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1.0f));

                FieldInfo fi = typeof(MaterialEditor).GetField("m_CustomShaderGUI", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi != null)
                {
                    object obj = fi.GetValue(curEditor);
                    Type shaderGUI = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.StandardShaderGUI");

                    MethodInfo mi = shaderGUI.GetMethod("SetKeyword", BindingFlags.NonPublic | BindingFlags.Static);
                    if (mi != null)
                    {
                        mi.Invoke(null, new object[] { curMaterial, "_EMISSION", true });
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
