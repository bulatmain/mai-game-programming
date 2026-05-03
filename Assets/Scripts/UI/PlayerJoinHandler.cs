using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class PlayerJoinHandler : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private bool m_usingGamepad = false;

    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();

        if (m_playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }

        AssignControlScheme();

        InputSystem.onDeviceChange += OnDeviceChange;

        m_playerInput.onControlsChanged += OnControlsChanged;
    }

    private void AssignControlScheme()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {

            Debug.Log("[INFO] Two gamepads detected. Assigning second gamepad to Player 1.");
            m_playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
            m_usingGamepad = true;
        }
        else
        {

            Debug.Log("[INFO] Using keyboard & mouse for Player 1.");
            m_playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            m_usingGamepad = false;
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log($"[DEVICE] Gamepad added. Re-evaluating input for Player 1.");

                if (!m_usingGamepad && Gamepad.all.Count >= 2)
                {
                    AssignControlScheme();
                }
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log($"[DEVICE] Gamepad removed. Re-evaluating input for Player 1.");

                if (m_usingGamepad && Gamepad.all.Count < 2)
                {
                    AssignControlScheme();
                }
            }
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        Debug.Log($"[CHANGE] Player 1 scheme changed to: {input.currentControlScheme}");
    }

    private void OnDestroy()
    {

        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
