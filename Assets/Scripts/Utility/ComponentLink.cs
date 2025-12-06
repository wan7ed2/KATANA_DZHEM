using UnityEngine;

/// <summary>
/// Generic component linker. Place on child objects (e.g. with colliders)
/// to link to components on parent or other objects.
/// Use GetLinked<T>() to get the linked component.
/// </summary>
public class ComponentLink : MonoBehaviour
{
    [Tooltip("The target object containing the components to link to. If null, uses this GameObject.")]
    [SerializeField] private GameObject _target;
    
    /// <summary>
    /// The target GameObject this link points to.
    /// </summary>
    public GameObject Target => _target != null ? _target : gameObject;
    
    /// <summary>
    /// Get a component from the linked target.
    /// </summary>
    public T GetLinked<T>() where T : class
    {
        if (_target != null)
            return _target.GetComponent<T>();
        return null;
    }
    
    /// <summary>
    /// Try to get a component from the linked target.
    /// </summary>
    public bool TryGetLinked<T>(out T component) where T : class
    {
        component = GetLinked<T>();
        return component != null;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_target == null)
        {
            Debug.LogWarning($"[{nameof(ComponentLink)}] Target is not assigned on {gameObject.name}", this);
        }
    }
#endif
}

