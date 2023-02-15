using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Splines.Examples.Editor
{
    [CustomEditor(typeof(LoftRoadBehaviour))]
    class RoadTool : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
        
            base.OnInspectorGUI();
        
            if (EditorGUI.EndChangeCheck())
                ((LoftRoadBehaviour)target).Loft();
        }
    }
}
