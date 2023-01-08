using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovementController : NetworkBehaviour
{
    //reference variables
    PlayerInput playerInput;
    CharacterController charController;
    Animator animator;

    //variables to store player input values
    Vector2 currentMoveInput;
    Vector3 currentMove;
    bool isMovingPressed;
    bool isSprintingPressed;

    //editable variables in the editor for playtesting
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float rotationFactor;
    [SerializeField] float gravity;

    //variables to handle animation
    int isWalkingHash;
    int isSprintingHash;

    private void Awake()
    {
        //assign variables
        playerInput = new PlayerInput();
        charController = GetComponent<CharacterController>();

        //setup animation
        animator = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isSprintingHash = Animator.StringToHash("isSprinting");

        //setup player input
        playerInput.CharacterControls.Move.started += OnMoveInput;
        playerInput.CharacterControls.Move.canceled += OnMoveInput;
        playerInput.CharacterControls.Move.performed += OnMoveInput;
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

    }
    void OnMoveInput(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
        currentMove.x = currentMoveInput.x;
        currentMove.z = currentMoveInput.y;
        isMovingPressed = currentMoveInput.x != 0 || currentMoveInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isSprintingPressed = context.ReadValueAsButton();
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSprinting = animator.GetBool(isSprintingHash);
        //check is the character is moving and set animation
        if (isMovingPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if(!isMovingPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        //check if the player is running and set animation
        if((isMovingPressed && isSprintingPressed) && !isSprinting)
        {
            animator.SetBool(isSprintingHash, true);
        }
        else if((!isMovingPressed && !isSprintingPressed) && isSprinting)
        {
            animator.SetBool(isSprintingHash, false);
        }
        else if ((isMovingPressed && !isSprintingPressed) && isSprinting)
        {
            animator.SetBool(isSprintingHash, false);
        }

    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMove.x;
        positionToLookAt.z = currentMove.z;
        positionToLookAt.y = 0.0f;

        Quaternion currentRotation = transform.rotation;

        if (isMovingPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactor * Time.deltaTime);
        }
        
    }

    void HandleGravity()
    {
        if (charController.isGrounded)
        {
            float groundedGravity = -0.05f;
            currentMove.y = groundedGravity;
        }
        else
        {
            currentMove.y -= gravity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (isSprintingPressed)
        {
            charController.Move(currentMove * sprintSpeed * Time.deltaTime);
        }
        else
        {
            charController.Move(currentMove * walkSpeed * Time.deltaTime);
        }
        HandleRotation();
        HandleGravity();
        HandleAnimation();
        

    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
