#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteInEditMode]
public class SplineRoadSampler : MonoBehaviour
{
    public SplineContainer m_splineContainer;

    [SerializeField] private int m_splineIndex;
    [SerializeField, UnityEngine.Range(0f, 1f)] private float m_time;

    [Tooltip("Half-width of the road")]
    [SerializeField] private float m_halfWidth = 3f;

    private float3 m_position;
    private float3 m_forward;
    private float3 m_upVector;

    public float GetKnotTime()
    {
        return m_time;
    }

    private void Update()
    {
        if (m_splineContainer == null) return;

        m_splineContainer.Evaluate(m_splineIndex, m_time, out m_position, out m_forward, out m_upVector);

        float3 right = math.normalize(math.cross(m_forward, m_upVector));

        float3 p1 = m_position + (right * m_halfWidth);
        float3 p2 = m_position - (right * m_halfWidth);

        Debug.DrawLine(m_position, p1, Color.green);
        Debug.DrawLine(m_position, p2, Color.red);
    }

    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        if (m_splineContainer == null) return;

        m_splineContainer.Evaluate(m_splineIndex, m_time, out float3 gizmoPos, out _, out _);

        Handles.color = Color.green;
        Handles.SphereHandleCap(0, gizmoPos, Quaternion.identity, 1f, EventType.Repaint);
    #endif
    }

    public void SampleSplineWidth(int splineIndex, float t, float width, out Vector3 p1, out Vector3 p2)
    {
        if (m_splineContainer == null)
        {
            p1 = p2 = Vector3.zero;
            return;
        }

        m_splineContainer.Evaluate(splineIndex, t, out float3 m_position, out float3 m_forward, out float3 up);
        float3 right = math.normalize(math.cross(m_forward, up));
        float3 offset = right * width * 0.5f;

        p1 = m_position + offset;
        p2 = m_position - offset;
    }
}
