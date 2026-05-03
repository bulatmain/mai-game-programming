using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotKillPowerUp : PowerUpBase
{

    public bool isAbleToOneShot;

    private GameObject m_activatingPlayer;

    public override void ActivatePowerUp(GameObject player)
    {

        PlayerPowerUpHolder manager = player.GetComponentInParent<PlayerPowerUpHolder>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");

            m_activatingPlayer = player.transform.root.gameObject;

            manager.StorePowerUp(this, StartOneShot);
        }
    }

    private void StartOneShot()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {

        var controller = m_activatingPlayer.GetComponent<VehicleController>();

        if (controller != null)
        {
            controller.isAbleToOneShot = true;
            yield return new WaitForSeconds(1.5f);
            controller.isAbleToOneShot = false;
        }
    }

    private void OnDisable()
    {
        isAbleToOneShot = false;
    }
}
