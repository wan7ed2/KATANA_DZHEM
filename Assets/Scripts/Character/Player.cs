using System;
using Character;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour, IPushableByObstacle, IWindAffected
{
    public event Action OnHit;
    
    [SerializeField] private bool _isEvil;
    
    [SerializeField] private MovementsSettings _movementsSettings;
    [SerializeField] private StickSettings _stickSettings;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private Rigidbody2D _stickRigidbody;
    [SerializeField] private CharacterStatusEffectHandler _statusHandler;
    [SerializeField] private AcceleratedJump _acceleratedJump;
    [SerializeField] private MovementAnimator _movementAnimator;
    
    [SerializeField] private MovementController _movementController;
    [SerializeField] private CharacterSoundView _soundView;
    [SerializeField] private ParticleSystem _jumpParticles;

    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorController _goodAnim;
    [SerializeField] private AnimatorController _evilAnim;

    public const string IS_EVIL_KEY = "IsEvil";
    
    public Rigidbody2D Rigidbody => _rigidbody;
    public MovementController MovementController => _movementController;
    public GroundChecker GroundChecker => _groundChecker;
    
    private StickController _stickController;
    private InputSystem_Actions _input;

    public void SetEvil(bool isEvil)
    {
        _isEvil = isEvil;
    }
    
    private void Awake()
    {
        _animator.runtimeAnimatorController = _isEvil ? _evilAnim : _goodAnim;
        _soundView.Init(this, _groundChecker);
        
        _input = new InputSystem_Actions();
        _groundChecker.Init(_rigidbody);
        _stickController = new StickController(_input.Player, _stickRigidbody, _stickSettings);
        _movementController.Init(
            _rigidbody,
            _movementsSettings,
            _groundChecker,
            _input.Player,
            _acceleratedJump,
            _movementAnimator,
            _statusHandler,
            _jumpParticles
        );
    }

    private void Start()
    {
        _soundView.Start();
        _movementController.Start();
        _stickController.Start();
        _input.Enable();
    }

    private void FixedUpdate()
    {
        _groundChecker.Tick();
        _movementController.Tick();
        _stickController.Tick();
        _acceleratedJump.Tick();
    }

    private void OnDestroy()
    {
        _soundView.Stop();
        _input.Disable();
        _movementController.Stop();
        _stickController.Stop();
    }

    public void Push(Vector2 direction, float force)
    {
        _movementController.Push(direction, force);
        OnHit?.Invoke();
    }
    
    public void ApplyWind(Vector2 force)
    {
        _rigidbody.AddForce(force);
    }
}
