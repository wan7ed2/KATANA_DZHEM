using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : IPushableByObstacle
{
    public MovementController(
        Rigidbody2D rigidbody, 
        MovementsSettings settings, 
        GroundChecker groundChecker, 
        InputSystem_Actions.PlayerActions input,
        AcceleratedJump acceleratedJump,
        CharacterStatusEffectHandler statusHandler = null)
    {
        _settings = settings;
        _rigidbody = rigidbody;
        _groundChecker = groundChecker;
        _statusHandler = statusHandler;
        
        _walkRightAction = input.WalkRight;
        _walkLeftAction = input.WalkLeft;
        _jumpAction = input.Jump;
        _resetAction = input.Reset;
        _acceleratedJump = acceleratedJump;
    }

    public void Start()
    {
        _jumpAction.performed += Jump;
        _resetAction.performed += Reset;
    }
    
    public void Stop()
    {
        _jumpAction.performed -= Jump;
        _resetAction.performed -= Reset;
    }

    public void Tick()
    {
        if (_walkRightAction.IsPressed()) Move(RIGHT_DIRECTION);
        else if (_walkLeftAction.IsPressed()) Move(LEFT_DIRECTION);
        else ApplyFriction();
    }
    
    public void Push(Vector2 direction, float force)
    {
        _rigidbody.velocity += direction * force;
    }

    private const float RIGHT_DIRECTION = 1f;
    private const float LEFT_DIRECTION = -1f;
    
    private InputAction _walkRightAction;
    private InputAction _walkLeftAction;
    private InputAction _jumpAction;
    private InputAction _resetAction;
    
    private GroundChecker _groundChecker;
    private MovementsSettings _settings;
    private CharacterStatusEffectHandler _statusHandler;

    private InputSystem_Actions.PlayerActions _input;

    private Rigidbody2D _rigidbody;
    private AcceleratedJump _acceleratedJump;

    private StatusEffectModifiers GetModifiers()
    {
        return _statusHandler != null 
            ? _statusHandler.CurrentModifiers 
            : StatusEffectModifiers.Default;
    }

    private void Move(float direction)
    {
        var mods = GetModifiers();
        
        var force = direction * (_settings.WALK_FORCE * mods.AccelerationMultiplier * Time.fixedDeltaTime);
        if (!_groundChecker.IsGrounded)
            force *= _settings.AIR_MULTIPLIER;
        
        var xVelocity = _rigidbody.velocity.x + force;
        
        var maxSpeed = _settings.MAX_WALK_SPEED * mods.SpeedMultiplier;
        xVelocity = Mathf.Clamp(xVelocity, -maxSpeed, maxSpeed);
        
        _rigidbody.velocity = new Vector2(xVelocity, _rigidbody.velocity.y);
    }
    
    private void ApplyFriction()
    {
        var mods = GetModifiers();
        
        // Friction: 1 = быстро тормозим, 0 = скользим (лёд)
        float minDecel = 0.99f;  // Почти нет торможения (лёд)
        float maxDecel = 0.85f;  // Сильное торможение
        
        var deceleration = Mathf.Lerp(minDecel, maxDecel, mods.Friction);
        
        _rigidbody.velocity = new Vector2(
            _rigidbody.velocity.x * deceleration, 
            _rigidbody.velocity.y
        );
    }
    
    private void Jump(InputAction.CallbackContext ctx)
    {
        _acceleratedJump.Jump(GetModifiers().JumpMultiplier);
    }

    private void Reset(InputAction.CallbackContext ctx)
    {
        _rigidbody.position = Vector2.one;
        _rigidbody.velocity = Vector2.zero;
    }
}
