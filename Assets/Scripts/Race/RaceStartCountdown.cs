using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaceStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;

    private GameObject[] m_playersToFreeze;

    public void SetPlayersToFreeze(params GameObject[] cars)
    {
        if (cars == null) { m_playersToFreeze = new GameObject[0]; return; }
        var list = new System.Collections.Generic.List<GameObject>();
        foreach (var c in cars) if (c != null) list.Add(c);
        m_playersToFreeze = list.ToArray();
    }

    public void FreezePlayers()
    {
        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {

        foreach (var player in m_playersToFreeze)
        {
            if (player == null) continue;
            var pi = player.GetComponent<PlayerInput>();
            if (pi != null && pi.enabled) pi.DeactivateInput();
            var bot = player.GetComponent<VehicleAutoPilot>();
            if (bot != null) bot.enabled = false;
            var car = player.GetComponent<VehicleController>();
            if (car != null)
            {
                car.botInput = Vector2.zero;
                car.botDrifting = false;
            }
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        float timeLeft = countdownDuration;

        while (timeLeft > 0)
        {
            countdownText.text = Mathf.Ceil(timeLeft).ToString();
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);

        foreach (var player in m_playersToFreeze)
        {
            if (player == null) continue;
            var pi = player.GetComponent<PlayerInput>();
            if (pi != null && pi.enabled) pi.ActivateInput();
            var bot = player.GetComponent<VehicleAutoPilot>();
            if (bot != null) bot.enabled = true;
        }
    }
}
