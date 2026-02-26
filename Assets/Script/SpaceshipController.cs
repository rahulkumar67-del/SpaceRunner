using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsSpaceship : MonoBehaviour
{
    private Rigidbody rb;

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
    public float speedThreshold = 1000f;
    private bool hasReachedTopSpeed = false;
    private bool isCollided = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1f;
        rb.maxAngularVelocity = maxRotationSpeed;
        rb.angularDamping = 2f;

        initialBoostThrust = boostThrust;

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
        float currentSpeed = rb.linearVelocity.magnitude;
        //Debug.Log("Current Speed: " + currentSpeed);
        if (currentSpeed >= speedThreshold && !hasReachedTopSpeed)
        {
            hasReachedTopSpeed = true;
            Debug.Log("Top Speed Achieved!");
        }
      
        HandleMovement();
        HandleLimitedRotation();
        
    }

    private void HandleMovement()
    {
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


    float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }
  
}