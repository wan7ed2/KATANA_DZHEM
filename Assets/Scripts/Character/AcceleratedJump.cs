using System.Collections;
using UnityEngine;

public class AcceleratedJump : MonoBehaviour
{
    [SerializeField] private MovementAnimator movementAnimator;
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private GroundChecker groundChecker;
    [Header("Settings")]
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private AnimationCurve fallCurve;
    [Header("ObstacleFinder")]
    [SerializeField] private float radius;
    [SerializeField] private Vector2 offset;
    [SerializeField] private LayerMask layerMask;

    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    private readonly Collider2D[] _buffer = new Collider2D[1];

    private bool _isJumping;
    private float _fallTime;

    public void Jump(float modifier) => JumpInternal(modifier, jumpForce, jumpTime);

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<JumpZone>(out var zone))
            return;

        JumpInternal(1, zone.jumpForce, zone.jumpTime);
    }

    public void Tick()
    {
        if (!_isJumping && !groundChecker.IsGrounded)
            ApplyGravity();
    }

    private void JumpInternal(float modifier, float force, float time)
    {
        if (_isJumping || !groundChecker.IsGrounded)
            return;

        StartCoroutine(JumpCoroutine(modifier, force, time));
    }

    private IEnumerator JumpCoroutine(float modifier, float force, float time)
    {
        _isJumping = true;
        movementAnimator.StartJump();

        for (var t = 0f; t <= time; t += Time.fixedDeltaTime)
        {
            if (HasObstacleAbove())
                break;

            playerRigidBody.velocity += Vector2.up * (jumpCurve.Evaluate(Mathf.InverseLerp(0f, time, t)) * modifier * force);
            yield return _waitForFixedUpdate;
        }

        _isJumping = false;
        movementAnimator.StopJump();
    }

    private void ApplyGravity()
    {
        playerRigidBody.velocity += Vector2.down * fallCurve.Evaluate(_fallTime);
        _fallTime += Time.fixedDeltaTime;
    }

    private bool HasObstacleAbove()
    {
        return Physics2D.OverlapCircleNonAlloc(playerRigidBody.transform.position + (Vector3)offset, radius, _buffer, layerMask) > 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerRigidBody.transform.position + (Vector3)offset, radius);
    }
#endif

}
