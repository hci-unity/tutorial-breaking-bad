using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 1.8f;
    public TextMeshProUGUI countLabelText;
    public TextMeshProUGUI countValueText;
    public GameObject winTextObject;

    private Rigidbody rb;
    private int count;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;
    private Vector3 movement;
    private Quaternion targetRotation;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        targetRotation = rb.rotation;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection += Vector3.forward;
            if (Keyboard.current.sKey.isPressed) inputDirection += Vector3.back;
            if (Keyboard.current.aKey.isPressed) inputDirection += Vector3.left;
            if (Keyboard.current.dKey.isPressed) inputDirection += Vector3.right;
        }

        inputDirection = inputDirection.normalized;

        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            movement = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
        }
        else
        {
            movement = inputDirection;
        }

        movement = movement.normalized;

        isSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && movement.magnitude > 0f;

        if (animator != null)
        {
            animator.SetFloat("Speed", isSprinting ? 2f : movement.magnitude);
        }

        if (movement != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(movement, Vector3.up);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        Vector3 targetVelocity = movement * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(targetRotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countValueText.text = ": " + count.ToString();

        if (count >= 10)
        {
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win";
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    public void OnCaughtByEnemy()
    {
        Destroy(gameObject);
        winTextObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "You l~e";
    }
}
