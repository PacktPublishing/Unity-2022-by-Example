using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool IsLayerInMask(this LayerMask layerMask, int layer)
        => (layerMask & (1 << layer)) != 0;
}