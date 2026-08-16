using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
{
    public static InputManager Instance { get; private set; }

    public InputSystem_Actions Actions { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }

    public event Action JumpEvent;
    public event Action AttackEvent;
    public event Action InteractEvent;
    public event Action<bool> SprintEvent;
    public event Action<bool> CrouchEvent;
    public event Action PreviousEvent;
    public event Action NextEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Actions = new InputSystem_Actions();
        Actions.Player.SetCallbacks(this);
        Actions.UI.SetCallbacks(this);
    }

    private void OnEnable()
    {
        Actions.Player.Enable();
    }

    private void OnDisable()
    {
        Actions.Player.Disable();
        Actions.UI.Disable();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        Actions.Player.RemoveCallbacks(this);
        Actions.UI.RemoveCallbacks(this);
        Actions.Dispose();
        Instance = null;
    }

    public void SwitchToGameplayMap()
    {
        Actions.UI.Disable();
        Actions.Player.Enable();
    }

    public void SwitchToUIMap()
    {
        Actions.Player.Disable();
        Actions.UI.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) AttackEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) InteractEvent?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsCrouching = true;
            CrouchEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            IsCrouching = false;
            CrouchEvent?.Invoke(false);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) JumpEvent?.Invoke();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed) PreviousEvent?.Invoke();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed) NextEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsSprinting = true;
            SprintEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            IsSprinting = false;
            SprintEvent?.Invoke(false);
        }
    }

    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}
