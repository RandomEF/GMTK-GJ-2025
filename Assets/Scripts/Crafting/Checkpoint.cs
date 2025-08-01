using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;
    public CraftBench craftBench;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Checkpoint touched");
        if (other.gameObject.tag == "Needle")
        {
            craftBench.EnteredCheckpoint(checkpointNumber);
        }
    }
}
