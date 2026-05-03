using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{

    public int checkpointID;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            other.GetComponentInParent<LapTracker>()?.CheckpointPassed(checkpointID);
        }
    }
}
