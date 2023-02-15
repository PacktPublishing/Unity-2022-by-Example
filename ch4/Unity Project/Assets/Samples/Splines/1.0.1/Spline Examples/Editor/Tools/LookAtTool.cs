using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEditor.EditorTools;

namespace Unity.Splines.Examples
{
    [EditorTool("LookAtPoint Tool", typeof(PointSplineData))]
    public class LookAtTool : SplineDataToolBase<float2>, IDrawSelectedHandles
    {   
        GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        bool m_DisableHandles;
        
        void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("Icons/LookAtTool"),
                text = "LookAt Tool",
                tooltip = "Adjust the LookAt DataPoint along the spline."
            };
        }
        
        public override void OnToolGUI(EditorWindow window)
        {
            var splineDataTarget = target as PointSplineData;
            if(splineDataTarget == null || splineDataTarget.container == null)
                return;

            var nativeSpline = new NativeSpline(splineDataTarget.container.Spline, splineDataTarget.container.transform.localToWorldMatrix);

            Handles.color = Color.yellow;
            m_DisableHandles = false;
            
            Undo.RecordObject(splineDataTarget,"Modifying LookAt SplineData");
            
            //User defined : Handles to manipulate LookAtPoint data
            DrawDataPoints(nativeSpline, splineDataTarget.points);

            //Using the out-of the box behaviour to manipulate SplineData indexes
            nativeSpline.DataPointHandles(splineDataTarget.points);
        }
        
        public void OnDrawHandles()
        {
            var splineDataTarget = target as PointSplineData;
            if(ToolManager.IsActiveTool(this) || splineDataTarget.container == null)
                return;
            
            if(Event.current.type != EventType.Repaint)
                return;
                 
            var nativeSpline = new NativeSpline(splineDataTarget.container.Spline, splineDataTarget.container.transform.localToWorldMatrix);

            Color color = Color.yellow;
            color.a = 0.5f;
            Handles.color = color;
            
            m_DisableHandles = true;
            DrawDataPoints(nativeSpline,splineDataTarget.points);
        }
        
        protected override bool DrawDataPoint(
            Vector3 position, 
            Vector3 tangent,
            Vector3 up, 
            float2 inValue,
            out float2 outValue)
        {
            var controlID = m_DisableHandles ? -1 : GUIUtility.GetControlID(FocusType.Passive);
            outValue = float2.zero;
            var handleColor = Handles.color;
            if(GUIUtility.hotControl == 0 
                && HandleUtility.nearestControl!= -1 
                && HandleUtility.nearestControl == controlID)
                handleColor = Handles.preselectionColor;
            
            var pointValue = new float3(inValue.x, 0f, inValue.y);
            
            var size = k_HandleSize * HandleUtility.GetHandleSize(pointValue);

            using (new Handles.DrawingScope(handleColor))
            {
                EditorGUI.BeginChangeCheck();
                Handles.DrawLine(position, pointValue);
                
                var newPointValue = (float3)Handles.Slider2D(controlID, pointValue, -Vector3.up, Vector3.right, Vector3.forward, size, Handles.ConeHandleCap, Vector2.zero, true); 
                if (EditorGUI.EndChangeCheck())
                {
                    var delta = newPointValue - pointValue;
                    outValue = inValue + new float2(delta.x, delta.z);
                    return true;
                }
            }
            return false;
        }
    }
}
