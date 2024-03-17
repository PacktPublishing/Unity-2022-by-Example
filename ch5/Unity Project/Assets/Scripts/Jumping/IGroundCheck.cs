internal interface IGroundCheck
{
    bool IsGrounded { get; }

    void SetGrounded(bool grounded);
}