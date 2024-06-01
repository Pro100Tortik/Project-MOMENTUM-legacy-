using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private InputMap _inputs;
    private InputMap.GameplayActions _gameplay;
    private InputMap.MenusActions _menu;

    private void Awake()
    {
        _inputs = new InputMap();
        _gameplay = _inputs.Gameplay;
        _menu = _inputs.Menus;
    }

    public Vector2 GetMoveInput() => _gameplay.Move.ReadValue<Vector2>();

    public Vector2 GetLookInput() => _gameplay.Look.ReadValue<Vector2>();

    public bool GetJumpInput() => _gameplay.Jump.IsPressed();

    private void OnEnable() => _inputs.Enable();

    private void OnDisable() => _inputs.Disable();
}
