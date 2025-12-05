using UnityEngine;

public interface IPushableByObstacle
{
    public void Push(Vector2 direction, float force);
}