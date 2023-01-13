using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerStateMachine : NetworkBehaviour
{
    // Start is called before the first frame update
    //reference variables
    PlayerInput _playerInput;
    CharacterController _charController;
    public CharacterController CharController { get { return _charController; } }
    Animator _animator;
    public Animator Animator { get { return _animator; } }

    //variables to store player input values
    Vector2 _currentMoveInput;
    Vector3 _currentMove;
    Vector3 _currentRunMove;
    Vector3 _appliedMovement;
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    bool _isMovingPressed;
    bool _isSprintingPressed;
    bool _isJumpPressed = false;
    public bool IsJumpPressed { get { return _isJumpPressed; }}

    //editable variables in the editor for playtesting
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float rotationFactor;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float maxJumpTime;
    [SerializeField] float fallMultiplier;
    public float FallMultiplier { get { return fallMultiplier; } }

    //variables to handle animation
    int _isWalkingHash;
    public int IsWalkingHash { get { return _isWalkingHash; } }
    int _isSprintingHash;
    public int IsSprintingHash { get { return _isSprintingHash; } }
    int _isJumpingHash;
    public int IsJumpingHash { get { return _isJumpingHash; } }
    int _isCrouchingHash;
    public int IsCrouchingHash { get { return _isCrouchingHash; } }

    //variables to handle jumping
    float _initialJumpVelocity;
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    bool _isJumping = false;
    public bool IsJumping { set { _isJumping = value; } }
    float _gravity;
    public float Gravity { get { return _gravity; } }

    //state variables 
    PlayerBaseState _currentState;
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }   }
    PlayerStateFactory _states;


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

        //setup states
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    void SetupJump()
    {
        float timeToApex = maxJumpTime / 2;
        _gravity = (-2 * maxJumpTime) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    // Update is called once per frame
    void Update()
    {
         HandleRotation();
        _charController.Move(_appliedMovement * Time.deltaTime);
        _currentState.UpdateState();
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

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

}
