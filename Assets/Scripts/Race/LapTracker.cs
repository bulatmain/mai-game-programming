using System.Linq;
using TMPro;
using UnityEngine;

public class LapTracker : MonoBehaviour
{
    public int totalCheckpoints = 5;
    public int currentCheckpointIndex = 0;
    public int lapCount = 0;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI lapText;

    public float raceProgress => (currentCheckpointIndex + m_partialProgressToNext) / totalCheckpoints;

    private float m_partialProgressToNext = 0f;

    [SerializeField]
    private RaceCheckpoint[] m_checkpointPositions;

    void Update()
    {
        UpdatePartialProgress();

        if (progressText != null)
            progressText.text = ((int)(raceProgress * 100f)).ToString() + "%";
    }

    public void SetGUI(TextMeshProUGUI progress, TextMeshProUGUI lap)
    {
        progressText = progress;
        lapText = lap;
    }

    public Vector3? GetNextCheckpointPosition()
    {
        if (m_checkpointPositions == null || m_checkpointPositions.Length == 0) return null;
        int idx = Mathf.Clamp(currentCheckpointIndex, 0, m_checkpointPositions.Length - 1);
        return m_checkpointPositions[idx].transform.position;
    }

    public void SetCheckPointsList(RaceCheckpoint[] list)
    {
        m_checkpointPositions = list.OrderBy(c => c.checkpointID).ToArray();
        totalCheckpoints = m_checkpointPositions.Length;
    }

    private void UpdatePartialProgress()
    {

        if (currentCheckpointIndex < m_checkpointPositions.Length - 1)
        {
            Vector3 currentPos = m_checkpointPositions[currentCheckpointIndex].transform.position;
            Vector3 nextPos = m_checkpointPositions[currentCheckpointIndex + 1].transform.position;
            Vector3 playerPos = transform.position;

            float totalDistance = Vector3.Distance(currentPos, nextPos);
            float playerDistance = Vector3.Distance(playerPos, nextPos);

            m_partialProgressToNext = Mathf.Clamp01(1f - (playerDistance / totalDistance));
        }
        else
        {

            m_partialProgressToNext = 0f;
        }
    }

    public void CheckpointPassed(int checkpointID)
    {

        if (checkpointID == currentCheckpointIndex)
        {
            currentCheckpointIndex++;

            if (currentCheckpointIndex >= totalCheckpoints)
            {

                foreach(GameObject powerUp in WorldPowerUpManager.list)
                {
                    if (powerUp != null)
                    {
                        powerUp.SetActive(true);
                    }
                }

                currentCheckpointIndex = 0;
                lapCount++;
                if (lapText != null)
                    lapText.text = lapCount.ToString();

                Debug.Log("Lap Completed! Total Laps: " + lapCount);
            }
        }
        else
        {
            Debug.Log("Wrong checkpoint!");
        }
    }
}
