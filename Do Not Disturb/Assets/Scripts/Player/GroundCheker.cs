
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Boxcast Property")]
    [SerializeField] private Vector3 boxSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private float maxDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = false;

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position - transform.up * maxDistance, boxSize);
    }

    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position, boxSize, -transform.up, Quaternion.identity, maxDistance, groundLayer);
    }
}
