#if UNITY_EDITOR
using UnityEngine;

public class LevelBoundsGizmo : MonoBehaviour
{
    public Vector2 BottomLeftPosition;
    public Vector2 TopRightPosition;
    public Color BoundsColor;
    public Color CenterColor;

    private void OnDrawGizmos()
    {
        Gizmos.color = BoundsColor;
        Gizmos.DrawWireCube(BottomLeftPosition, Vector3.one);
        Gizmos.DrawWireCube(TopRightPosition, Vector3.one);
        Gizmos.DrawLine(BottomLeftPosition, new Vector2(BottomLeftPosition.x, TopRightPosition.y));
        Gizmos.DrawLine(BottomLeftPosition, new Vector2(TopRightPosition.x, BottomLeftPosition.y));
        Gizmos.DrawLine(TopRightPosition, new Vector2(BottomLeftPosition.x, TopRightPosition.y));
        Gizmos.DrawLine(TopRightPosition, new Vector2(TopRightPosition.x, BottomLeftPosition.y));

        Gizmos.color = CenterColor;
        Gizmos.DrawWireSphere(BottomLeftPosition + ((TopRightPosition - BottomLeftPosition) / 2), 0.5F);
    }
}
#endif