# Loft Road Sample

This sample demonstrates how to write a road mesh loft tool using `SplineUtility` and `SplineEditorUtility` APIs. See `LoftRoadBehaviour.cs` script as example of how `SplineUtility`'s `EvaluatePosition`, `EvaluateTangent` and `EvaluateUpVector` functions can be used to extract information about a Spline and how `EditorSplineUtility`'s callbacks can be leveraged for tooling. In addition, the width of the loft is driven by use of `SplineData` which is a good example of how custom data can be associated with Splines.

Manipulate `Road Spline` game object's Spline to see the road regenerate.
