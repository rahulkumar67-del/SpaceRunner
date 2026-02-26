using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsSpaceship : MonoBehaviour
{
    private Rigidbody rb;
    private CoreLogic CL;
    [Header("Movement Stats")]
    public float constantThrust = 1000f;
    public float boostThrust = 4000f;
    public float strafeForce = 3000f;
    private float initialBoostThrust; // To remember starting value

    [Header("Rotation Limits")]
    public float maxRotationSpeed = 2f;
    public float rotationPower = 500f;
    public float stopPower = 10f;
    public float maxPitchAngle = 60f;
    public float maxRollAngle = 45f;

    [Header("Boost Settings")]
    public float maxBoostLimit = 10000f;
  
    private bool isCollided = false;

    [Header("Boundary Limits")]
    public float horizontalLimit = 10f; // Max distance Left/Right from center
    public float verticalLimit = 5f;    // Max distance Up/Down from center
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        CL = GetComponent<CoreLogic>();
        rb.useGravity = false;
        rb.linearDamping = 1f;
        rb.maxAngularVelocity = maxRotationSpeed;
        rb.angularDamping = 2f;

        initialBoostThrust = boostThrust;
        Cursor.lockState = CursorLockMode.Locked;

        // Hides the cursor so it's invisible
        Cursor.visible = false;
        // START the coroutine ONCE here
        StartCoroutine(BoostProgressionCoroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Penalty: Drop boost thrust significantly
            boostThrust *= 0.5f;
            Debug.Log("collided");
            // Optional: prevent boost from growing for a few seconds
            StartCoroutine(CollisionCooldown());
        }
    }

    private IEnumerator CollisionCooldown()
    {
        isCollided = true;
        yield return new WaitForSeconds(3f); // Wait 3 seconds before boost can grow again
        isCollided = false;
    }
    private float Timecount = 0f;
    private float Timethro = 3f;
    private IEnumerator BoostProgressionCoroutine()
    {
        while (true)
        {
            yield return null; // run every frame

            if (!isCollided && boostThrust < maxBoostLimit && isThrottling)
            {
                boostThrust += 500f;
                Debug.Log("Boost Power Upgraded: " + boostThrust);

                isThrottling = false;
                Timecount = 0f;

                yield return new WaitForSeconds(5f); // cooldown between boosts
            }
        }
    }
    private bool isThrottling = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Timecount += Time.deltaTime;

            if (Timecount > Timethro)
            {
                isThrottling = true;
            }
            else
            {
                isThrottling = false;
            }
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleLimitedRotation();
        ApplyBoundaries();

    }
    private void ApplyBoundaries()
    {
        Vector3 pos = transform.localPosition;

        // Clamp X (Horizontal / Width)
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);

        // Clamp Y (Vertical / Height)
        pos.y = Mathf.Clamp(pos.y, -verticalLimit, verticalLimit);

        // Apply the restricted position back to the ship
        transform.localPosition = pos;

        // Optional: Kill velocity in the direction of the wall to prevent "bouncing" or jitter
        Vector3 vel = rb.linearVelocity;
        if (Mathf.Abs(pos.x) >= horizontalLimit) vel.x = 0;
        if (Mathf.Abs(pos.y) >= verticalLimit) vel.y = 0;
        rb.linearVelocity = vel;
    }
    private float FuelAmount = 1f;
    private void HandleMovement()
    {

        Vector3 totalForce = Vector3.forward * constantThrust;

        float vInput = Input.GetAxis("Vertical");

        if (vInput > 0)
        {
            totalForce += Vector3.forward * vInput * boostThrust;
            FuelAmount = 5;
        }

        //else if (vInput < 0) totalForce += Vector3.forward * vInput * (constantThrust * 0.8f);

        float hInput = Input.GetAxis("Horizontal");
        totalForce += Vector3.right * hInput * strafeForce;
        CL.FuelCosumption(FuelAmount);
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
  
}