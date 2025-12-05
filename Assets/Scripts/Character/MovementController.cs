using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController
{
    public MovementController(Rigidbody2D rigidbody, MovementsSettings settings, GroundChecker groundChecker)
    {
        _settings = settings;
        _rigidbody = rigidbody;
        _groundChecker = groundChecker;
        
        _input = new InputSystem_Actions();
        
        _walkRightAction = _input.Player.WalkRight;
        _walkLeftAction = _input.Player.WalkLeft;
        _jumpAction = _input.Player.Jump;
        _resetAction = _input.Player.Reset;
    }

    public void Start()
    {
        _input.Player.Enable();
        
        _jumpAction.performed += Jump;
        _resetAction.performed += Reset;
    }
    
    public void Stop()
    {
        _input.Player.Disable();
        
        _jumpAction.performed -= Jump;
        _resetAction.performed -= Jump;
    }

    public void Update()
    {
        if (_walkRightAction.IsPressed()) Move(RIGHT_DIRECTION);
        else if (_walkLeftAction.IsPressed()) Move(LEFT_DIRECTION);
    }

    private const float RIGHT_DIRECTION = 1f;
    private const float LEFT_DIRECTION = -1f;
    
    private InputAction _walkRightAction;
    private InputAction _walkLeftAction;
    private InputAction _jumpAction;
    private InputAction _resetAction;
    
    private GroundChecker _groundChecker;
    private MovementsSettings _settings;

    private InputSystem_Actions _input;

    private Rigidbody2D _rigidbody;

    private void Move(float direction)
    {
        var force = direction * (_settings.WALK_SPEED * Time.fixedDeltaTime);
        var xVelocity = _rigidbody.velocity.x + force;
        xVelocity = Mathf.Clamp(xVelocity, -_settings.MAX_WALK_SPEED, _settings.MAX_WALK_SPEED);
        _rigidbody.velocity = new Vector2(xVelocity, _rigidbody.velocity.y);
    }
    
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!_groundChecker.IsGrounded)
            return;
        
        var force = Vector2.up * _settings.JUMP_FORCE;
        _rigidbody.velocity += force;
    }

    private void ApplyGravity()
    {
        var gravity = Time.fixedDeltaTime * _settings.GRAVITY;
        _rigidbody.velocity += Vector2.down * gravity; 
    }

    private void Reset(InputAction.CallbackContext ctx)
    {
        _rigidbody.position = Vector2.one;
        _rigidbody.velocity = Vector2.zero;
    }
    
}