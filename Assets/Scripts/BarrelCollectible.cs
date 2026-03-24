using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BarrelCollectible : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float bobHeight = 0.1f;
    public float bobSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        var col = GetComponent<BoxCollider>();
        if (col != null)
            col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        float bob = (Mathf.Sin(Time.time * bobSpeed) + 1f) * 0.5f * bobHeight;
        transform.position = new Vector3(startPosition.x, startPosition.y + bob, startPosition.z);
    }
}
