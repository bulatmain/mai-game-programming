using System.Collections;
using UnityEngine;

public class DemoDirector : MonoBehaviour
{
    public float startDelay = 0.5f;
    public float deformPhase = 2.5f;
    public float slipstreamPhase = 10f;

    public Vector3 demoCenter = new Vector3(800f, 0.5f, 350f);
    public Vector3 demoForward = new Vector3(1f, 0f, 0f);

    private VehicleController m_carA;
    private VehicleController m_carB;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);

        var setup = GetComponent<RaceSetupManager>();
        if (setup != null) setup.maxLaps = 99;
        Time.timeScale = 1f;

        var cars = FindObjectsByType<VehicleController>(FindObjectsSortMode.None);
        if (cars.Length < 2)
        {
            Debug.LogError("[Demo] Need 2 cars, found " + cars.Length);
            yield break;
        }
        m_carA = cars[0];
        m_carB = cars[1];

        DisableDriver(m_carA);
        DisableDriver(m_carB);

        foreach (var c in cars)
        {
            var d = c.GetComponent<DurabilityHandler>();
            if (d != null) d.enabled = false;

            var df = c.GetComponent<DamageDeformer>();
            if (df != null) Destroy(df);

            var pi = c.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (pi != null) pi.DeactivateInput();
        }

        yield return RunSlipstreamDemo();

        Debug.Log("[Demo] Sequence complete.");
    }

    private void DisableDriver(VehicleController car)
    {
        var ap = car.GetComponent<VehicleAutoPilot>();
        if (ap != null) ap.enabled = false;
        car.useBotInput = true;
        car.botInput = Vector2.zero;
    }

    private IEnumerator RunDeformationDemo()
    {
        Debug.Log("[Demo] PHASE 1: DEFORMATION");

        Vector3 fwd = demoForward.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

        Rigidbody rbA = m_carA.GetComponent<Rigidbody>();
        Rigidbody rbB = m_carB.GetComponent<Rigidbody>();

        m_carA.transform.SetPositionAndRotation(
            demoCenter - fwd * 40f,
            Quaternion.LookRotation(fwd, Vector3.up));
        m_carB.transform.SetPositionAndRotation(
            demoCenter + fwd * 40f,
            Quaternion.LookRotation(-fwd, Vector3.up));

        rbA.linearVelocity = Vector3.zero; rbA.angularVelocity = Vector3.zero;
        rbB.linearVelocity = Vector3.zero; rbB.angularVelocity = Vector3.zero;

        yield return new WaitForFixedUpdate();
        Debug.Log("[Demo] cars positioned for head-on collision");

        yield return new WaitForSeconds(1.5f);

        rbA.linearVelocity = fwd * 60f;
        rbB.linearVelocity = -fwd * 60f;
        Debug.Log("[Demo] HEAD-ON LAUNCHED at 60 m/s each");

        yield return new WaitForSeconds(deformPhase);
    }

    private IEnumerator RunSlipstreamDemo()
    {
        Debug.Log("[Demo] PHASE 2: SLIPSTREAM");

        Vector3 fwd = demoForward.normalized;
        Rigidbody rbA = m_carA.GetComponent<Rigidbody>();
        Rigidbody rbB = m_carB.GetComponent<Rigidbody>();

        var dfA = m_carA.GetComponent<DamageDeformer>();
        var dfB = m_carB.GetComponent<DamageDeformer>();
        if (dfA != null) dfA.ResetDeformation();
        if (dfB != null) dfB.ResetDeformation();

        float originalMassA = rbA.mass;
        float originalMassB = rbB.mass;
        rbA.mass = originalMassA * 6f;
        rbB.mass = originalMassB;
        Debug.Log($"[Demo] masses set A={rbA.mass} B={rbB.mass}");

        Vector3 anchor = demoCenter + fwd * 5f;
        m_carA.transform.SetPositionAndRotation(
            anchor + fwd * 8f,
            Quaternion.LookRotation(fwd, Vector3.up));
        m_carB.transform.SetPositionAndRotation(
            anchor,
            Quaternion.LookRotation(fwd, Vector3.up));

        rbA.linearVelocity = fwd * 18f;
        rbB.linearVelocity = fwd * 18f;
        rbA.angularVelocity = Vector3.zero;
        rbB.angularVelocity = Vector3.zero;

        m_carA.useBotInput = true;
        m_carB.useBotInput = true;
        m_carA.botInput = new Vector2(0f, 0.55f);
        m_carB.botInput = new Vector2(0f, 1f);

        float t = 0f;
        float lastLog = -1f;
        while (t < slipstreamPhase)
        {
            if (t - lastLog > 1f)
            {
                Debug.Log($"[Demo] t={t:F1}s  speedA={rbA.linearVelocity.magnitude:F1}  speedB={rbB.linearVelocity.magnitude:F1}  gap={(m_carA.transform.position - m_carB.transform.position).magnitude:F1}");
                lastLog = t;
            }
            t += Time.deltaTime;
            yield return null;
        }

        rbA.mass = originalMassA;
        rbB.mass = originalMassB;
    }
}
