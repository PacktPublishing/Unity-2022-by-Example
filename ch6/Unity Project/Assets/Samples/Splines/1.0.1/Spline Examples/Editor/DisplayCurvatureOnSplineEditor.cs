using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [CustomEditor(typeof(DisplayCurvatureOnSpline))]
    public class DisplayCurvatureOnSplineEditor : UnityEditor.Editor
    {
        void OnSceneGUI()
        {
            var displayScript = ( (DisplayCurvatureOnSpline)target );
            var curvatureTimes = displayScript.m_CurvatureTimes;
            var container = displayScript.container;
            
            if(container.Spline == null || Event.current.type != EventType.Repaint)
                return;

            using var nativeSpline = new NativeSpline(container.Spline, container.transform.localToWorldMatrix);

            foreach(var config in curvatureTimes)
            {
                if(!config.display)
                    continue;
                
                var t = math.clamp(config.time, 0f, 1f);
                
                if(container.transform.lossyScale != Vector3.one)
                {
                    //Convert t to be the same for the spline and the spline data in case
                    //a scale is applied on the SplineContainer GameObject
                    var curveIndex = container.Spline.SplineToCurveT(t, out float curveT);
                    t = nativeSpline.CurveToSplineT(curveIndex + curveT);
                }

                //Compute Spline Position in World Space
                var nativeSplinePos = nativeSpline.EvaluatePosition(t);

                //Compute Curvature at t, Curvature k = 1/radius
                var curvature = nativeSpline.EvaluateCurvature(t);
                
                //Compute the curvature center = i.e the center point of the tangent circle at the spline at t
                var curvatureCenter = nativeSpline.EvaluateCurvatureCenter(t);
                
                //Computing signed curvature :
                //Evaluate on which side of the spline is the curvature bending
                var up = nativeSpline.EvaluateUpVector(t);
                var velocity = nativeSpline.EvaluateTangent(t);
                var acceleration = nativeSpline.EvaluateAcceleration(t);
                var curvatureUp = math.normalize(math.cross(acceleration, velocity));
                
                var c = curvature * math.sign(math.dot(up, curvatureUp));
                c = math.clamp(5f* c, -1f, 1f);
                var curvatureColor = new Color(
                    c < 0 ? c + 1 : 1,
                    math.abs(c),
                    c < 0 ? 1 : 1 - c);

                using(new Handles.DrawingScope(curvatureColor))
                    Handles.DrawSolidDisc(curvatureCenter, curvatureUp, math.length(curvatureCenter - nativeSplinePos));

                using(new Handles.DrawingScope(Color.black))
                    Handles.SphereHandleCap(-1, nativeSplinePos, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(nativeSplinePos), EventType.Repaint);

            }
        }
    }
}
