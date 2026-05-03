using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPowerUpHolder : MonoBehaviour
{

    public PowerUpBase storedPowerUp;

    private System.Action m_storedEffect;

    private VehicleController m_controller;
    private PlayerInput m_playerInput;

    [FormerlySerializedAs("powerUpImage")]
    [SerializeField] private RawImage m_powerUpImage;
    [FormerlySerializedAs("emptyPowerUpIcon")]
    [SerializeField] private Texture m_emptyPowerUpIcon;

    private void Start()
    {

        m_controller = GetComponent<VehicleController>();
        m_playerInput = GetComponent<PlayerInput>();

        if (m_playerInput != null && m_playerInput.actions != null)
            m_playerInput.actions["UsePowerUp"].performed += OnUsePowerUp;
    }

    public void SetPowerUpImage(RawImage image)
    {
        m_powerUpImage = image;
    }

    public void StorePowerUp(PowerUpBase powerUp, System.Action effect)
    {
        storedPowerUp = powerUp;
        m_storedEffect = effect;

        if (powerUp.iconTexture != null && m_powerUpImage != null)
            m_powerUpImage.texture = powerUp.iconTexture;
    }

    private void OnUsePowerUp(InputAction.CallbackContext context)
    {
        TryUsePowerUp();
    }

    public void TryUsePowerUp()
    {
        if (storedPowerUp != null)
        {

            m_storedEffect?.Invoke();

            if (m_controller != null) m_controller.SetHasActivePowerUp(false);

            if (m_powerUpImage != null)
                m_powerUpImage.texture = m_emptyPowerUpIcon;

            storedPowerUp = null;
            m_storedEffect = null;
        }
    }
}
