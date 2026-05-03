using System.Collections;
using UnityEngine;

public class DemoAutoplayBootstrapper : MonoBehaviour
{
    public int forceMaxLaps = 5;
    public float attachDelay = 0.5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(attachDelay);

        var setup = GetComponent<RaceSetupManager>();
        if (setup != null && forceMaxLaps > 0) setup.maxLaps = forceMaxLaps;

        var cars = FindObjectsByType<VehicleController>(FindObjectsSortMode.None);
        foreach (var car in cars)
        {
            if (car.GetComponent<VehicleAutoPilot>() == null)
                car.gameObject.AddComponent<VehicleAutoPilot>();
            car.useBotInput = true;

            var d = car.GetComponent<DurabilityHandler>();
            if (d != null) d.enabled = false;
        }

        if (cars.Length >= 2)
        {
            var rbA = cars[0].GetComponent<Rigidbody>();
            var rbB = cars[1].GetComponent<Rigidbody>();
            if (rbA != null && rbB != null)
            {
                rbA.mass = rbB.mass * 5f;
                Debug.Log($"[Autoplay] Slipstream-friendly masses: {cars[0].name}={rbA.mass}  {cars[1].name}={rbB.mass}");
            }
        }

        Debug.Log($"[DemoAutoplayBootstrapper] Attached VehicleAutoPilot to {cars.Length} car(s).");
    }
}
