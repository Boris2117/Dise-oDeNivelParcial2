using UnityEngine;

public class ButtonInteractive : MonoBehaviour
{
    [SerializeField] GameObject Object;
    [SerializeField] Transform buttonTransform;   
    public float speed = 0.1f;                      
    public float maxPosition = 0.3f;              
    public float minPosition = 0.1f;              
    public bool inMax = false;
    public bool inMin = true;
    private int _objectsOnTop = 0;

    void Update()
    {
        float targetY = _objectsOnTop > 0 ? minPosition : maxPosition;

        Vector3 pos = buttonTransform.localPosition;
        pos.y = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        buttonTransform.localPosition = pos;

        if (Object == null) return;
        if(pos.y == minPosition)
        {
            Object.SetActive(inMin);
        }
        else Object.SetActive(inMax);
    }

    private void OnTriggerEnter(Collider other)
    {
        _objectsOnTop++;
    }

    private void OnTriggerExit(Collider other)
    {
        _objectsOnTop = Mathf.Max(0, _objectsOnTop - 1);
    }
}