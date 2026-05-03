using UnityEngine;
using System.Collections.Generic;

public class WorldPowerUpManager : MonoBehaviour
{

    public static List<GameObject> list = new List<GameObject>();

    public List<PowerUpBase> powerUps;

    private PowerUpBase m_chosenPowerUp;

    private void Awake()
    {

        if (!list.Contains(gameObject))
            list.Add(gameObject);
    }

    private void Start()
    {

        if (powerUps.Count > 0)
        {
            m_chosenPowerUp = powerUps[Random.Range(0, powerUps.Count)];
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player")) return;

        var player = other.GetComponentInParent<VehicleController>();

        if (player == null || m_chosenPowerUp == null || player.HasActivePowerUp()) return;

        player.SetHasActivePowerUp(true);

        m_chosenPowerUp.ActivatePowerUp(other.gameObject);

        gameObject.SetActive(false);
    }
}
