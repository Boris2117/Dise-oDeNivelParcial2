using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    [SerializeField] Transform _otherPortal; // arrastrá acá el OTRO portal

    static float _lastTeleport;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - _lastTeleport < 0.3f) return;

        var cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        other.transform.position = _otherPortal.position + _otherPortal.forward;

        if (cc != null) cc.enabled = true;

        _lastTeleport = Time.time;
    }
}
