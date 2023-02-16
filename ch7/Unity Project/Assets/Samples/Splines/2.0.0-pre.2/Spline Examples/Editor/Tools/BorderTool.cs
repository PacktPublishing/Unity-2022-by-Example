using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

using Interpolators = UnityEngine.Splines.Interpolators;

namespace Unity.Splines.Examples
{
    [EditorTool("Border Tool", typeof(SpawnWithinSplineBounds))]
    public class BorderTool : SplineDataToolBase<float>, IDrawSelectedHandles
    {
        const float k_HandleOffset = 2f;
        const float k_LineLengthsSize = 4f;
        const int k_SamplesPerCurve = 15;

        List<Vector3> m_LineSegments = new List<Vector3>();

        GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("Icons/BorderTool"),
                text = "Border Tool",
                tooltip = "Define the border limit of the area defined by the spline."
            };
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var splineDataTarget = target as SpawnWithinSplineBounds;
            var nativeSpline = new NativeSpline(splineDataTarget.Container.Spline, splineDataTarget.Container.transform.localToWorldMatrix);

            Undo.RecordObject(splineDataTarget, "Modifying Border SplineData");

            Handles.color = Color.yellow;
            //User defined : Handles to manipulate Border data
            DrawDataPoints(nativeSpline, splineDataTarget.SpawnBorderData);
            //Use defined : Draws a line along the whole Border SplineData
            DrawSplineData(nativeSpline, splineDataTarget.SpawnBorderData);

            //Using the out-of the box behaviour to manipulate SplineData indexes
            nativeSpline.DataPointHandles(splineDataTarget.SpawnBorderData);
        }

        public void OnDrawHandles()
        {
            var splineDataTarget = target as SpawnWithinSplineBounds;
            if (ToolManager.IsActiveTool(this) || splineDataTarget.Container == null)
                return;

            //Reduce number of time we have to convert spline To NativeSpline
            if (Event.current.type != EventType.Repaint)
                return;

            var nativeSpline = new NativeSpline(splineDataTarget.Container.Spline, splineDataTarget.Container.transform.localToWorldMatrix);
            Color color = Color.yellow;
            color.a = 0.5f;
            Handles.color = color;
            DrawSplineData(nativeSpline, splineDataTarget.SpawnBorderData);
        }

        protected override bool DrawDataPoint(
            Vector3 position,
            Vector3 tangent,
            Vector3 up,
            float inValue,
            out float outValue)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            outValue = inValue;
            var handleColor = Handles.color;
            if (GUIUtility.hotControl == 0 && HandleUtility.nearestControl == controlID)
                handleColor = Handles.preselectionColor;

            var right = Vector3.Cross(tangent.normalized, up.normalized);
            var borderPos = position + right * inValue;

            var size = k_HandleSize * HandleUtility.GetHandleSize(borderPos);
            var sliderOffset = GetBorderHandleOffset(borderPos);

            var handleMatrix = Handles.matrix * Matrix4x4.TRS(sliderOffset, Quaternion.identity, Vector3.one);
            var inUse = false;
            using (new Handles.DrawingScope(handleColor, handleMatrix))
            {
                EditorGUI.BeginChangeCheck();
                var newBorderPos = Handles.Slider(controlID, borderPos, right, size, Handles.RectangleHandleCap, 0f);
                if (EditorGUI.EndChangeCheck())
                {
                    var delta = newBorderPos - borderPos;

                    outValue = Mathf.Max(0f, inValue + (Vector3.Dot(right, delta) > 0 ? delta.magnitude : -delta.magnitude));
                    inUse = true;
                }
            }
            return inUse;
        }

        void DrawSplineData(
            ISpline spline,
            SplineData<float> splineData)
        {
            m_LineSegments.Clear();

            var curveCount = spline.Closed ? spline.Count : spline.Count - 1;
            var stepSize = 1f / k_SamplesPerCurve;
            var prevBorderPos = Vector3.zero;

            for (int curveIndex = 0; curveIndex < curveCount; ++curveIndex)
            {
                for (int step = 0; step < k_SamplesPerCurve; ++step)
                {
                    var splineTime = spline.CurveToSplineT(curveIndex + step * stepSize);
                    spline.Evaluate(splineTime, out var position, out var tangent, out var upVector);
                    var right = math.cross(math.normalize(tangent), upVector);
                    var border = splineData.Evaluate(spline, splineTime, PathIndexUnit.Normalized, new Interpolators.LerpFloat());
                    var borderPos = position + right * border;
                    borderPos += (float3)GetBorderHandleOffset(borderPos);
                    if (curveIndex > 0 || step > 0)
                    {
                        m_LineSegments.Add(prevBorderPos);
                        m_LineSegments.Add(borderPos);
                    }

                    prevBorderPos = borderPos;
                }
            }

            Handles.DrawDottedLines(m_LineSegments.ToArray(), k_LineLengthsSize);
        }

        Vector3 GetBorderHandleOffset(Vector3 borderPos)
        {
            var size = k_HandleSize * HandleUtility.GetHandleSize(borderPos);
            return Vector3.up * size * k_HandleOffset;
        }
    }
}
