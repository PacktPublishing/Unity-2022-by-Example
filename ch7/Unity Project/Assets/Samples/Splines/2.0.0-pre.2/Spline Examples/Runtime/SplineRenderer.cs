using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [RequireComponent(typeof(LineRenderer), typeof(SplineContainer))]
    public class SplineRenderer : MonoBehaviour
    {
        Spline m_Spline;
        LineRenderer m_Line;
        bool m_Dirty;
        Vector3[] m_Points;

        [SerializeField, Range(16, 512)]
        int m_Segments = 128;

        void Awake()
        {
            m_Spline = GetComponent<SplineContainer>().Spline;
            m_Line = GetComponent<LineRenderer>();
        }

        void OnEnable()
        {
            Spline.Changed += OnSplineChanged;
        }

        void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (m_Spline == spline)
                m_Dirty = true;
        }

        void Update()
        {
            // It's nice to be able to see resolution changes at runtime
            if (m_Points?.Length != m_Segments)
            {
                m_Dirty = true;
                m_Points = new Vector3[m_Segments];
                m_Line.loop = m_Spline.Closed;
                m_Line.positionCount = m_Segments;
            }

            if (!m_Dirty)
                return;

            m_Dirty = false;

            for (int i = 0; i < m_Segments; i++)
                m_Points[i] = m_Spline.EvaluatePosition(i / (m_Segments - 1f));

            m_Line.SetPositions(m_Points);
        }
    }
}
