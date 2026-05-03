using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSpawner : MonoBehaviour
{

    public void Initialize(
        RawImage image,
        VehicleController controller,
        TextMeshProUGUI progressText,
        TextMeshProUGUI LapText,
        RaceCheckpoint[] checkPoints,
        Image parryCD,
        Image HealthBar,
        CinemachineCamera cam,
        ParticleSystem explosion)
    {

        GetComponent<PlayerPowerUpHolder>()?.SetPowerUpImage(image);

        GetComponent<LapTracker>()?.SetGUI(progressText, LapText);
        GetComponent<LapTracker>()?.SetCheckPointsList(checkPoints);

        GetComponent<VehicleController>()?.SetPlayerCamera(cam);

        GetComponent<DurabilityHandler>()?.SetUpParticleSystem(explosion);

        GetComponent<MomentumBooster>()?.SetPowerUp(controller);

        GetComponent<PlayerHudController>()?.SetGUI(parryCD, HealthBar);
    }
}
