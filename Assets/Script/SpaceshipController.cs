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

    [Header("Rotation Limits")]
    public float maxRotationSpeed = 2f; // The "Speed Limit"
    public float rotationPower = 500f;  // How fast it reaches that limit
    public float stopPower = 10f;       // How fast it stops when you let go


   [Header("Limits")]
    public float maxPitchAngle = 60f; // Max degrees up/down
    public float maxRollAngle = 45f;  // Max degrees bank left/right

    bool hasReachedTopSpeed = false;
    public float speedThreshold = 50f;
    [Header("Boost Settings")]
    public float maxBoostMultiplier = 3.0f; // Max boost is 3x the base thrust
    public float spoolSpeed = 0.5f;        // How fast it reaches max boost
    public float currentBoostFactor = 1.0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1f;
        rb.maxAngularVelocity = maxRotationSpeed;
        rb.angularDamping = 2f;
    }
   
    private void FixedUpdate()
    {

        float currentSpeed = rb.linearVelocity.magnitude;
        Debug.Log("Current Speed: " + currentSpeed);
        if (currentSpeed >= speedThreshold && !hasReachedTopSpeed)
        {
            
            hasReachedTopSpeed = true;
        }
      
        HandleMovement();
        HandleLimitedRotation();

        
    }

    private void HandleMovement()
    {
        Vector3 totalForce = Vector3.forward * constantThrust;
        // 1. Constant Forward Force

        if (Input.GetAxis("Vertical") > 0)
        {
            // Increase the multiplier over time, capped at maxBoostMultiplier
            currentBoostFactor += spoolSpeed * Time.fixedDeltaTime;
            currentBoostFactor = Mathf.Min(currentBoostFactor, maxBoostMultiplier);
        }
        else
        {
            // Decay the boost back to 1.0 when not thrusting
            currentBoostFactor -= spoolSpeed * 2f * Time.fixedDeltaTime;
            currentBoostFactor = Mathf.Max(currentBoostFactor, 1.0f);
        }

        // Apply the force using the multiplier
        // constantThrust is always there, but Vertical input gets amplified
        Vector3 thrust = Vector3.forward * (constantThrust + (Input.GetAxis("Vertical") * boostThrust * currentBoostFactor));

        rb.AddRelativeForce(thrust * Time.fixedDeltaTime, ForceMode.Acceleration);

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


    float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided");
        }
    }

}