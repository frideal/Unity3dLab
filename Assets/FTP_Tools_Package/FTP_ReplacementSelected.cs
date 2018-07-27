using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//
// http://wiki.unity3d.com/index.php/ReplaceSelection
//
namespace FastToolsPackage
{
    public class FTP_ReplacementSelected : ScriptableWizard
    {
        static GameObject s_replacement;
        static bool s_keepOriginals = false;

        public GameObject m_replacement = null;
        public bool m_keepOriginals = false;

        [MenuItem("FTP_Tools/Replace Selected... _%#R")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard("Replacement Selection", typeof(FTP_ReplacementSelected), "Replacement");
        }

        public FTP_ReplacementSelected()
        {
            m_replacement = s_replacement;
            m_keepOriginals = s_keepOriginals;
        }

        private void OnWizardUpdate()
        {
            s_replacement = m_replacement;
            s_keepOriginals = m_keepOriginals;
        }

        private void OnWizardCreate()
        {
            if (m_replacement == null)
                return;

            Transform[] targets = Selection.GetTransforms(SelectionMode.OnlyUserModifiable);

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject newObj;
                PrefabType type = PrefabUtility.GetPrefabType(m_replacement);
                if (type == PrefabType.Prefab || type == PrefabType.ModelPrefab)
                {
                    newObj = PrefabUtility.InstantiatePrefab(m_replacement) as GameObject;
                }
                else
                {
                    newObj = Editor.Instantiate(m_replacement) as GameObject;
                }

                Transform newTrs = newObj.transform;
                newTrs.parent = targets[i].parent;
                newTrs.name = m_replacement.name;
                newTrs.localPosition = targets[i].localPosition;
                newTrs.localScale = targets[i].localScale;
                newTrs.localRotation = targets[i].localRotation;

                Undo.RegisterCreatedObjectUndo(newObj, "Undo Replacement");
            }
             
            if (!m_keepOriginals)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go != null)
                        Undo.DestroyObjectImmediate(go);
                }
            }
        }
    }
}
