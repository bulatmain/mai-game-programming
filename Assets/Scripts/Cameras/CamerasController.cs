using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamerasController : MonoBehaviour
{
    [Header("Player 1 Camera Rig")]
    public CinemachineCamera p1DefaultCam;
    public CinemachineCamera p1LookBackCam;
    public CameraLookController p1CameraLook;
    public CameraSwitchController p1CameraSwitcher;

    public void Setup(int playerNumber, GameObject followTarget, PlayerInput playerInput)
    {
        if (followTarget == null) return;
        if (playerNumber != 1) return;

        if (p1DefaultCam != null)
        {
            p1DefaultCam.Follow = followTarget.transform;
            p1DefaultCam.LookAt = followTarget.transform;
        }

        if (p1LookBackCam != null)
        {
            p1LookBackCam.Follow = followTarget.transform;
            p1LookBackCam.LookAt = followTarget.transform;
        }

        if (p1CameraLook != null) p1CameraLook.SetPlayer(followTarget);
        if (p1CameraSwitcher != null) p1CameraSwitcher.SetPlayerInput(playerInput);
    }
}
