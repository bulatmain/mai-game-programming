using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHudController : MonoBehaviour
{

    [FormerlySerializedAs("powerUpIcon")]
    [SerializeField] private Image m_powerUpIcon;

    [FormerlySerializedAs("parryCooldownBar")]
    [SerializeField] private Image m_parryCooldownBar;

    [FormerlySerializedAs("healthBar")]
    [SerializeField] private Image m_healthBar;

    [FormerlySerializedAs("imageList")]
    [SerializeField] private List<RawImage> m_imageList;

    private ParryHandler m_parrySystem;

    private DurabilityHandler health;

    private PlayerPowerUpHolder m_PUM;

    private PowerUpBase m_storedPowerUp;

    public void SetGUI(Image parryBar, Image health)
    {
        m_parryCooldownBar = parryBar;
        m_healthBar = health;
    }

    private void Start()
    {
        m_PUM = GetComponent<PlayerPowerUpHolder>();
        m_parrySystem = GetComponent<ParryHandler>();
        health = GetComponent<DurabilityHandler>();
    }

    void Update()
    {

        if (m_parrySystem != null && m_parryCooldownBar != null)
            m_parryCooldownBar.fillAmount = m_parrySystem.GetParryCooldownNormalized();

        if (health != null && m_healthBar != null)
            m_healthBar.fillAmount = health.GetDurabilityNormalized();
    }
}
