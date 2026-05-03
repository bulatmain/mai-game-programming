using UnityEngine;

using UnityEngine.Serialization;
public partial class VehicleController
{
    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < m_wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += m_wheelsIsGrounded[i];
        }

        m_isGrounded = tempGroundedWheels > 1;
    }

    private void Suspension()
    {
        for (int i = 0; i < m_rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = m_restLength + m_springTravel;

            if (Physics.Raycast(m_rayPoints[i].position, -m_rayPoints[i].up, out hit, maxLength + m_wheelRadius, m_drivable))
            {
                m_wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - m_wheelRadius;
                float springCompression = (m_restLength - currentSpringLength) / m_springTravel;
                float springVelocity = Vector3.Dot(m_carRB.GetPointVelocity(m_rayPoints[i].position), m_rayPoints[i].up);

                float suspensionForce = CalculateSuspensionForce(springCompression, springVelocity);
                ApplySuspensionForce(suspensionForce, m_rayPoints[i]);

                Debug.DrawLine(m_rayPoints[i].position, hit.point, UnityEngine.Color.red);
            }
            else
            {
                m_wheelsIsGrounded[i] = 0;
                Debug.DrawLine(m_rayPoints[i].position, m_rayPoints[i].position + (m_wheelRadius + maxLength) * -m_rayPoints[i].up, UnityEngine.Color.green);
            }
        }
    }

    private void ApplySuspensionForce(float suspensionForce, Transform rayPoint)
    {
        m_carRB.AddForceAtPosition(suspensionForce * rayPoint.up, rayPoint.position);

        Vector3 lateralVelocity = Vector3.ProjectOnPlane(m_carRB.GetPointVelocity(rayPoint.position), transform.forward);
        Vector3 lateralDampingForce = -lateralVelocity * m_dampingFactor;
        m_carRB.AddForceAtPosition(lateralDampingForce, rayPoint.position);
    }

    private float CalculateSuspensionForce(float springCompression, float springVelocity)
    {
        float springForce = m_springStiffness * springCompression;
        float dampForce = m_damperStiffness * springVelocity;
        return springForce - dampForce;
    }
}
