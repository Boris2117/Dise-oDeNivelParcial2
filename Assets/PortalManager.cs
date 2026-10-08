using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [SerializeField] Transform _blue;
    [SerializeField] Transform _orange;

    void Awake() => Instance = this;

    public Transform GetPortal(PortalType type)
    {
        return type == PortalType.Blue ? _blue : _orange;
    }

}
