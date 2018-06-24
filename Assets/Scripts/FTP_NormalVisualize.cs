using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastToolsPackage
{
    //
    // Visualize Normal for vertex and face
    //
    public class FTP_NormalVisualize : MonoBehaviour
    {
        public enum NormalVisualizeType
        {
            None,
            Vertex,
            Surface
        }

        public NormalVisualizeType m_VisualizeType = NormalVisualizeType.Surface;

        [Range(0f, 1)]
        public float m_NormalLength = 0.1f;

        public Color m_NormalColor = Color.white;
        public bool m_ColorFromDirection = true;

        protected Mesh m_TargetMesh;
        private Transform m_Transform;

        protected virtual void OnDrawGizmos()
        {
            m_Transform = transform;
            if (m_VisualizeType == NormalVisualizeType.None)
                return;

            if (m_TargetMesh == null)
            {
                MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    this.m_TargetMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                }
                else
                {
                    this.m_TargetMesh = base.gameObject.GetComponent<MeshFilter>().sharedMesh;
                }
            }

            if (m_TargetMesh == null)
                return;

            Color previousColor = Gizmos.color;
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(m_Transform.position,
                                         m_Transform.rotation,
                                          m_Transform.localScale);
            // Gizmos.matrix = m_Transform.localToWorldMatrix;

            switch (this.m_VisualizeType)
            {
                case NormalVisualizeType.Surface:
                    {
                        DrawSurfaceNormalGizmos();
                        break;
                    }
                case NormalVisualizeType.Vertex:
                    {
                        DrawVertexNormalGizmos();
                        break;
                    }
            }

            Gizmos.color = previousColor;
            Gizmos.matrix = previousMatrix;
        }

        protected virtual void DrawVertexNormalGizmos()
        {
            Vector3[] vertices = this.m_TargetMesh.vertices;
            Vector3[] normals = this.m_TargetMesh.normals;
            Vector3 normal;

            Gizmos.color = this.m_NormalColor;

            for (int i = 0; i < this.m_TargetMesh.vertexCount; i++)
            {
                normal = Vector3.Normalize(normals[i]);

                if (this.m_ColorFromDirection)
                {
                    Gizmos.color = new Color(normal.x, normal.y, normal.z);
                }

                Gizmos.DrawRay(vertices[i], normal * this.m_NormalLength);
            }
        }

        protected virtual void DrawSurfaceNormalGizmos()
        {
            Vector3[] vertices = this.m_TargetMesh.vertices;
            Vector3[] normals = this.m_TargetMesh.normals;
            int[] triangles = this.m_TargetMesh.triangles;

            Vector3 normal;
            Vector3 position;

            int triangleIndex0;
            int triangleIndex1;
            int triangleIndex2;

            Gizmos.color = this.m_NormalColor;

            for (int i = 0; i <= triangles.Length - 3; i += 3)
            {
                triangleIndex0 = triangles[i];
                triangleIndex1 = triangles[i + 1];
                triangleIndex2 = triangles[i + 2];

                position = (vertices[triangleIndex0]
                          + vertices[triangleIndex1]
                          + vertices[triangleIndex2]) / 3;

                normal = (normals[triangleIndex0]
                        + normals[triangleIndex1]
                        + normals[triangleIndex2]) / 3;

                normal = Vector3.Normalize(normal);

                if (this.m_ColorFromDirection)
                {
                    Gizmos.color = new Color(normal.x, normal.y, normal.z);
                }

                Gizmos.DrawRay(position, normal * this.m_NormalLength);
            }
        }
    }
}

