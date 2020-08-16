using UnityEngine;

public class movementControllerFirstPerson : MonoBehaviour
{
    public new Transform camera;
    public Rigidbody rb;

    // Camera variable decleration
    [SerializeField] private float camRotationSpeed          = 5f;
    [SerializeField] private float camMinY                   = -60f;
    [SerializeField] private float camMaxY                   = 75f;
    [SerializeField] private float camRotationSmoothingSpeed = 10f;

    // Player movement variable decleration
    [SerializeField] private float walkSpeed     = 9f;
    [SerializeField] private float sprintSpeed   = 14f;
    [SerializeField] private float maxSpeed      = 20f;
    [SerializeField] private float jumpPow       = 30f;
    [SerializeField] private float betterGravity = 45;
    private float bodyRotationX;
    private float camRotationY;
    Vector3 directionIntentX;
    Vector3 directionIntentY;
    private float speed;
    public bool grounded;
    [SerializeField] private float crouchSpeed = 5f;

    void Update()
    {
        LookRotation();
        Grounded();
        ExtraGravity();
        if(grounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void LookRotation()
    {
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Get camera and body rotational values
        bodyRotationX += Input.GetAxis("Mouse X") * camRotationSpeed;
        camRotationY  += Input.GetAxis("Mouse Y") * camRotationSpeed;

        // Stop the camera from rotating 360 degrees
        camRotationY = Mathf.Clamp(camRotationY, camMinY, camMaxY);

        // Create rotation targets and handle rotations of the body and camera
        Quaternion camTargetRotation  = Quaternion.Euler(-camRotationY, 0, 0);
        Quaternion bodyTargetRotation = Quaternion.Euler(0, bodyRotationX, 0);

        // To handle the rotations
        transform.rotation   = Quaternion.Lerp(transform.rotation, bodyTargetRotation, Time.deltaTime * camRotationSmoothingSpeed);

        camera.localRotation = Quaternion.Lerp(camera.localRotation, camTargetRotation, Time.deltaTime * camRotationSmoothingSpeed);
    }

    void Movement()
    {
        // Making sure the direction matches the camera direction
        directionIntentX   = camera.right;
        directionIntentX.y = 0;
        directionIntentX.Normalize();

        directionIntentY   = camera.forward;
        directionIntentY.y = 0;
        directionIntentY.Normalize();

        // Change character velocity in the direction
        rb.velocity = directionIntentY * Input.GetAxis("Vertical") * speed + directionIntentX * Input.GetAxis("Horizontal") * speed + Vector3.up * rb.velocity.y;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // Control the speed based on movement state
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else if (Input.GetButton("Crouch"))
        {
            speed = crouchSpeed;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetButton("Crouch"))
            {
                speed = walkSpeed;
            }

    }

    void ExtraGravity()
    {
        rb.AddForce(Vector3.down * betterGravity);
    }

    void Grounded()
    {
        RaycastHit groundHit;
        grounded = Physics.Raycast(transform.position, -transform.up, out groundHit, 2.50f);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpPow, ForceMode.Impulse);
    }
}
