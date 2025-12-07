using UnityEngine;

public class JumpZone : MonoBehaviour
{
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float jumpTime { get; private set; }

    [field: SerializeField] public ParticleSystem particleEffects { get; private set; }
}
