using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedBoostPowerUp : PowerUpBase
{

    public float speedMultiplier = 2.0f;

    public float duration = 2.0f;

    private float m_originalSpeed;

    private Coroutine m_activeBoost;

    private VehicleController m_controller;
    private CinemachineCamera m_playerCamera;

    public override void ActivatePowerUp(GameObject player)
    {

        PlayerPowerUpHolder manager = player.GetComponentInParent<PlayerPowerUpHolder>();
        if (manager != null)
        {

            player = player.transform.root.gameObject;
            Debug.Log("Speed Boost Stored!");

            m_controller = player.GetComponent<VehicleController>();
            m_playerCamera = m_controller.GetPlayerCamera();

            if (m_controller == null)
            {
                Debug.LogWarning("SpeedBoostPowerUp: Missing m_controller ");
                return;
            }
            if (m_playerCamera == null)
            {
                Debug.LogWarning("Missing camera");
            }

            m_originalSpeed = m_controller.GetAcceleration();

            manager.StorePowerUp(this, () => ApplySpeedBoost(speedMultiplier, duration));
        }
    }

    public void ApplySpeedBoost(float boostMultiplier, float boostDuration)
    {
        if (m_controller == null) return;

        if (m_activeBoost != null)
        {
            m_controller.StopCoroutine(m_activeBoost);
            m_controller.SetAcceleration(m_originalSpeed);
        }

        m_activeBoost = m_controller.StartCoroutine(ApplyBoost(boostMultiplier, boostDuration));
    }

    private IEnumerator ApplyBoost(float boostMultiplier, float boostDuration)
    {
        Debug.Log("Speed Boost Activated!");

        if (m_controller != null)
            m_controller.SetAcceleration(boostMultiplier * m_originalSpeed);

        yield return m_controller != null ? m_controller.StartCoroutine(ChangeFOV(70, 100, 0.3f)) : null;

        yield return new WaitForSeconds(boostDuration);

        if (m_controller != null)
            m_controller.SetAcceleration(m_originalSpeed);

        yield return m_controller != null ? m_controller.StartCoroutine(ChangeFOV(100, 70, 0.3f)) : null;

        IsUsed = true;

        m_activeBoost = null;
    }

    private IEnumerator ChangeFOV(float from, float to, float duration)
    {
        if (m_playerCamera == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            m_playerCamera.Lens.FieldOfView = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        m_playerCamera.Lens.FieldOfView = to;
    }
}
