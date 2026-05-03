using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitchController : MonoBehaviour
{

    public CinemachineCamera mainCamera;

    public CinemachineCamera lookBackCamera;

    public PlayerInput playerInput;

    private bool m_usingChaseCam = true;

    private void Start()
    {

        playerInput.actions["SwitchCamera"].performed += ctx => SwitchCameraView();
    }

    public void SetPlayerInput(PlayerInput input)
    {
        this.playerInput = input;
    }

    private void SwitchCameraView()
    {
        m_usingChaseCam = !m_usingChaseCam;

        mainCamera.Priority = m_usingChaseCam ? 10 : 0;
        lookBackCamera.Priority = m_usingChaseCam ? 0 : 10;
    }
}
