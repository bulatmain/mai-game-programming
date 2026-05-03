using UnityEngine;

using UnityEngine.Serialization;
public partial class VehicleController
{
    private void EngineSound()
    {
        m_engineSound.pitch = Mathf.Lerp(m_minPitch, m_maxPitch, Mathf.Abs(m_carVelocityRatio));
    }

    private void ToggleSkidSound(bool toggle)
    {
        if (toggle && !m_skidSound.isPlaying)
            m_skidSound.Play();
        else if (!toggle && m_skidSound.isPlaying)
            m_skidSound.Stop();
    }
}
