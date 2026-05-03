using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MomentumBooster : MonoBehaviour
{
    public float rayDistance = 20f;
    public float boostMultiplier = 1.2f;
    public float boostDuration = 1f;

    private Rigidbody m_rb;
    private VehicleController controller;
    private float m_originalAcceleration;
    private Coroutine m_activeBoost;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public void SetPowerUp(VehicleController controller)
    {
        this.controller = controller;
        m_originalAcceleration = controller.GetAcceleration();
    }

    void FixedUpdate()
    {

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance))
        {

            if (hit.collider.CompareTag("Player"))
            {

                float mass = hit.transform.gameObject.GetComponent<Rigidbody>().mass;

                float proximity = (mass > m_rb.mass) ? (rayDistance - hit.distance) / rayDistance : 0;

                if (proximity > 0.3f)
                {
                    TriggerBoost();
                }
            }
        }
    }

    void TriggerBoost()
    {
        Debug.Log($"<color=#00ff66>[SLIPSTREAM]</color> {gameObject.name} caught draft -> +{(boostMultiplier - 1f) * 100f:F0}% acceleration for {boostDuration:F1}s");

        if (m_activeBoost != null)
        {
            StopCoroutine(m_activeBoost);
            controller.SetAcceleration(m_originalAcceleration);
        }

        m_activeBoost = StartCoroutine(ApplyBoost());
    }

    IEnumerator ApplyBoost()
    {
        controller.SetAcceleration(m_originalAcceleration * boostMultiplier);

        yield return new WaitForSeconds(boostDuration);

        controller.SetAcceleration(m_originalAcceleration);

        m_activeBoost = null;
    }
}
