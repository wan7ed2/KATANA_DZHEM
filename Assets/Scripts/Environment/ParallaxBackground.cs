using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0, 1)] private float strength;

    private Vector2 _lastPosition;
    private Vector2 _spriteSize;

    void Start()
    {
        _lastPosition = transform.position;
        _spriteSize = GetComponent<SpriteRenderer>().bounds.size;
    }

    void Update()
    {
        Vector2 reverseOffset = target.position * (1 - strength);
        Vector2 offset = target.position * strength;

        Vector2 newPosition = _lastPosition + offset;

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        if (reverseOffset.x > _lastPosition.x + _spriteSize.x)
            _lastPosition.x += _spriteSize.x;
        else if (reverseOffset.x < _lastPosition.x - _spriteSize.x)
            _lastPosition.x -= _spriteSize.x;

        if (reverseOffset.y > _lastPosition.y + _spriteSize.y)
            _lastPosition.y += _spriteSize.y;
        else if (reverseOffset.y < _lastPosition.y - _spriteSize.x)
            _lastPosition.y -= _spriteSize.y;
    }
}
