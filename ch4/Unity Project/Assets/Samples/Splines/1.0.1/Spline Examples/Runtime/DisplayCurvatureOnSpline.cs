using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [RequireComponent(typeof(SplineContainer))]
    public class DisplayCurvatureOnSpline : MonoBehaviour
    {
        [Serializable]
        public struct CurvatureConfig
        {
            public bool display;
            public float time;
        }
        
        public List<CurvatureConfig> m_CurvatureTimes = new List<CurvatureConfig>();

        SplineContainer m_Container; 
        public SplineContainer container
        {
            get
            {
                if(m_Container == null)
                    m_Container = GetComponent<SplineContainer>();
                return m_Container;
            }
        }
    }
}
