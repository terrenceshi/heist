using UnityEngine;

public class simpleController : MonoBehaviour
{
    public GameObject playerCamera;
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;
    private CharacterController characterController;
    public float ySpeedMin = -6.0f;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private bool jumped;
    private Vector3 current_movement;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        jumped = false;
    }


    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;
        ySpeed = Mathf.Clamp(ySpeed, ySpeedMin, jumpSpeed);

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
            jumped = true;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod && jumped)
        {
            ySpeed = -0.5f;
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && jumped)
            {
                ySpeed = jumpSpeed;
                jumped = false;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }

        Vector3 velocity = movementDirection * magnitude;

        current_movement = new Vector3(velocity.x, velocity.y, velocity.z); // I want this here before Physics gets involved.

        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            // rbody.MoveRotation(rbody.rotation * Quaternion.AngleAxis(inputTurn * Time.deltaTime * turnMaxSpeed, Vector3.up));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }

}
