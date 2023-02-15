using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [EditorTool("Width Tool", typeof(WidthSplineData))]
    public class WidthTool : SplineDataToolBase<float>, IDrawSelectedHandles
    {
        GUIContent m_IconContent;
        public override GUIContent toolbarIcon => m_IconContent;

        bool m_DisableHandles = false;
        
        void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("Icons/WidthTool"),
                text = "Width Tool",
                tooltip = "Adjust the width of the created road mesh."
            };
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var splineDataTarget = target as WidthSplineData;
            if(splineDataTarget == null || splineDataTarget.container == null)
                return;
            
            var nativeSpline = new NativeSpline(splineDataTarget.container.Spline, splineDataTarget.container.transform.localToWorldMatrix);

            Undo.RecordObject(splineDataTarget,"Modifying Width SplineData");
            
            Handles.color = Color.blue;
            m_DisableHandles = false;
            
            //User defined handles to manipulate width
            DrawDataPoints(nativeSpline, splineDataTarget.width);

            //Using the out-of the box behaviour to manipulate indexes
            nativeSpline.DataPointHandles(splineDataTarget.width);
        }
        
        public void OnDrawHandles()
        {
            var splineDataTarget = target as WidthSplineData;
            if(ToolManager.IsActiveTool(this) || splineDataTarget.container == null)
                return;
            
            var nativeSpline = new NativeSpline(splineDataTarget.container.Spline, splineDataTarget.container.transform.localToWorldMatrix);

            Color color = Color.blue;
            color.a = 0.5f;
            Handles.color = color;
            m_DisableHandles = true;
            DrawDataPoints(nativeSpline,splineDataTarget.width);
        }
        
        protected override bool DrawDataPoint(
            Vector3 position, 
            Vector3 tangent,
            Vector3 up,
            float inValue,
            out float outValue)
        {
            int id1 = m_DisableHandles ? -1 : GUIUtility.GetControlID(FocusType.Passive);
            int id2 = m_DisableHandles ? -1 : GUIUtility.GetControlID(FocusType.Passive);
        
            outValue = 0f;
            if(tangent == Vector3.zero)
                return false;
        
            if(Event.current.type == EventType.MouseUp
                && Event.current.button != 0
                && ( GUIUtility.hotControl == id1 || GUIUtility.hotControl == id2 ))
            {
                Event.current.Use();
                return false;
            }
                
            var handleColor = Handles.color;
            if(GUIUtility.hotControl == id1 || GUIUtility.hotControl == id2)
                handleColor = Handles.selectedColor;
            else if(GUIUtility.hotControl == 0 && (HandleUtility.nearestControl==id1 || HandleUtility.nearestControl==id2))
                handleColor = Handles.preselectionColor;
        
            var normalDirection = math.normalize(math.cross(tangent, up));
        
            var extremity1 = position - inValue * (Vector3)normalDirection;
            var extremity2 = position + inValue * (Vector3)normalDirection;
            Vector3 val1, val2;
            using(new Handles.DrawingScope(handleColor))
            {
                Handles.DrawLine(extremity1, extremity2);
                val1 = Handles.Slider(id1, extremity1, normalDirection,
                    k_HandleSize * .5f * HandleUtility.GetHandleSize(position), CustomHandleCap, 0);
                val2 = Handles.Slider(id2, extremity2, normalDirection,
                    k_HandleSize * .5f * HandleUtility.GetHandleSize(position), CustomHandleCap, 0);
            }
        
            if(GUIUtility.hotControl == id1 && math.abs(( val1 - extremity1 ).magnitude) > 0)
            {
                outValue = math.abs(( val1 - position ).magnitude);
                return true;
            }
        
            if(GUIUtility.hotControl == id2 && math.abs(( val2 - extremity2 ).magnitude) > 0)
            {
                outValue = math.abs(( val2 - position ).magnitude);
                return true;
            }
            
            return false;
        }

        public void CustomHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Handles.CubeHandleCap(controlID, position, rotation, size, m_DisableHandles ? EventType.Repaint : eventType);
        }
    }
}
