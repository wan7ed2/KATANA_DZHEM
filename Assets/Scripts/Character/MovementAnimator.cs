using UnityEngine;

public class MovementAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private const float RightDirection = 1;

    private readonly string MovementAnimationParameter = "isMoving";
    private readonly string JumpAnimationParameter = "isJumping";

    public void StartMovement(float direction)
    {
        spriteRenderer.flipX = direction == RightDirection;
        animator.SetBool(MovementAnimationParameter, true);
    }

    public void StopMovement()
    {
        animator.SetBool(MovementAnimationParameter, false);
    }

    public void StartJump()
    {
        animator.SetBool(JumpAnimationParameter, true);
    }

    public void StopJump()
    {
        animator.SetBool(JumpAnimationParameter, false);
    }

}