using UnityEngine;

public class GroundCheckTrigger : MonoBehaviour, IGroundCheck
{
    public bool IsGrounded => _isGrounded;
    private bool _isGrounded;

    [SerializeField] private LayerMask _groundMask;


    public void SetGrounded(bool grounded) => _isGrounded = grounded;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_groundMask.IsLayerInMask(collision.gameObject.layer))
        {
            _isGrounded = true;
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (IsLayerInLayerMask(collision.gameObject.layer, _groundMask))
    //    {
    //        _isGrounded = false;
    //    }
    //}
}