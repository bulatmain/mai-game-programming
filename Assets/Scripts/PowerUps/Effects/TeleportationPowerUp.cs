using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;
public class TeleportationPowerUp : PowerUpBase
{
    [FormerlySerializedAs("radius")]
    [SerializeField] private float m_radius = 30f;
    [FormerlySerializedAs("teleportationEffectPrefabs")]
    [SerializeField] private List<GameObject> m_teleportationEffectPrefabs;

    private List<GameObject> m_activeTeleportationEffects = new List<GameObject>();
    private GameObject m_activatingPlayer;
    private GameObject m_playerWithMaxDistance;

    public override void ActivatePowerUp(GameObject player)
    {

        PlayerPowerUpHolder manager = player.GetComponentInParent<PlayerPowerUpHolder>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");

            m_activatingPlayer = player.transform.root.gameObject;

            manager.StorePowerUp(this, StartTeleportation);
        }
    }

    private GameObject PlayerToTeleport(GameObject player)
    {

        RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, m_radius, Vector3.up, 10f);

        float maxDistance = 0;
        GameObject farthestPlayer = null;

        foreach (RaycastHit hit in hits)
        {

            if (hit.transform.CompareTag("Player") && hit.transform.gameObject != player)
            {

                float distance = Vector3.Distance(hit.transform.position, player.transform.position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPlayer = hit.transform.gameObject;
                }

                Debug.DrawLine(player.transform.position, hit.transform.position, Color.green, 2f);
            }
        }

        return farthestPlayer;
    }

    public void StartTeleportation()
    {

        m_playerWithMaxDistance = PlayerToTeleport(m_activatingPlayer);

        PlayTeleportationEffects(m_activatingPlayer);

        if (m_playerWithMaxDistance != null)
        {
            PlayTeleportationEffects(m_playerWithMaxDistance);
        }

        StartCoroutine(TeleportWithDelay());
    }

    private IEnumerator TeleportWithDelay()
    {
        yield return new WaitForSeconds(1.5f);

        StopTeleportationEffects();

        Teleport();
    }

    public void Teleport()
    {
        if (m_activatingPlayer == null || m_playerWithMaxDistance == null)
            return;

        Vector3 previousPosition = m_activatingPlayer.transform.position;

        m_activatingPlayer.transform.position = m_playerWithMaxDistance.transform.position + (Vector3.up * 0.5f);

        m_playerWithMaxDistance.transform.position = previousPosition + (Vector3.up * 0.5f);

        IsUsed = true;

        m_activatingPlayer = null;
        m_playerWithMaxDistance = null;
    }

    private void PlayTeleportationEffects(GameObject playerToPlayEffect)
    {
        foreach (var effectPrefab in m_teleportationEffectPrefabs)
        {

            GameObject instance = Instantiate(effectPrefab, playerToPlayEffect.transform);
            m_activeTeleportationEffects.Add(instance);

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    private void StopTeleportationEffects()
    {
        foreach (var vfx in m_activeTeleportationEffects)
        {
            if (vfx != null)
                Destroy(vfx);
        }

        m_activeTeleportationEffects.Clear();
    }

    private void OnDrawGizmos()
    {
        if (m_activatingPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_activatingPlayer.transform.position, m_radius);
        }
    }
}
