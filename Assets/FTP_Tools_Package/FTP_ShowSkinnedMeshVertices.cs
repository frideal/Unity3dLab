using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastToolsPackage
{
    //
    // Visualize Skinned Mesh Vertex
    //
    public class FTP_ShowSkinnedMeshVertices : MonoBehaviour
    {
        public bool _showVertices = false;
        public SkinnedMeshRenderer _skinnedMesh;

        [Range(0, 1)]
        public float m_Size = 0.01f;
        public Color m_DrawColor = Color.green;
        public int m_MaxVertexCount = 6000;

        private void OnDrawGizmos()
        {
            if (_showVertices && _skinnedMesh != null)
            {
                Mesh mesh = _skinnedMesh.sharedMesh;
                Matrix4x4[] bindPoses = mesh.bindposes;
                Transform[] bones = _skinnedMesh.bones;
                BoneWeight[] boneWeights = mesh.boneWeights;

                Vector3[] vertices = mesh.vertices;
                for (int index = 0; index < vertices.Length; index++)
                {
                    Vector3 verCurPos = Vector3.zero;
                    Vector3 bindLocalPos = vertices[index];
                    BoneWeight weight = boneWeights[index];

                    if (weight.weight0 != 0)
                    {
                        Matrix4x4 matrix = bones[weight.boneIndex0].localToWorldMatrix * bindPoses[weight.boneIndex0];
                        verCurPos += matrix.MultiplyPoint(bindLocalPos) * weight.weight0;
                    }

                    if (weight.weight1 != 0)
                    {
                        Matrix4x4 matrix = bones[weight.boneIndex1].localToWorldMatrix * bindPoses[weight.boneIndex1];
                        verCurPos += matrix.MultiplyPoint(bindLocalPos) * weight.weight1;
                    }

                    if (weight.weight2 != 0)
                    {
                        Matrix4x4 matrix = bones[weight.boneIndex2].localToWorldMatrix * bindPoses[weight.boneIndex2];
                        verCurPos += matrix.MultiplyPoint(bindLocalPos) * weight.weight2;
                    }

                    if (weight.weight3 != 0)
                    {
                        Matrix4x4 matrix = bones[weight.boneIndex3].localToWorldMatrix * bindPoses[weight.boneIndex3];
                        verCurPos += matrix.MultiplyPoint(bindLocalPos) * weight.weight3;
                    }

                    if (index >= m_MaxVertexCount)
                        return;
                    Color preColor = Gizmos.color;
                    Gizmos.color = m_DrawColor;
                    Gizmos.DrawSphere(verCurPos, m_Size);
                    Gizmos.color = preColor;
                }
            }
        }
    }
}

