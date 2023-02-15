using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    public class DriftSplineData : MonoBehaviour
    {
        public float m_Default = 0f;
        
        [SerializeField]
        SplineData<float> m_Drift;
        
        public SplineData<float> drift => m_Drift;
        
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
