using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerStateMachine : NetworkBehaviour
{
    //reference variables
    PlayerInput _playerInput;
    CharacterController _charController;
    Camera _cam;
    public CharacterController CharController { get { return _charController; } }
    Animator _animator;
    public Animator Animator { get { return _animator; } }

    //variables to store player input values
    Vector2 _currentMoveInput;
    Vector3 _currentMove;
    public float CurrentMoveX { get { return _currentMove.x; } set { _currentMove.x = value; } }
    public float CurrentMoveZ { get { return _currentMove.z; } set { _currentMove.z = value; } }

    Vector3 _currentRunMove;
    public float CurrentRunMoveX { get { return _currentRunMove.x; } set { _currentRunMove.x = value; } }
    public float CurrentRunMoveZ { get { return _currentRunMove.z; } set { _currentRunMove.z = value; } }

    Vector3 _currentSneakMove;
    public float CurrentSneakMoveX { get { return _currentSneakMove.x; } set { _currentSneakMove.x = value; } }
    public float CurrentSneakMoveZ { get { return _currentSneakMove.z; } set { _currentSneakMove.z = value; } }

    Vector3 _appliedMovement;
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    Vector3 _camRelativeMovement;

    bool _isMovingPressed;
    public bool IsMovingPressed { get { return _isMovingPressed;  } }
    bool _isSprintingPressed;
    public bool IsSprintingPressed { get { return _isSprintingPressed; } }
    bool _isJumpPressed = false;
    public bool IsJumpPressed { get { return _isJumpPressed; }}
    bool _isCrouchPressed = false;
    public bool IsCrouchPressed { get { return _isCrouchPressed; } }
    bool _requiresNewJumpPress = false;
    public bool RequiresNewJumpPress { get { return _requiresNewJumpPress; } set { _requiresNewJumpPress = value; } }

    //editable variables in the editor for playtesting
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float sneakSpeed;
    public float SneakSpeed { get { return sneakSpeed; } }
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
        _cam = Camera.main;

        //setup animation
        _animator = GetComponentInChildren<Animator>();
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isSprintingHash = Animator.StringToHash("isSprinting");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isCrouchingHash = Animator.StringToHash("isCrouched");

        //setup player input
        _playerInput.CharacterControls.Move.started += OnMoveInput;
        _playerInput.CharacterControls.Move.canceled += OnMoveInput;
        _playerInput.CharacterControls.Move.performed += OnMoveInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        _playerInput.CharacterControls.Crouch.started += OnCrouch;
        _playerInput.CharacterControls.Crouch.canceled += OnCrouch;

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

        _camRelativeMovement = ConvertToCamSpace(_appliedMovement);
        _charController.Move(_camRelativeMovement * Time.deltaTime);
         HandleRotation();
        _currentState.UpdateStates();
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = _camRelativeMovement.x;
        positionToLookAt.z = _camRelativeMovement.z;
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
        _currentSneakMove.x = _currentMoveInput.x * sneakSpeed;
        _currentSneakMove.z = _currentMoveInput.y * sneakSpeed;
        _isMovingPressed = _currentMoveInput.x != 0 || _currentMoveInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isSprintingPressed = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requiresNewJumpPress = false;
    }

    void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouchPressed = context.ReadValueAsButton();
    }

    Vector3 ConvertToCamSpace(Vector3 vectorToRotate)
    {
        float yValue = vectorToRotate.y;
        
        //get the forward and right directional vectors of the camera
        Vector3 camForward = _cam.transform.forward;
        Vector3 camRight = _cam.transform.right;

        //ignore up and down camera angles
        camForward.y = 0f;
        camRight.y = 0f;

        //normalise the vectors again
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 camForwardZProduct = vectorToRotate.z * camForward;
        Vector3 camForwardXProduct = vectorToRotate.x * camRight;
        Vector3 rotatedVector = camForwardXProduct + camForwardZProduct;
        rotatedVector.y = yValue;
        return rotatedVector;
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
