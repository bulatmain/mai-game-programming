using UnityEngine;

public class PowerUpSpinner : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;

    private Vector3 m_startPos;

    void Start()
    {
        m_startPos = transform.position;
    }

    void Update()
    {

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        float newY = m_startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = new Vector3(m_startPos.x, newY, m_startPos.z);
    }
}
