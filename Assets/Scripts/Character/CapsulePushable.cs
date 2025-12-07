using UnityEngine;

namespace Character
{
    public class CapsulePushable : MonoBehaviour, IPushableByObstacle
    {
        [SerializeField] private Player _player;

        public void Push(Vector2 direction, float force)
        {
            _player.Push(direction, force);
        }
    }
}