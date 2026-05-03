using UnityEngine;
using UnityEngine.Serialization;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TMPro;

public class CameraLookController : MonoBehaviour
{

    public CinemachineCamera mainCamera;

    private CinemachinePanTilt m_cameraTilt;

    public float sensitivity = 2f;

    public float minTilt = -10f;
    public float maxTilt = 10f;

    public TextMeshProUGUI sensetivityText;

    [FormerlySerializedAs("Player")]
    [SerializeField] private GameObject m_Player;

    private PlayerInput m_playerInput;

    private Vector2 m_lookInput;

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        sensetivityText.text = value.ToString("0.0");
        Debug.Log($"New sensitivity: {sensitivity}");
    }

    private void Start()
    {

        m_playerInput = m_Player.GetComponent<PlayerInput>();

        m_playerInput.actions["Look"].performed += ctx => m_lookInput = ctx.ReadValue<Vector2>();
        m_playerInput.actions["Look"].canceled += ctx => m_lookInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;

        m_cameraTilt = mainCamera.GetComponent<CinemachinePanTilt>();
    }

    public void SetPlayer(GameObject player)
    {
        m_Player = player;
    }

    private void Update()
    {

        if (m_playerInput == null || m_Player == null) return;

        float inputX = m_lookInput.x * sensitivity;
        float inputY = m_lookInput.y * sensitivity;

        m_cameraTilt.PanAxis.Value += inputX * Time.deltaTime * 50f;

        m_cameraTilt.TiltAxis.Value = Mathf.Clamp(
            m_cameraTilt.TiltAxis.Value - (inputY * Time.deltaTime * 50f),
            minTilt,
            maxTilt
        );
    }
}
