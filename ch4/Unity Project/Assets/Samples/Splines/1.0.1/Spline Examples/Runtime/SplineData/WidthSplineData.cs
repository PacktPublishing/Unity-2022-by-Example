using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    public class WidthSplineData : MonoBehaviour
    {
        public float m_DefaultWidth = 1f;
        
        [SerializeField]
        SplineData<float> m_Width;
        
        public SplineData<float> width => m_Width;

        public int Count => m_Width.Count;
        
        SplineContainer m_SplineContainer;
        
        public SplineContainer container
        {
            get
            {
                if(m_SplineContainer == null)
                    m_SplineContainer = GetComponent<SplineContainer>();
                return m_SplineContainer;
            }
            set => m_SplineContainer = value;
        }
    }
}
