using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private string controlScheme = "Player1";

    private PlayerControls controls;
    private InputAction dashAction;
    private InputAction throwAction;
    private InputAction useAction;

    private Vector2 moveInput;
    private bool interactPressed;
    private bool interactHeld;
    private bool dashPressed;
    private bool throwPressed;
    private bool usePressed;

    public Vector2 Move => Vector2.ClampMagnitude(moveInput, 1f);
    public bool InteractPressedThisFrame => interactPressed;
    public bool InteractHeld => interactHeld;
    public bool DashPressedThisFrame => dashPressed;
    public bool ThrowPressedThisFrame => throwPressed;
    public bool UsePressedThisFrame => usePressed;

    public void ConfigureControlScheme(string scheme)
    {
        controlScheme = scheme;
        if (controls != null)
        {
            controls.bindingMask = InputBinding.MaskByGroup(controlScheme);
        }
    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.bindingMask = InputBinding.MaskByGroup(controlScheme);

        dashAction = controls.FindAction("Dash");
        throwAction = controls.FindAction("Throw");
        useAction = controls.FindAction("Use") ?? controls.FindAction("UseItem");
    }

    private void OnEnable()
    {
        SubscribeInput();
        controls.Enable();
    }

    private void OnDisable()
    {
        UnsubscribeInput();
        controls.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    public void ConsumeFrameFlags()
    {
        interactPressed = false;
        dashPressed = false;
        throwPressed = false;
        usePressed = false;
    }

    private void SubscribeInput()
    {
        controls.Player.Move.started += OnMove;
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;

        controls.Player.Interact.started += OnInteractStarted;
        controls.Player.Interact.performed += OnInteractPerformed;
        controls.Player.Interact.canceled += OnInteractCanceled;

        if (dashAction != null)
        {
            dashAction.started += OnDash;
        }

        if (throwAction != null)
        {
            throwAction.started += OnThrow;
        }

        if (useAction != null)
        {
            useAction.started += OnUse;
        }
    }

    private void UnsubscribeInput()
    {
        controls.Player.Move.started -= OnMove;
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;

        controls.Player.Interact.started -= OnInteractStarted;
        controls.Player.Interact.performed -= OnInteractPerformed;
        controls.Player.Interact.canceled -= OnInteractCanceled;

        if (dashAction != null)
        {
            dashAction.started -= OnDash;
        }

        if (throwAction != null)
        {
            throwAction.started -= OnThrow;
        }

        if (useAction != null)
        {
            useAction.started -= OnUse;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnInteractStarted(InputAction.CallbackContext _)
    {
        interactPressed = true;
        interactHeld = true;
    }

    private void OnInteractPerformed(InputAction.CallbackContext _)
    {
        interactPressed = true;
    }

    private void OnInteractCanceled(InputAction.CallbackContext _)
    {
        interactHeld = false;
    }

    private void OnDash(InputAction.CallbackContext _)
    {
        dashPressed = true;
    }

    private void OnThrow(InputAction.CallbackContext _)
    {
        throwPressed = true;
    }

    private void OnUse(InputAction.CallbackContext _)
    {
        usePressed = true;
    }
}
