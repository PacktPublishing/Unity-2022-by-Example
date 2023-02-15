# SplineData and Generation Sample

This is a more complex sample that makes heavy use of `SplineData`, `SplineUtility` and `SplineEditorUtility` APIs to proceduraly generate various spline-based objects in the scene.

Select `FenceSpline` or `EvenlySpawnSpline` game objects and manipulate the spline to see object generation along spline that leverages `SplineUtility.GetPointAtLinearDistance` function.

Select `Road Spline` or `Spline Follower` game objects to see `SplineDataHandles` in action and how they can be used to enable in-editor customization of road's width and follower's animation parameters. See `AnimateCarAlongSpline.cs` example script on how a spline follower can be implemented using `SplineUtility` and `SplineData` APIs.

Enter playmode to see the `AnimateCarAlongSpline.cs` script in action.