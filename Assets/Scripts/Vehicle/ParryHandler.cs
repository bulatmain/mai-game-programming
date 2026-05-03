using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class ParryHandler : MonoBehaviour
{
    private Rigidbody m_rb;
    private Vector3 m_preHitVelocity;
    private PlayerInput m_playerInput;

    [Header("Parry Timing")]
    [FormerlySerializedAs("perfectParryWindow")]
    [SerializeField] private float m_perfectParryWindow = 0.2f;
    [FormerlySerializedAs("totalParryDuration")]
    [SerializeField] private float m_totalParryDuration = 0.4f;
    [FormerlySerializedAs("parryCooldownTime")]
    [SerializeField] private float m_parryCooldownTime = 1.5f;

    [Header("Parry Effects")]
    [FormerlySerializedAs("baseBoostForce")]
    [SerializeField] private float m_baseBoostForce = 500f;

    private float m_parryTimer;
    private float m_parryCooldown;
    [HideInInspector] public bool isParrying = false;

    public float GetParryCooldownNormalized()
    {
        return Mathf.Clamp01(m_parryCooldown / m_parryCooldownTime);
    }

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();

        if (m_playerInput != null && m_playerInput.actions != null)
            m_playerInput.actions["Parry"].performed += OnParryPerformed;
    }

    private void OnDestroy()
    {

        if (m_playerInput != null)
            m_playerInput.actions["Parry"].performed -= OnParryPerformed;
    }

    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        TryParry();
    }

    public void TryParry()
    {
        if (m_parryCooldown <= 0f && !isParrying)
        {
            isParrying = true;
            m_parryTimer = m_totalParryDuration;
        }
    }

    void FixedUpdate()
    {

        m_preHitVelocity = m_rb.linearVelocity;

        if (m_parryCooldown > 0f)
            m_parryCooldown -= Time.fixedDeltaTime;
    }

    void Update()
    {
        if (isParrying)
        {

            m_parryTimer -= Time.deltaTime;

            if (m_parryTimer <= 0f)
            {
                isParrying = false;
                m_parryCooldown = m_parryCooldownTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!isParrying) return;

        bool isPerfectParry = m_parryTimer > (m_totalParryDuration - m_perfectParryWindow);

        if (isPerfectParry)
        {
            Debug.Log("Perfect Parry!");
            NullifyCollision();
            ApplyBoost(1.5f);
        }
        else
        {
            Debug.Log("Normal Parry");
            NullifyCollision();
            ApplyBoost(1.0f);
        }

        isParrying = false;
        m_parryCooldown = m_parryCooldownTime;
    }

    void NullifyCollision()
    {
        m_rb.linearVelocity = m_preHitVelocity;
        m_rb.angularVelocity = Vector3.zero;
    }

    void ApplyBoost(float multiplier)
    {
        float boostForce = m_baseBoostForce * multiplier;
        m_rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
    }
}
