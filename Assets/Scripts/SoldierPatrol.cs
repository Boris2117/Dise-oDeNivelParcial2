

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierPatrol : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 2f;
    [SerializeField] float waitTime = 0.5f;
    [SerializeField] LayerMask carryLayers;   // Player + Carryable

    int index = 0;
    int dir = 1;
    float waitTimer;
    readonly HashSet<Transform> riders = new HashSet<Transform>();

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

        foreach (var r in riders)
        {
            if (r == null) continue;

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
}

