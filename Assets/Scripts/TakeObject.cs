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
    private Rigidbody _candidate;
    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        if (pickedObject != null)
        {
            if (Time.time < _pickedTime + dropDelay) return;

            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            pickedObject.transform.SetParent(null);
            pickedObject = null;
            _lastDropTime = Time.time;
        }
        else if (_candidate != null && Time.time >= _lastDropTime + pickDelay)
        {
            _candidate.useGravity = false;
            _candidate.isKinematic = true;
            _candidate.transform.position = handPoint.transform.position;
            _candidate.transform.SetParent(handPoint.transform);

            pickedObject = _candidate.gameObject;
            _pickedTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb)) _candidate = rb;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_candidate != null && other.gameObject == _candidate.gameObject)
            _candidate = null;
    }
}