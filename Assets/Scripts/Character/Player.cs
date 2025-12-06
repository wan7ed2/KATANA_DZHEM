using Character;
using UnityEngine;

public class Player : MonoBehaviour, IPushableByObstacle, IWindAffected
{
    [SerializeField] private MovementsSettings _movementsSettings;
    [SerializeField] private StickSettings _stickSettings;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private Rigidbody2D _stickRigidbody;
    [SerializeField] private CharacterStatusEffectHandler _statusHandler;
    [SerializeField] private AcceleratedJump _acceleratedJump;

    public Rigidbody2D Rigidbody => _rigidbody;
    public MovementController MovementController => _movementController;
    
    private MovementController _movementController;
    private StickController _stickController;
    private InputSystem_Actions _input;
    
    private void Awake()
    {
        _input = new InputSystem_Actions();
        _groundChecker.Init(_rigidbody);
        _stickController = new StickController(_input.Player, _stickRigidbody, _stickSettings);
        _movementController = new MovementController(
            _rigidbody, 
            _movementsSettings, 
            _groundChecker, 
            _input.Player,
            _acceleratedJump,
            _statusHandler
        );
    }

    private void Start()
    {
        _movementController.Start();
        _stickController.Start();
        _input.Enable();
    }

    private void FixedUpdate()
    {
        _groundChecker.Update();
        _movementController.Update();
        _stickController.Update();
    }

    private void OnDestroy()
    {
        _input.Disable();
        _movementController.Stop();
        _stickController.Stop();
    }

    public void Push(Vector2 direction, float force)
    {
        _rigidbody.AddForce(direction);
    }
    
    public void ApplyWind(Vector2 force)
    {
        _rigidbody.AddForce(force);
    }
}
