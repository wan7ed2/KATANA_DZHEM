using UnityEngine;

public interface IPushableByObstacle
{
    public void Push(Vector2 direction, float force);
}

public class Character : MonoBehaviour, IPushableByObstacle
{
    public void Push(Vector2 direction, float force)
    {
        // If you need it do it yourself
    }
}