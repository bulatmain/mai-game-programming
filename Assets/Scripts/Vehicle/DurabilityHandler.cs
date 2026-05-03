using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class DurabilityHandler : MonoBehaviour
{
    [FormerlySerializedAs("maxDurability")]
    [SerializeField] private float m_maxDurability = 100f;
    [FormerlySerializedAs("damageMultiplyer")]
    [SerializeField] private float m_damageMultiplyer = 0.01f;

    private ParryHandler m_parrySystem;
    private Rigidbody m_rb;
    private bool m_isParrying;
    ParticleSystem explosion;
    private float m_currentDurability;
    PlayerInput playerInput;

    public void SetUpParticleSystem(ParticleSystem explosion)
    {
        this.explosion = explosion;
    }

    public float GetDurabilityNormalized()
    {
        return Mathf.Clamp01(m_currentDurability / m_maxDurability);
    }

    private void Start()
    {

        m_rb = GetComponent<Rigidbody>();
        m_parrySystem = GetComponent<ParryHandler>();
        playerInput = GetComponent<PlayerInput>();
        m_currentDurability = m_maxDurability;
    }

    private void Update()
    {

        if (playerInput != null && playerInput.actions["Destroy"].WasPressedThisFrame())
        {
            OnCarDestroyed();
        }

        if (m_parrySystem != null)
            m_isParrying = m_parrySystem.isParrying;
    }

    private void OnCollisionEnter(Collision collision)
    {

        bool isTargetRelevant = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Obstacle");
        if (m_isParrying || !isTargetRelevant)
        {
            return;
        }

        GameObject other = collision.gameObject;
        VehicleController otherController = other.GetComponentInParent<VehicleController>();

        if (otherController != null && otherController.isAbleToOneShot)
        {
            Debug.Log("Destroyed instantly");
            OnCarDestroyed();
            return;
        }

        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        float damage = impactForce * m_damageMultiplyer;

        Debug.Log($"Applying {damage:F1} damage from impact force.");
        ApplyDamage(damage);
    }

    void ApplyDamage(float damage)
    {
        m_currentDurability -= damage;
        m_currentDurability = Mathf.Clamp(m_currentDurability, 0, m_maxDurability);

        Debug.Log($"Took {damage:F1} damage! Remaining: {m_currentDurability}");

        if (m_currentDurability <= 0)
        {
            OnCarDestroyed();
        }
    }

    void OnCarDestroyed()
    {

        if (explosion != null)
        {
            ParticleSystem explosion_ = Instantiate(
                explosion,
                new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z),
                Quaternion.identity
            );
            explosion_.Play();

            Destroy(explosion_.gameObject, explosion_.main.duration + explosion_.main.startLifetime.constantMax);
        }

        gameObject.transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);

        Invoke(nameof(Respawn), 3f);
    }

    void Respawn()
    {
        m_currentDurability = m_maxDurability;

        m_rb.linearVelocity = Vector3.zero;
        m_rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(true);
    }
}
