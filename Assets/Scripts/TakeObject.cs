using UnityEngine;
using UnityEngine.InputSystem;

public class TakeObject : MonoBehaviour
{
    public GameObject handPoint;
    [SerializeField] private float dropDelay = 2f;  // segundos mínimos con el objeto en la mano antes de soltarlo
    [SerializeField] private float pickDelay = 1f;  // segundos que hay que esperar después de soltar para volver a agarrar

    private GameObject pickedObject = null;
    private float _pickedTime;
    private float _lastDropTime = -999f; // valor muy negativo: al arrancar el juego se puede agarrar sin esperar

    void Update()
    {
        if (pickedObject == null) return;
        if (Keyboard.current == null) return;

        bool delayPassed = Time.time >= _pickedTime + dropDelay;

        if (Keyboard.current.eKey.wasPressedThisFrame && delayPassed)
        {
            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            pickedObject.transform.SetParent(null);
            pickedObject = null;

            _lastDropTime = Time.time; // guarda el momento en que lo soltó
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Keyboard.current == null) return;
        if (pickedObject != null) return;
        if (Time.time < _lastDropTime + pickDelay) return; // todavía en cooldown de agarre
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        if (!other.TryGetComponent<Rigidbody>(out Rigidbody rb)) return;

        rb.useGravity = false;
        rb.isKinematic = true;
        other.transform.position = handPoint.transform.position;
        other.transform.SetParent(handPoint.transform);

        pickedObject = other.gameObject;
        _pickedTime = Time.time;
    }
}