using UnityEngine;
public enum PortalType { Blue, Orange }
public class portal : MonoBehaviour
{
    [SerializeField] PortalType _type;
    [SerializeField] LayerMask _wallLayers;
    [SerializeField] float _offset = 0.01f;
    [SerializeField] Vector3 _rotationOffset = new Vector3(-90f, 0f, 0f);

    void OnCollisionEnter(Collision collision)
    {

        bool isWall = (_wallLayers.value & (1 << collision.gameObject.layer)) != 0;

        if (isWall)
        {
            ContactPoint contact = collision.GetContact(0);
            Transform portal = PortalManager.Instance.GetPortal(_type);

            portal.position = contact.point + contact.normal * _offset;
            portal.rotation = Quaternion.LookRotation(-contact.normal) * Quaternion.Euler(0f, 180f, 0f); 
            portal.gameObject.SetActive(true);
           
        }

        Destroy(gameObject); // se destruye siempre al chocar
    }
}
