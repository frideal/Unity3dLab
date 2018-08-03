using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace FastToolsPackage
{
    [System.Serializable]
    public class ScenePreviewItemData
    {
        public SceneAsset m_targetScene;
        public string m_title;
        public string m_des;
        public Texture2D m_previewTexture;
    }

    public class FTP_ScenePreviewData : ScriptableObject
    {
        public List<ScenePreviewItemData> m_sceneList;
    }
}
