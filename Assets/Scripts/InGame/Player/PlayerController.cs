using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Horizontal")]
    [SerializeField] private float _xMaxSpeed = 50f;
    [SerializeField] private float _XGroundAcc = 50f;
    [SerializeField] private float _XAirAcc = 50f;
    [SerializeField] private float _XGroundDec = 50f;
    [SerializeField] private float _XAirDec = 50f;

    [Header("Vertical")]
    [SerializeField] private float _jumpSpeedBurst = 2f;
    [SerializeField] private float _jumpAcceleration = 5f;
    [SerializeField] private float _maxJumpDuration = 0.5f;
    [SerializeField] private float _fallGravityModifier = 1.8f;


    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _collider;
    private bool _isDead;
    private bool _isJumping;
    private float _mass;

    private float _jumpTimer;
    private bool _isGrounded;


    protected override void Awake()
    {
        base.Awake();

        //Component references
        //_animator = GetComponent<PlayerAnimator>();
        _collider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _mass = _rb.mass;
    }

    private void Start()
    {
        /*GameInput.Actions.Platforming.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        GameInput.Actions.Platforming.Move.canceled += ctx => _moveInput = Vector2.zero;*/
        GameInput.Actions.Platforming.Jump.performed += TryJump;
        GameInput.Actions.Platforming.Jump.canceled += EndJump;
    }

    private void OnDisable()
    {
        /*GameInput.Actions.Platforming.Move.performed -= ctx => _moveInput = ctx.ReadValue<Vector2>();
        GameInput.Actions.Platforming.Move.canceled -= ctx => _moveInput = Vector2.zero;*/
        GameInput.Actions.Platforming.Jump.performed -= TryJump;
        GameInput.Actions.Platforming.Jump.canceled -= EndJump;
    }


    private void FixedUpdate()
    {
        if (InGameManager.Instance.IsPaused) return;
        CheckIfGrounded();
        SetGravity();
        if (_isDead || InGameManager.Instance.Mode == GameplayType.Construction) {
            _moveInput = Vector2.zero;
        }
        else
        {
            _moveInput = GameInput.Actions.Platforming.Move.ReadValue<Vector2>();
        }
        float movement = _moveInput.x;

        // Apply force
        float acceleration = _isGrounded ? _XGroundAcc : _XAirAcc;
        float deceleration = _isGrounded ? _XGroundDec : _XAirDec;
        float change = _moveInput.x  * _rb.linearVelocity.x > 0 ? acceleration : deceleration;
        float groundChange = _isGrounded ? _XGroundDec * 0.6f :  _XAirAcc * 0.5f;

        if (_moveInput == Vector2.zero && Math.Abs(_rb.linearVelocity.x) > Mathf.Epsilon)
        {
            _rb.AddForce(-Math.Sign(_rb.linearVelocity.x) * Math.Min(Math.Abs(_rb.linearVelocity.x), 1f) *
                Vector2.right * groundChange * _mass, ForceMode2D.Force);
        }
        else if (_moveInput == Vector2.zero) { }
        else if (Math.Abs(_rb.linearVelocity.x) > _xMaxSpeed && _moveInput.x * _rb.linearVelocity.x > 0) { }
        else
        {
            _rb.AddForce(movement * Vector2.right * change * _mass, ForceMode2D.Force);
        }

        if (_isJumping)
        {
            //_rb.linearVelocityY += _jumpAcceleration * Time.fixedDeltaTime;
            _rb.AddForce(Vector2.up * _jumpAcceleration * _mass, ForceMode2D.Force);
            _jumpTimer -= Time.fixedDeltaTime;
            if (_jumpTimer <= 0) _isJumping = false;
        }
    }

    private void TryJump(InputAction.CallbackContext context)
    {
        if (InGameManager.Instance.IsPaused || InGameManager.Instance.Mode == GameplayType.Construction) return;
        if (_isGrounded && !_isJumping)
        {
            _rb.AddForce(Vector2.up * _jumpSpeedBurst * _mass, ForceMode2D.Impulse);
            _isJumping = true;
            _jumpTimer = _maxJumpDuration;
        }
    }

    private void EndJump(InputAction.CallbackContext context)
    {
        _isJumping = false;
    }

    private void CheckIfGrounded()
    {
        _isGrounded = _collider.IsTouchingLayers(LayerMask.GetMask("Floor"));
        if (_isGrounded && _jumpTimer < _maxJumpDuration * 0.5f) {_isJumping = false;}
    }

    private void SetGravity()
    {
        if (_isJumping) _rb.gravityScale = 0f;
        else if (_rb.linearVelocityY < 0f) _rb.gravityScale = _fallGravityModifier;
        else _rb.gravityScale = 1f;
    }
}
