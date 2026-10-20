using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camara orbital minima en tercera persona. Ponela en la Main Camera
/// y arrastra al jugador en "target".
/// </summary>
public class SimpleThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float pivotHeight = 1.4f;
    public float sensitivity = 0.12f;
    public float stickSensitivity = 120f;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public float followSmooth = 12f;
    public LayerMask collisionMask = ~0;

    float yaw, pitch = 15f;
    float smoothedPivotY;
    InputAction lookAction;

    void Awake()
    {
        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");
    }

    void OnEnable()
    {
        lookAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        lookAction.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDestroy() { lookAction?.Dispose(); }

    void Start()
    {
        yaw = transform.eulerAngles.y;
        if (target != null) smoothedPivotY = target.position.y + pivotHeight;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 look = lookAction.ReadValue<Vector2>();
        bool fromMouse = lookAction.activeControl != null && lookAction.activeControl.device is Mouse;
        if (fromMouse) look *= sensitivity;
        else look *= stickSensitivity * Time.deltaTime;

        yaw += look.x;
        pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);

        // El pivote baja un poco suavemente cuando el jugador se agacha
        float desiredY = target.position.y + pivotHeight;
        smoothedPivotY = Mathf.Lerp(smoothedPivotY, desiredY, followSmooth * Time.deltaTime);
        Vector3 pivot = new Vector3(target.position.x, smoothedPivotY, target.position.z);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.back;

        // Evitar que la camara atraviese paredes
        float dist = distance;
        if (Physics.SphereCast(pivot, 0.2f, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.transform.IsChildOf(target))
                dist = Mathf.Max(0.5f, hit.distance);
        }

        transform.position = pivot + dir * dist;
        transform.rotation = rot;
    }
}
