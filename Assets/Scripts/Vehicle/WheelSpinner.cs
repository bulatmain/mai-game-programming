using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform[] wheels;
    public float wheelRadius = 0.5f;
    public float maxSpeed = 100f;

    private Rigidbody m_carRigidbody;

    private void Start()
    {
        m_carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RotateWheels();
    }

    private void RotateWheels()
    {
        if (m_carRigidbody == null) return;

        float carSpeed = m_carRigidbody.linearVelocity.magnitude;

        carSpeed = Mathf.Min(carSpeed, maxSpeed);

        float wheelRotationSpeed = (carSpeed / (2 * Mathf.PI * wheelRadius)) * 360f;

        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(wheelRotationSpeed * Time.fixedDeltaTime, 0f, 0f, Space.Self);
        }
    }
}
