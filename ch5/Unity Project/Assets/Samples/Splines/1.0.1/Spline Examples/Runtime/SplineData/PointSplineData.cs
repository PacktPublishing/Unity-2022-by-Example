using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    public class PointSplineData : MonoBehaviour
    {
        [SerializeField]
        SplineData<float2> m_Points;
        public SplineData<float2> points => m_Points;
        
        public int Count => m_Points.Count;
       
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
