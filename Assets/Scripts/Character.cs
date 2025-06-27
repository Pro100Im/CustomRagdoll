using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    [Space]
    [SerializeField] private string _speed = "Speed";
    [SerializeField] private string _jumpAnimTrigger = "Jump";
    [Space]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _groundCheck;
    [Space]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;
    [Space]
    [SerializeField] private LayerMask _groundLayer;

    private const float _magnitudeThreshold = 0.001f;

    private bool _jumpPressed;
    private bool _isGrounded;

    private Vector2 _moveInput;

    private void Awake()
    {
        _moveAction.action.performed += SetMoveInput;
        _moveAction.action.canceled += SetMoveInput;
        _jumpAction.action.performed += SetJumpInput;

        _moveAction.action.Enable();
        _jumpAction.action.Enable();
    }

    private void SetMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
    }

    private void SetJumpInput(InputAction.CallbackContext context)
    {
        if(_jumpPressed)
            return;

        _jumpPressed = true;
    }

    public void SetCharacterEnable(bool value)
    {
        //_rb.linearVelocity = Vector3.zero;
        //_rb.detectCollisions = value;
        _animator.enabled = value;
    }

    private void FixedUpdate()
    {
        if(_rb.isKinematic) 
            return;

        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer);

        Move();
        Jump();

        _jumpPressed = false;
    }

    private void Move()
    {
        var velocity = new Vector3(_moveInput.x * _moveSpeed, _rb.linearVelocity.y, _moveInput.y * _moveSpeed);
        var moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);

        _rb.linearVelocity = velocity;

        if(moveDirection.sqrMagnitude > _magnitudeThreshold)
        {
            var targetRotation = Quaternion.LookRotation(moveDirection);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));
        }

        _animator.SetFloat(_speed, _moveInput.magnitude);
    }

    private void Jump()
    {
        if(_jumpPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Acceleration);
            _animator.SetTrigger(_jumpAnimTrigger);
        }    
        else if(!_isGrounded)
            _rb.AddForce(Vector3.down * GameConstants.PlayerGravity, ForceMode.Acceleration);
    }

    private void OnDestroy()
    {
        _moveAction.action.performed -= SetMoveInput;
        _moveAction.action.canceled -= SetMoveInput;
        _jumpAction.action.performed -= SetJumpInput;

        _moveAction.action.Disable();
        _jumpAction.action.Disable();
    }
}
