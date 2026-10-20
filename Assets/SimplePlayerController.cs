using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Character simple: caminar, correr, saltar, agacharse y prone.
/// Sin animaciones. Requiere el paquete Input System.
/// 
/// Setup:
///  - Objeto vacio (pivot en los PIES) con este script + CharacterController.
///  - Hijo opcional: una Capsule primitiva (sin collider) como "visual", con localPosition (0,1,0).
///    Asignala en el campo "visual" para que se achique al agacharte.
///  - Una camara con el tag MainCamera (podes usar SimpleThirdPersonCamera).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    // 0 = parado, 1 = agachado, 2 = prone
    int stance = 0;
    readonly float[] heights = { 1.8f, 1.0f, 0.2f };
    readonly float[] speeds = { 4f, 2f, 0.8f };

    CharacterController cc;
    Transform visual;
    float vy;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cc.radius = 0.3f;
        if (transform.childCount > 0) visual = transform.GetChild(0);
        SetHeight(heights[0]);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Posturas
        if (kb.cKey.wasPressedThisFrame) { stance = stance == 1 ? 0 : 1; SetHeight(heights[stance]); }
        if (kb.zKey.wasPressedThisFrame) { stance = stance == 2 ? 1 : 2; SetHeight(heights[stance]); }
        if (kb.spaceKey.wasPressedThisFrame && stance != 0) { stance = 0; SetHeight(heights[0]); }

        // Movimiento relativo a la camara
        float x = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);
        float z = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);

        Transform cam = Camera.main.transform;
        Vector3 fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        Vector3 dir = Vector3.ClampMagnitude(fwd * z + right * x, 1f);

        float speed = speeds[stance];
        if (stance == 0 && kb.leftShiftKey.isPressed) speed = 7f;

        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);

        // Gravedad y salto
        if (cc.isGrounded && vy < 0) vy = -2f;
        if (cc.isGrounded && stance == 0 && kb.spaceKey.wasPressedThisFrame) vy = 6f;
        vy -= 20f * Time.deltaTime;

        cc.Move((dir * speed + Vector3.up * vy) * Time.deltaTime);
    }

    void SetHeight(float h)
    {
        cc.height = h;
        cc.center = new Vector3(0, h / 2f, 0);
        if (visual != null)
        {
            visual.localScale = new Vector3(0.6f, h / 2f, 0.6f);
            visual.localPosition = new Vector3(0, h / 2f, 0);
        }
    }
}
