using UnityEngine;
using System.Collections.Generic;

public class ParallaxLayers : MonoBehaviour
{
    public List<Layer> Layers;

    [System.Serializable]
    public class Layer      // Nested class.
    {
        public Renderer Image;
        [Tooltip("How far away is the image from the camera?"), Range(0, 10000)]
        public int ZDepth;
    }

    private Camera _camera;
    private Vector3 _cameraLastScreenPosition;


    private void Awake()
    {
        _camera = Camera.main;
        _cameraLastScreenPosition = _camera.transform.position;
    }

    // Switched to FixedUpdate() to eliminate jitter when Cinemachine was introduced as the camera controller for following the player.
    //private void LateUpdate()
    private void FixedUpdate()
    {
        if (_camera.transform.position.x == _cameraLastScreenPosition.x)
            return;

        foreach (var item in Layers)
        {
            float parallaxSpeed = 1 - Mathf.Clamp01(Mathf.Abs(_camera.transform.position.z / item.ZDepth));
            float difference = _camera.transform.position.x - _cameraLastScreenPosition.x;
            item.Image.transform.Translate(difference * parallaxSpeed * Vector3.right);
        }

        _cameraLastScreenPosition = _camera.transform.position;
    }
}
