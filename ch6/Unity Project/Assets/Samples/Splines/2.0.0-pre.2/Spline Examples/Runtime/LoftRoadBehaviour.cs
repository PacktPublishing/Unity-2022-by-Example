#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Splines;
#endif

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Interpolators = UnityEngine.Splines.Interpolators;

namespace Unity.Splines.Examples
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SplineContainer), typeof(MeshRenderer), typeof(MeshFilter))]
    public class LoftRoadBehaviour : MonoBehaviour
    {
        [SerializeField]
        SplineContainer m_Spline;

        [SerializeField]
        int m_SegmentsPerMeter = 1;

        [SerializeField]
        Mesh m_Mesh;

        [SerializeField]
        float m_TextureScale = 1f;

        WidthSplineData m_WidthData;

        WidthSplineData widthData
        {
            get
            {
                if (m_WidthData == null)
                    m_WidthData = GetComponent<WidthSplineData>();
                if (m_WidthData == null)
                    m_WidthData = gameObject.AddComponent<WidthSplineData>();

                if (m_WidthData.Container == null)
                    m_WidthData.Container = m_Spline;

                return m_WidthData;
            }
        }

        [Obsolete("Use LoftSpline instead.", false)]
        public Spline spline => LoftSpline;
        public Spline LoftSpline
        {
            get
            {
                if (m_Spline == null)
                    m_Spline = GetComponent<SplineContainer>();
                if (m_Spline == null)
                {
                    Debug.LogError("Cannot loft road mesh because Spline reference is null");
                    return null;
                }
                return m_Spline.Spline;
            }
        }

        [Obsolete("Use LoftMesh instead.", false)]
        public Mesh mesh => LoftMesh;
        public Mesh LoftMesh
        {
            get
            {
                if (m_Mesh != null)
                    return m_Mesh;

                m_Mesh = new Mesh();
                GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Road");
                return m_Mesh;
            }
        }

        [Obsolete("Use SegmentsPerMeter instead.", false)]
        public int segmentsPerMeter => SegmentsPerMeter;
        public int SegmentsPerMeter => Mathf.Min(10, Mathf.Max(1, m_SegmentsPerMeter));

        List<Vector3> m_Positions = new List<Vector3>();
        List<Vector3> m_Normals = new List<Vector3>();
        List<Vector2> m_Textures = new List<Vector2>();
        List<int> m_Indices = new List<int>();

        public void OnEnable()
        {
            //Avoid to point to an existing instance when duplicating the GameObject
            if (m_Mesh != null)
                m_Mesh = null;

            if (m_WidthData == null)
                m_WidthData = GetComponent<WidthSplineData>();
            if (m_WidthData == null)
                m_WidthData = gameObject.AddComponent<WidthSplineData>();

            Loft();
#if UNITY_EDITOR
            EditorSplineUtility.AfterSplineWasModified += OnAfterSplineWasModified;
            EditorSplineUtility.RegisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Undo.undoRedoPerformed += Loft;
#endif
        }

        public void OnDisable()
        {
#if UNITY_EDITOR
            EditorSplineUtility.AfterSplineWasModified -= OnAfterSplineWasModified;
            EditorSplineUtility.UnregisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Undo.undoRedoPerformed -= Loft;
#endif

            if (m_Mesh != null)
#if  UNITY_EDITOR
                DestroyImmediate(m_Mesh);
#else
                Destroy(m_Mesh);
#endif
        }

        void OnAfterSplineWasModified(Spline s)
        {
            if (s == LoftSpline)
                Loft();
        }

        void OnAfterSplineDataWasModified(SplineData<float> splineData)
        {
            if (splineData == m_WidthData.Width)
                Loft();
        }

        public void Loft()
        {
            if (LoftSpline == null || LoftSpline.Count < 2)
                return;

            LoftMesh.Clear();

            float length = LoftSpline.GetLength();

            if (length < 1)
                return;

            int segments = (int)(SegmentsPerMeter * length);
            int vertexCount = segments * 2, triangleCount = (LoftSpline.Closed ? segments : segments - 1) * 6;

            m_Positions.Clear();
            m_Normals.Clear();
            m_Textures.Clear();
            m_Indices.Clear();

            m_Positions.Capacity = vertexCount;
            m_Normals.Capacity = vertexCount;
            m_Textures.Capacity = vertexCount;
            m_Indices.Capacity = triangleCount;

            for (int i = 0; i < segments; i++)
            {
                var index = i / (segments - 1f);
                var control = SplineUtility.EvaluatePosition(LoftSpline, index);
                var dir = SplineUtility.EvaluateTangent(LoftSpline, index);
                var up = SplineUtility.EvaluateUpVector(LoftSpline, index);

                var scale = transform.lossyScale;
                //var tangent = math.normalize((float3)math.mul(math.cross(up, dir), new float3(1f / scale.x, 1f / scale.y, 1f / scale.z)));
                var tangent = math.normalize(math.cross(up, dir)) * new float3(1f / scale.x, 1f / scale.y, 1f / scale.z);

                var w = widthData.Width.DefaultValue;
                if (widthData.Width != null && widthData.Count > 0)
                {
                    w = widthData.Width.Evaluate(LoftSpline, index, PathIndexUnit.Normalized, new Interpolators.LerpFloat());
                    w = math.clamp(w, .001f, 10000f);
                }

                m_Positions.Add(control - (tangent * w));
                m_Positions.Add(control + (tangent * w));
                m_Normals.Add(Vector3.up);
                m_Normals.Add(Vector3.up);
                m_Textures.Add(new Vector2(0f, index * m_TextureScale));
                m_Textures.Add(new Vector2(1f, index * m_TextureScale));
            }

            for (int i = 0, n = 0; i < triangleCount; i += 6, n += 2)
            {
                m_Indices.Add((n + 2) % vertexCount);
                m_Indices.Add((n + 1) % vertexCount);
                m_Indices.Add((n + 0) % vertexCount);
                m_Indices.Add((n + 2) % vertexCount);
                m_Indices.Add((n + 3) % vertexCount);
                m_Indices.Add((n + 1) % vertexCount);
            }

            LoftMesh.SetVertices(m_Positions);
            LoftMesh.SetNormals(m_Normals);
            LoftMesh.SetUVs(0, m_Textures);
            LoftMesh.subMeshCount = 1;
            LoftMesh.SetIndices(m_Indices, MeshTopology.Triangles, 0);
            LoftMesh.UploadMeshData(false);

            GetComponent<MeshFilter>().sharedMesh = m_Mesh;
        }
    }
}
