using UnityEngine;

public class ClearZone : MonoBehaviour
{
    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = destination.position;
    }
}