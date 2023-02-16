using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Interpolators = UnityEngine.Splines.Interpolators;

namespace Unity.Splines.Examples
{
    public class AnimateLookAt : MonoBehaviour
    {
        [SerializeField]
        SplineAnimate m_SplineAnimate;

        [SerializeField]
        PointSplineData m_LookAtPoints;

        [SerializeField]
        Transform m_LookTransform;

        void LateUpdate()
        {
            if (m_SplineAnimate == null || m_LookAtPoints == null || m_LookTransform == null)
                return;

            var spline = m_SplineAnimate.Container.Spline;
            var t = m_SplineAnimate.NormalizedTime;
            var lookAtPoint = m_LookAtPoints.Points.Evaluate(spline, t, PathIndexUnit.Normalized, new Interpolators.SlerpFloat2());
            var direction = math.normalize(new float3(lookAtPoint.x, 0f, lookAtPoint.y) - (float3)m_LookTransform.position);

            m_LookTransform.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
