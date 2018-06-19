using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastToolsPackage
{
    [ExecuteInEditMode]
    public class FTP_GizmoScript : MonoBehaviour
    {
        public enum GizmoType
        {
            Box,
            Icon,
            Sphere,
            WireCube,
            WireSphere,
            Arrow,
            Cone,
        }

        public Color _color = new Color(0.2f, 0.3f, 1f, 1f);
        public float _radius = 0.5f;
        public bool _selectedVisible = false;
        public bool _enableGizmo = true;
        public GizmoType _gizmoType = GizmoType.Sphere;

        // Cone and arrow
        [Range(5f, 60f)]
        public float _coneAngle = 45.0f;

        [Range(0.5f, 100f)]
        public float _length = 1.0f;

        [Range(0.2f, 0.8f)]
        public float _arrowConeLength = 0.2f;

        private void OnDrawGizmos()
        {
            if (!_selectedVisible && _enableGizmo)
            {
                RealDrawGizmo();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_selectedVisible && _enableGizmo)
            {
                RealDrawGizmo();
            }
        }

        private void RealDrawGizmo()
        {
            switch (_gizmoType)
            {
                case GizmoType.Sphere:
                case GizmoType.Box:
                case GizmoType.WireCube:
                case GizmoType.WireSphere:
                    DrawSimpleGeometry(_gizmoType, transform.position, _radius);
                    break;
                case GizmoType.Arrow:
                    DrawArrow(transform.position, transform.forward * _length, _color);
                    break;
                case GizmoType.Cone:
                    // Spot Light
                    Light spotLight = GetComponent<Light>();
                    if (spotLight != null && spotLight.type == LightType.Spot)
                    {
                        _coneAngle = spotLight.spotAngle * 0.5f;
                        _length = spotLight.range;
                    }
                    DrawCone(transform.position, transform.forward * _length, _color, _coneAngle);
                    break;
                case GizmoType.Icon:
                    break;
                default: break;
            }
        }

        private void DrawSimpleGeometry(GizmoType gizmoType, Vector3 position, float radius)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = _color;
            switch (gizmoType)
            {
                case GizmoType.Sphere:
                    Gizmos.DrawSphere(position, radius);
                    Gizmos.color = preColor;
                    break;
                case GizmoType.Box:
                    Gizmos.DrawCube(position, new Vector3(radius, radius, radius));
                    Gizmos.color = preColor;
                    break;
                case GizmoType.WireCube:
                    Gizmos.DrawWireCube(position, new Vector3(radius, radius, radius));
                    Gizmos.color = preColor;
                    break;
                case GizmoType.WireSphere:
                    Gizmos.DrawWireSphere(position, radius);
                    Gizmos.color = preColor;
                    break;
                default: break;
            }
        }

        private void DrawArrow(Vector3 position, Vector3 dir, Color color)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawRay(position, dir);
            Gizmos.DrawWireSphere(position, 0.05f);

            DrawCone(position + dir, -dir * _arrowConeLength, color, _coneAngle);
            Gizmos.color = preColor;
        }

        private void DrawCone(Vector3 position, Vector3 dir, Color color, float angle)
        {
            float length = dir.magnitude;
            Vector3 _forward = dir;
            Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
            Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;
            dir = dir.normalized;
            Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);
            float dist;
            var farPlane = new Plane(-dir, position + _forward);
            var distRay = new Ray(position, slerpedVector);
            farPlane.Raycast(distRay, out dist);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawRay(position, slerpedVector.normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist);

            DrawCircle(position + _forward, dir, color, (_forward - (slerpedVector.normalized * dist)).magnitude);
            DrawCircle(position + (_forward * 0.5f), dir, color, (_forward * 0.5f - (slerpedVector.normalized * (dist * 0.5f))).magnitude);
            Gizmos.color = oldColor;
        }

        public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f)
        {
            up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
            Vector3 _forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 _right = Vector3.Cross(up, _forward).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            Color oldColor = Gizmos.color;
            Gizmos.color = (color == default(Color)) ? Color.white : color;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

                Gizmos.DrawLine(_lastPoint, _nextPoint);
                _lastPoint = _nextPoint;
            }
            Gizmos.color = oldColor;
        }
    }
}
