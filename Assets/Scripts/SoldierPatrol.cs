

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierPatrol : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 2f;
    [SerializeField] float waitTime = 0.5f;
    [SerializeField] LayerMask carryLayers;      // Player + Carryable
    [SerializeField] float boundsMargin = 0.2f;  // tolerancia para no soltar al player en el borde

    int index = 0;
    int dir = 1;
    float waitTimer;
    Collider triggerCol;
    readonly HashSet<Transform> riders = new HashSet<Transform>();

    void Awake()
    {
        foreach (var c in GetComponents<Collider>())
        {
            if (c.isTrigger) { triggerCol = c; break; }
        }

        if (triggerCol == null)
            Debug.LogWarning($"{name}: falta un Collider con Is Trigger activado.", this);
    }

    void Start()
    {
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    void LateUpdate()
    {
        if (waypoints.Length < 2) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 target = waypoints[index].position;
        Vector3 before = transform.position;
        transform.position = Vector3.MoveTowards(before, target, speed * Time.deltaTime);
        Vector3 delta = transform.position - before;

        // Sacar de la lista a quien ya no está encima (teleport, destruido, etc.)
        if (triggerCol != null)
        {
            Bounds b = triggerCol.bounds;
            b.Expand(boundsMargin);
            riders.RemoveWhere(r => r == null || !b.Contains(r.position));
        }

        foreach (var r in riders)
        {
            if (r.TryGetComponent(out CharacterController cc))
                cc.Move(delta);
            else
                r.position += delta;
        }

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            waitTimer = waitTime;
            if (index + dir >= waypoints.Length || index + dir < 0)
                dir *= -1;
            index += dir;
        }
    }

    bool InLayerMask(GameObject go) => (carryLayers.value & (1 << go.layer)) != 0;

    Transform GetRoot(Collider c) =>
        c.attachedRigidbody != null ? c.attachedRigidbody.transform : c.transform;

    void OnTriggerEnter(Collider other)
    {
        if (InLayerMask(other.gameObject))
            riders.Add(GetRoot(other));
    }

    void OnTriggerExit(Collider other)
    {
        if (InLayerMask(other.gameObject))
            riders.Remove(GetRoot(other));
    }

    // Opcional: llamalo desde el portal si querés soltar al player explícitamente
    public void Release(Transform t) => riders.Remove(t);

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.15f);
            if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}

