using Unity.Splines.Examples;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimateCarAlongSpline))]
public class AnimateCarAlongSplineEditor : Editor
{
    void OnEnable()
    {
        ((AnimateCarAlongSpline)target).Initialize();
    }
}
