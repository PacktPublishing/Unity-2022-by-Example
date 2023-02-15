# GPU Evaluation Sample

This sample demonstrates how Splines can be evaluated on the GPU. Splines package comes with a `Spline.cginc` shader include file which can be used to write shaders that can work with Splines. See `SplineRendererCompute.cs` script for an example on how Spline information can be passed to the GPU using `SplineComputeBufferScope`. See `InterpolateSpline.compute` shader on how Spline position can be evaluated using Spline.cginc in a compute shader.

Play the sample scene to see GPU Spline evaluation in action.
