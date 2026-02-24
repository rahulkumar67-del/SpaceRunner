using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsSpaceship : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Stats")]
    public float constantThrust = 1000f; // Always moving forward
    public float boostThrust = 4000f;    // Extra force when pressing 'W'
    public float strafeForce = 3000f;

    [Header("Rotation Stats")]
    public float pitchSpeed = 1000f;
    public float yawSpeed = 1000f;
    public float rollSpeed = 800f;
    [Header("Rotation")]
    public float rotationSpeed = 1000f;
    public float stabilityStrength = 5f; // How fast it stops spinning
    private Vector2 mouseInput;
    private float rollInput;
    [Header("Roll Settings")]
    private float currentRollAngle = 0f;


    [Header("Rotation Limits")]
    public float maxRotationSpeed = 2f; // The "Speed Limit"
    public float rotationPower = 500f;  // How fast it reaches that limit
    public float stopPower = 10f;       // How fast it stops when you let go


   [ Header("Limits")]
    public float maxPitchAngle = 60f; // Max degrees up/down
    public float maxRollAngle = 45f;  // Max degrees bank left/right
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Drag is essential to prevent the ship from spinning forever
        rb.linearDamping = 1f;
       

        // This is the most important line for your request
        rb.maxAngularVelocity = maxRotationSpeed;

        // Drag helps the ship feel less like it's on ice
        rb.angularDamping = 2f;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleLimitedRotation();
        
    }

    void HandleMovement()
    {
        // 1. Constant Forward Force
        Vector3 totalForce = Vector3.forward * constantThrust;

       
        float vInput = Input.GetAxis("Vertical");
        if (vInput > 0) totalForce += Vector3.forward * vInput * boostThrust;
        else if (vInput < 0) totalForce += Vector3.forward * vInput * (constantThrust * 0.8f);

        
        float hInput = Input.GetAxis("Horizontal");
        totalForce += Vector3.right * hInput * strafeForce;

        
        rb.AddRelativeForce(totalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    void HandleLimitedRotation()
    {
        
        float pitchInput = -Input.GetAxis("Mouse Y");
        float yawInput = Input.GetAxis("Mouse X");
        float rollInput = (Input.GetKey(KeyCode.Q) ? 1 : 0) - (Input.GetKey(KeyCode.E) ? 1 : 0);

        // 2. Get Current Angles (Normalized to -180 to 180)
        float currentPitch = NormalizeAngle(transform.localEulerAngles.x);
        float currentRoll = NormalizeAngle(transform.localEulerAngles.z);

        // 3. Logic: Only apply torque if we are WITHIN the limits OR trying to move BACK to center
        float finalPitch = 0;
        if (Mathf.Abs(currentPitch) < maxPitchAngle || (currentPitch > 0 && pitchInput < 0) || (currentPitch < 0 && pitchInput > 0))
        {
            finalPitch = pitchInput * rotationPower;
        }

        float finalRoll = 0;
        if (Mathf.Abs(currentRoll) < maxRollAngle || (currentRoll > 0 && rollInput < 0) || (currentRoll < 0 && rollInput > 0))
        {
            finalRoll = rollInput * rotationPower;
        }

        // 4. Apply the Torque
        Vector3 torque = new Vector3(finalPitch, yawInput * rotationPower, finalRoll);
        rb.AddRelativeTorque(torque * Time.fixedDeltaTime, ForceMode.Acceleration);

        // 5. Auto-Stabilize (Stops the drift when keys are released)
        if (pitchInput == 0 && yawInput == 0 && rollInput == 0)
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, stopPower * Time.fixedDeltaTime);
        }
    }

    // Helper to convert 0-360 angles to -180 to 180 for easier math
    float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }
}