

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierPatrol : MonoBehaviour
{

    public List<Transform> waypoints = new List<Transform>();


    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float arrivalDistance = 0.2f;


    public float waitMin = 2f;
    public float waitMax = 5f;

    private int currentWaypoint = 0;
    private bool waiting = false;

    public float visionRange = 10f;
    [Range(0f, 360f)]
    public float visionAngle = 90f;
    public Color visionColor = Color.red;

    void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("Asigná los waypoints al soldado.");
            enabled = false;
        }
    }

    void Update()
    {
        if (waiting || waypoints.Count == 0)
            return;

        Transform target = waypoints[currentWaypoint];

        if (target == null)
            return;


        Vector3 direction = target.position - transform.position;
        direction.y = 0f;


        if (direction.magnitude <= arrivalDistance)
        {
            StartCoroutine(WaitAtWaypoint());
            return;
        }


        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );


        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }

    IEnumerator WaitAtWaypoint()
    {
        waiting = true;

        yield return new WaitForSeconds(
            Random.Range(waitMin, waitMax)
        );

        currentWaypoint = (currentWaypoint + 1) % waypoints.Count;

        waiting = false;
    }


    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Gizmos.color = visionColor;
        Gizmos.DrawWireSphere(origin, visionRange);

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 leftBoundary = Quaternion.Euler(
            0f, -visionAngle / 2f, 0f
        ) * forward;

        Vector3 rightBoundary = Quaternion.Euler(
            0f, visionAngle / 2f, 0f
        ) * forward;

        Gizmos.DrawRay(origin, leftBoundary * visionRange);
        Gizmos.DrawRay(origin, rightBoundary * visionRange);

        int segments = 40;
        Vector3 previousPoint = origin + leftBoundary * visionRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -visionAngle / 2f
                        + visionAngle * i / segments;

            Vector3 direction = Quaternion.Euler(
                0f, angle, 0f
            ) * forward;

            Vector3 nextPoint = origin + direction * visionRange;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

}

