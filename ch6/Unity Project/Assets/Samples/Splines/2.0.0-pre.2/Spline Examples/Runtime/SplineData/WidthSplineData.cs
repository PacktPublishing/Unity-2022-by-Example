using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [CanEditMultipleObjects]
    public class WidthSplineData : MonoBehaviour
    {
        [HideInInspector]
        [Obsolete("No longer used.", false)]
        public float m_DefaultWidth;

        [SerializeField]
        SplineData<float> m_Width = new SplineData<float>();

        [Obsolete("Use Width instead.", false)]
        public SplineData<float> width => Width;
        public SplineData<float> Width
        {
            get
            {
                if (m_Width.DefaultValue == 0)
                    m_Width.DefaultValue = 1f;
                return m_Width;
            }
        }

        public int Count => m_Width.Count;

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
