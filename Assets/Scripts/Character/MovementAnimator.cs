using UnityEngine;

public class MovementAnimator : MonoBehaviour
{
    [SerializeField] private AnimatorComponentsProvider components;

    private const float RightDirection = 1;

    private readonly string MovementAnimationParameter = "isMoving";
    private readonly string JumpAnimationParameter = "isJumping";

    public void StartMovement(float direction)
    {
        components.SpriteRenderer.flipX = direction == RightDirection;
        components.Animator.SetBool(MovementAnimationParameter, true);
    }

    public void StopMovement()
    {
        components.Animator.SetBool(MovementAnimationParameter, false);
    }

    public void StartJump()
    {
        components.Animator.SetBool(JumpAnimationParameter, true);
    }

    public void StopJump()
    {
        components.Animator.SetBool(JumpAnimationParameter, false);
    }

}