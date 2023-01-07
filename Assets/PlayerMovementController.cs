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

    //variables to store player input values
    Vector2 currentMoveInput;
    Vector3 currentMove;
    bool isMoving;

    private void Awake()
    {
        playerInput = new PlayerInput();
        charController = GetComponent<CharacterController>();

        playerInput.CharacterControls.Move.started += OnMoveInput;
        playerInput.CharacterControls.Move.canceled += OnMoveInput;
        playerInput.CharacterControls.Move.performed += OnMoveInput;

    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
        currentMove.x = currentMoveInput.x;
        currentMove.z = currentMoveInput.y;
        isMoving = currentMoveInput.x != 0 || currentMoveInput.y != 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        charController.Move(currentMove * Time.deltaTime);

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
