using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    public class PointSplineData : MonoBehaviour
    {
        [SerializeField]
        SplineData<float2> m_Points = new SplineData<float2>();
        [Obsolete("Use Points instead.", false)]
        public SplineData<float2> points => Points;
        public SplineData<float2> Points => m_Points;

        public int Count => m_Points.Count;

        SplineContainer m_SplineContainer;
        [Obsolete("Use Container instead.", false)]
        public SplineContainer container => Container;
        public SplineContainer Container
        {
            get
            {
                if (m_SplineContainer == null)
                    m_SplineContainer = GetComponent<SplineContainer>();
                return m_SplineContainer;
            }
            set => m_SplineContainer = value;
        }
    }
}
