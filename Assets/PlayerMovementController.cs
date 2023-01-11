using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovementController : NetworkBehaviour
{
    //reference variables
    PlayerInput _playerInput;
    CharacterController _charController;
    Animator _animator;

    //variables to store player input values
    Vector2 _currentMoveInput;
    Vector3 _currentMove;
    Vector3 _currentRunMove;
    bool _isMovingPressed;
    bool _isSprintingPressed;
    bool _isJumpPressed = false;

    //editable variables in the editor for playtesting
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float rotationFactor;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float maxJumpTime;
    [SerializeField] float fallMultiplier;

    //variables to handle animation
    int _isWalkingHash;
    int _isSprintingHash;
    int _isJumpingHash;

    //variables to handle jumping
    float _initialJumpVelocity;
    bool _isJumping= false;
    float _gravity;

    private void Awake()
    {
        //assign classes to the variables
        _playerInput = new PlayerInput();
        _charController = GetComponent<CharacterController>();

        //setup animation
        _animator = GetComponentInChildren<Animator>();
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isSprintingHash = Animator.StringToHash("isSprinting");
        _isJumpingHash = Animator.StringToHash("isJumping");

        //setup player input
        _playerInput.CharacterControls.Move.started += OnMoveInput;
        _playerInput.CharacterControls.Move.canceled += OnMoveInput;
        _playerInput.CharacterControls.Move.performed += OnMoveInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;

        //setup jump 
        SetupJump();
    }

    //callback functions fo input system
    void OnMoveInput(InputAction.CallbackContext context)
    {
        _currentMoveInput = context.ReadValue<Vector2>();
        _currentMove.x = _currentMoveInput.x * walkSpeed;
        _currentMove.z = _currentMoveInput.y * walkSpeed;
        _currentRunMove.x = _currentMoveInput.x * sprintSpeed;
        _currentRunMove.z = _currentMoveInput.y * sprintSpeed;
        _isMovingPressed = _currentMoveInput.x != 0 || _currentMoveInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isSprintingPressed = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void SetupJump()
    {
        float timeToApex = maxJumpTime / 2;
        _gravity = (-2 * maxJumpTime) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isSprinting = _animator.GetBool(_isSprintingHash);
        //check is the character is moving and set animation
        if (_isMovingPressed && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true);
        }
        else if(!_isMovingPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }

        //check if the player is running and set animation
        if((_isMovingPressed && _isSprintingPressed) && !isSprinting)
        {
            _animator.SetBool(_isSprintingHash, true);
        }
        else if((!_isMovingPressed && !_isSprintingPressed) && isSprinting)
        {
            _animator.SetBool(_isSprintingHash, false);
        }
        else if ((_isMovingPressed && !_isSprintingPressed) && isSprinting)
        {
            _animator.SetBool(_isSprintingHash, false);
        }

    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = _currentMove.x;
        positionToLookAt.z = _currentMove.z;
        positionToLookAt.y = 0.0f;

        Quaternion currentRotation = transform.rotation;

        if (_isMovingPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactor * Time.deltaTime);
        }
        
    }

    void HandleGravity()
    {
        bool isFalling = _currentMove.y <= 0.0f;
        if (_charController.isGrounded)
        {
            float groundedGravity = -0.05f;
            _currentMove.y = groundedGravity;
            _currentRunMove.y = groundedGravity;
            _animator.SetBool(_isJumpingHash, false);
        }
        else if (isFalling)
        {
            _currentMove.y += Mathf.Min(_gravity * fallMultiplier * Time.deltaTime, -20.0f);
            _currentRunMove.y += Mathf.Min(_gravity * fallMultiplier * Time.deltaTime, -20.0f);
        }
        else
        {
            _currentMove.y += _gravity * Time.deltaTime;
            _currentRunMove.y += _gravity * Time.deltaTime;
        }
    }

    void HandleJump()
    {
       
        if(!_isJumping && _charController.isGrounded && _isJumpPressed)
        {
            _isJumping = true;
            _currentMove.y = _initialJumpVelocity;
            _currentRunMove.y = _initialJumpVelocity;
            _animator.SetBool(_isJumpingHash, true);
        }
        else if(!_isJumpPressed && _charController.isGrounded && _isJumping)
        {
            _isJumping = false;            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (_isSprintingPressed)
        {
            _charController.Move(_currentRunMove * Time.deltaTime);
        }
        else
        {
            _charController.Move(_currentMove * Time.deltaTime);
        }

        HandleRotation();
        HandleGravity();
        HandleAnimation();
        HandleJump();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
