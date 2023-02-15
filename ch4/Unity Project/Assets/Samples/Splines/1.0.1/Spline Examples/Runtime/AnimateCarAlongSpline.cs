using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Splines;
using Unity.Mathematics;
using Interpolators = UnityEngine.Splines.Interpolators;
using Quaternion = UnityEngine.Quaternion;

namespace Unity.Splines.Examples
{
    public class AnimateCarAlongSpline: MonoBehaviour
    {
        [SerializeField]
        SplineContainer m_SplineContainer;
        public SplineContainer splineContainer => m_SplineContainer;
        
        [SerializeField]
        Car m_CarToAnimate;

        [Min(0f)]
        public float m_DefaultSpeed = 10f;
        [Min(0f)]
        public float m_MaxSpeed = 30f;

        [SerializeField]
        SplineData<float> m_Speed = new SplineData<float>();
        public SplineData<float> speed => m_Speed;
        
        public Vector3 m_DefaultTilt = Vector3.up;
        [SerializeField]
        SplineData<float3> m_Tilt = new SplineData<float3>();
        public SplineData<float3> tilt => m_Tilt;
        
        [SerializeField]
        DriftSplineData m_Drift;
        public DriftSplineData driftData
        {
            get
            {
                if(m_Drift == null)
                    m_Drift = GetComponent<DriftSplineData>();

                return m_Drift;
            }
        }

        [SerializeField]
        PointSplineData m_LookAtPoints;
        public PointSplineData lookAtPoints
        {
            get
            {
                if(m_LookAtPoints == null)
                    m_LookAtPoints = GetComponent<PointSplineData>();

                return m_LookAtPoints;
            }
        }

        [SerializeField] 
        Transform m_LookTransform;

        float m_CurrentOffset;
        float m_CurrentSpeed;
        float m_SplineLength;
        Spline m_Spline;

        public void Initialize()
        {
            //Trying to initialize either the spline container or the car
            if(m_SplineContainer == null && !TryGetComponent<SplineContainer>(out m_SplineContainer))
                if(m_CarToAnimate == null)
                    TryGetComponent<Car>(out m_CarToAnimate);
        }
        
        void Start()
        {
            Assert.IsNotNull(m_SplineContainer);

            m_Spline = m_SplineContainer.Spline;
            m_SplineLength = m_Spline.GetLength();
            m_CurrentOffset = 0f;
        }

        void OnValidate()
        {
            if(m_Speed != null)
            {
                for(int index = 0; index < m_Speed.Count; index++)
                {
                    var data = m_Speed[index];

                    //We don't want to have a value that is negative or null as it might block the simulation
                    if(data.Value <= 0)
                    {
                        data.Value = m_DefaultSpeed;
                        m_Speed[index] = data;
                    }
                }
            }

            if(m_Tilt != null)
            {
                for(int index = 0; index < m_Tilt.Count; index++)
                {
                    var data = m_Tilt[index];

                    //We don't want to have a up vector of magnitude 0
                    if(math.length(data.Value) == 0)
                    {
                        data.Value = m_DefaultTilt;
                        m_Tilt[index] = data;
                    }
                }
            }

            if(lookAtPoints != null)
                lookAtPoints.container = splineContainer;

            if(driftData != null)
                driftData.container = splineContainer;
        }

        void Update()
        {
            if(m_SplineContainer == null || m_CarToAnimate == null)
                return;
            
            m_CurrentOffset = (m_CurrentOffset + m_CurrentSpeed * Time.deltaTime / m_SplineLength) % 1f;
            
            if (m_Speed.Count > 0)
                m_CurrentSpeed = m_Speed.Evaluate(m_Spline, m_CurrentOffset, PathIndexUnit.Normalized, new Interpolators.LerpFloat());
            else
                m_CurrentSpeed = m_DefaultSpeed;

            var posOnSplineLocal = SplineUtility.EvaluatePosition(m_Spline, m_CurrentOffset);
            var direction = SplineUtility.EvaluateTangent(m_Spline, m_CurrentOffset);
            var upSplineDirection = SplineUtility.EvaluateUpVector(m_Spline, m_CurrentOffset);
            var right = math.normalize(math.cross(upSplineDirection, direction));
            var driftOffset = 0f;
            if(driftData != null)
            {
                driftOffset = driftData.drift.Count == 0 ?
                    driftData.m_Default : 
                    driftData.drift.Evaluate(m_Spline, m_CurrentOffset, PathIndexUnit.Normalized, new Interpolators.LerpFloat());
            }
            
            m_CarToAnimate.transform.position = m_SplineContainer.transform.TransformPoint(posOnSplineLocal + driftOffset * right);

            var up = 
                (m_Tilt == null  || m_Tilt.Count == 0) ?
                    m_DefaultTilt : 
                    (Vector3)m_Tilt.Evaluate(m_Spline, m_CurrentOffset,PathIndexUnit.Normalized, new Interpolators.LerpFloat3());

            var rot = Quaternion.LookRotation(direction, upSplineDirection);
            m_CarToAnimate.transform.rotation = Quaternion.LookRotation(direction, rot * up);

            if (lookAtPoints != null && m_LookTransform != null && m_LookAtPoints.Count > 0)
            {
                var lookAtPoint = m_LookAtPoints.points.Evaluate(m_Spline, m_CurrentOffset, PathIndexUnit.Normalized, new Interpolators.LerpFloat2());
                direction = math.normalize(new float3(lookAtPoint.x, 0f, lookAtPoint.y) - (float3)m_LookTransform.position);
                m_LookTransform.transform.rotation = Quaternion.LookRotation(direction, up);
            }
        }
    }
}