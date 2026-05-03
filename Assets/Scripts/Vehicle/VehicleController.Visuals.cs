using UnityEngine;

using UnityEngine.Serialization;
public partial class VehicleController
{
    public void TireVisuals()
    {
        float steeringAngle = m_maxSteeringAngle * m_steerInput;

        for (int i = 0; i < m_frontTireParent.Length; i++)
        {
            m_frontTireParent[i].transform.localEulerAngles = new Vector3(m_frontTireParent[i].transform.localEulerAngles.x, steeringAngle, m_frontTireParent[i].transform.localEulerAngles.z);
        }
    }

    private void Vfx()
    {
        bool shouldEmit = m_isGrounded && Mathf.Abs(m_currentCarVelocity.x) > m_minSideSkidVelocity;

        if (shouldEmit)
            m_skidTimer += Time.fixedDeltaTime;
        else
            m_skidTimer = 0f;

        bool allowVFX = m_skidTimer > m_skidDelay;

        if (allowVFX != m_skidActive)
        {
            ToggleSkidMarks(allowVFX);
            ToggleSkidSmokes(allowVFX);
            ToggleSkidSound(allowVFX);
            m_skidActive = allowVFX;
        }
    }

    private void ToggleSkidMarks(bool toggle)
    {
        foreach (var skidMark in m_skidMarks)
        {
            foreach (var mark in m_skidMarks)
            {
                if (mark.emitting != toggle)
                    mark.emitting = toggle;
            }
        }
    }

    private void ToggleSkidSmokes(bool toggle)
    {
        foreach (var smoke in m_skidSmokes)
        {
            if (toggle)
            {
                if (!smoke.isPlaying)
                    smoke.Play();
            }
            else
            {
                if (smoke.isPlaying)
                    smoke.Stop();
            }
        }
    }
}
