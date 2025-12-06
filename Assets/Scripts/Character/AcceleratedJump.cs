using System.Collections;
using UnityEngine;

public class AcceleratedJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private GroundChecker groundChecker;
    [Header("Settings")]
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpTime;
    [SerializeField] private AnimationCurve fallCurve;
    [Header("ObstacleFinder")]
    [SerializeField] private float radius;
    [SerializeField] private Vector2 offset;
    [SerializeField] private LayerMask layerMask;

    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    private readonly Collider2D[] _buffer = new Collider2D[1];

    private bool _isJumping;
    private float _fallTime;

    public void Jump(float modifier)
    {
        Debug.Log($"Jumping: {_isJumping}, Grounded: {groundChecker.IsGrounded}");
        if (_isJumping || !groundChecker.IsGrounded)
            return;

        StartCoroutine(JumpCoroutine(modifier));
    }

    private void FixedUpdate()
    {
        if (!_isJumping && !groundChecker.IsGrounded)
            ApplyGravity();
    }

    private IEnumerator JumpCoroutine(float modifier)
    {
        _isJumping = true;

        for (var t = 0f; t <= jumpTime; t += Time.fixedDeltaTime)
        {
            if (HasObstacleAbove())
                break;

            playerRigidBody.velocity += Vector2.up * jumpCurve.Evaluate(t) * modifier;
            yield return _waitForFixedUpdate;
        }

        _isJumping = false;
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
