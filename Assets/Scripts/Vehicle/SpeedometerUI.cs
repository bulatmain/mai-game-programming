using TMPro;
using UnityEngine;

public class SpeedometerUI : MonoBehaviour
{
    public Rigidbody target;

    public float maxSpeed = 200f;

    public float minSpeedArrowAngle = -90f;
    public float maxSpeedArrowAngle = 90f;

    [Header("UI")]
    public TextMeshProUGUI speedLabel;
    public RectTransform arrow;

    private float m_speed = 0.0f;

    public void SetTarget(Rigidbody rb)
    {
        target = rb;
    }

    private void Update()
    {
        if (target == null) return;

        m_speed = target.linearVelocity.magnitude * 3.6f;

        float clampedSpeed = Mathf.Clamp(m_speed, 0, maxSpeed);

        if (speedLabel != null)
            speedLabel.text = ((int)m_speed).ToString() + " km/h";

        if (arrow != null)
        {
            float angle = Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, clampedSpeed / maxSpeed);
            arrow.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
}
