using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [RequireComponent(typeof(SplineContainer))]
    public class DisplayCurvatureOnSpline : MonoBehaviour
    {
        [Serializable]
        public struct CurvatureConfig
        {
            [HideInInspector]
            [Obsolete("Use Display instead.", false)]
            public bool display;
            [FormerlySerializedAs("display")]
            public bool Display;

            [HideInInspector]
            [Obsolete("Use Time instead.", false)]
            public float time;
            [FormerlySerializedAs("time")]
            public float Time;
        }

        [HideInInspector]
        [Obsolete("Use CurvatureTimes instead.", false)]
        public List<CurvatureConfig> m_CurvatureTimes;
        [FormerlySerializedAs("m_CurvatureTimes")]
        public List<CurvatureConfig> CurvatureTimes = new List<CurvatureConfig>();

        SplineContainer m_Container;
        [Obsolete("Use Container instead.", false)]
        public SplineContainer container => Container;
        public SplineContainer Container
        {
            get
            {
                if (m_Container == null)
                    m_Container = GetComponent<SplineContainer>();
                return m_Container;
            }
        }
    }
}
