using UnityEngine;

using UnityEngine.Serialization;
public partial class VehicleController
{
    private void Movement()
    {
        if (m_isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
        }
    }

    private void Acceleration()
    {
        float effectiveMassFactor = Mathf.Pow(m_carRB.mass, m_customPower);
        float actualAcceleration = m_isDrifting ? m_acceleration * 0.7f : m_acceleration;

        Vector3 force = (transform.forward * actualAcceleration * m_moveInput) * (1f / effectiveMassFactor);
        m_carRB.AddForceAtPosition(force * 20, m_accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        float speedFactor = Mathf.Clamp01(m_carVelocityRatio);
        m_carRB.AddForceAtPosition(m_deceleration * m_moveInput * -transform.forward * speedFactor, m_accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        if (!m_isGrounded) return;

        float turnAmount = m_turnSpeed * m_steerInput * m_turningCurve.Evaluate(Mathf.Abs(m_carVelocityRatio));
        float directionMultiplier = (m_carVelocityRatio < 0) ? -1f : 1f;

        if (m_isDrifting)
            turnAmount *= 1.2f;

        m_carRB.AddTorque(turnAmount * directionMultiplier * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        if (!m_isGrounded) return;

        float rearSlipFactor = m_isDrifting ? m_rearGripWhenDrifting : m_rearGrip;
        float sidewaysSpeed = m_currentCarVelocity.x;

        float dragForce = -sidewaysSpeed * m_dragCoefficient * rearSlipFactor;
        m_carRB.AddForce(transform.right * dragForce, ForceMode.Acceleration);
    }

    private void YawStabilizer()
    {
        if (!m_isGrounded || !m_isDrifting) return;

        float angularY = m_carRB.angularVelocity.y;
        float correction = -angularY * 0.5f;
        m_carRB.AddTorque(Vector3.up * correction, ForceMode.Acceleration);
    }

    private void CalculateCarVelocity()
    {
        m_currentCarVelocity = transform.InverseTransformDirection(m_carRB.linearVelocity);
        m_carVelocityRatio = Mathf.Clamp(m_currentCarVelocity.z / m_maxSpeed, -1f, 1f);
    }
}
