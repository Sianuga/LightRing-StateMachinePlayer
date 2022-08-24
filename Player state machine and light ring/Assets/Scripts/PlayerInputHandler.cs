using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }

    public Vector2 YInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputFinished { get; private set; }

    private void Awake()
    {
        Controller2D_variant controller = GetComponent<Controller2D_variant>();
        Controls  controls = new Controls();
        controls.Enable();
        controls.Gameplay.Movement.performed += OnMoveInput;
        controls.Gameplay.Movement.canceled += OnMoveInput;
        controls.Gameplay.Jump.started += OnJumpInput;
        controls.Gameplay.Jump.canceled += OnJumpInput;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            JumpInput = true;
            JumpInputFinished = false;
        }
       if(context.canceled)
        {
            JumpInputFinished = true;
        }
        
    }


    public void UseJumpInput() => JumpInput = false;

}
