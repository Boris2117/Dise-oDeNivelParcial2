using UnityEngine;

public class SueloQueSube : MonoBehaviour
{
    [SerializeField] private float alturaExtra = 2f;
    [SerializeField] private float velocidad = 1.5f;

    private Vector3 posInicial;
    private Vector3 posFinal;
    private Vector3 destino;

    private void Start()
    {
        posInicial = transform.position;
        posFinal = posInicial + Vector3.up * alturaExtra;
        destino = posInicial;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);
    }

    public void Subir() { destino = posFinal; }
    public void Bajar() { destino = posInicial; }
}