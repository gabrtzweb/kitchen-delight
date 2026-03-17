using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private const float FacingBlendSpeed = 14f;

    [SerializeField, Min(0.1f)] private float moveSpeed = 6f;
    [SerializeField, Min(1f)] private float acceleration = 30f;
    [SerializeField, Min(1f)] private float rotationSpeed = 8f;
    [SerializeField, Min(0.2f)] private float dashDistance = 2.8f;
    [SerializeField, Min(0.05f)] private float dashDuration = 0.2f;
    [SerializeField, Min(0f)] private float dashCooldown = 0.4f;
    [SerializeField, Range(0f, 1f)] private float playerPushResistance = 0.75f;
    [SerializeField, Min(0f)] private float maxPlayerPushSpeed = 2.4f;
    [SerializeField] private bool applyRecommendedRigidbodySettings = true;
    [SerializeField] private bool stopImmediatelyAfterDash = true;

    private Rigidbody rb;
    private Vector3 lastFacingDirection = Vector3.forward;
    private Vector3 dashDirection = Vector3.forward;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private bool wasDashingLastFixedStep;
    private float moveInputMagnitude;

    public float CurrentPlanarSpeed { get; private set; }
    public bool IsDashing => dashTimeRemaining > 0f;

    public void Configure(float configuredMoveSpeed, float configuredRotationSpeed)
    {
        moveSpeed = Mathf.Max(0.1f, configuredMoveSpeed);
        rotationSpeed = Mathf.Max(1f, configuredRotationSpeed);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        acceleration = Mathf.Max(1f, acceleration);
        rotationSpeed = Mathf.Max(1f, rotationSpeed);
        dashDistance = Mathf.Max(0.2f, dashDistance);
        dashDuration = Mathf.Max(0.05f, dashDuration);
        dashCooldown = Mathf.Max(0f, dashCooldown);
        playerPushResistance = Mathf.Clamp01(playerPushResistance);
        maxPlayerPushSpeed = Mathf.Max(0f, maxPlayerPushSpeed);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (applyRecommendedRigidbodySettings)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    public void Tick(PlayerInputReader input)
    {
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.deltaTime;
            if (dashCooldownRemaining < 0f)
            {
                dashCooldownRemaining = 0f;
            }
        }

        if (dashTimeRemaining > 0f)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining < 0f)
            {
                dashTimeRemaining = 0f;
            }
        }

        if (input.DashPressedThisFrame && dashCooldownRemaining <= 0f && !IsDashing)
        {
            StartDash(input.Move);
        }
    }

    public void FixedTick(PlayerInputReader input)
    {
        Vector2 move = input.Move;
        moveInputMagnitude = move.magnitude;
        Vector3 desiredDirection = new Vector3(move.x, 0f, move.y);

        if (IsDashing)
        {
            lastFacingDirection = dashDirection;
            float dashSpeed = dashDuration > 0f ? dashDistance / dashDuration : 0f;
            Vector3 dashVelocity = dashDirection * dashSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);
            CurrentPlanarSpeed = dashVelocity.magnitude;
            wasDashingLastFixedStep = true;
            RotateTowardFacing();
            return;
        }

        if (wasDashingLastFixedStep)
        {
            wasDashingLastFixedStep = false;
            if (stopImmediatelyAfterDash)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        if (desiredDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 desiredFacing = desiredDirection.normalized;
            if (lastFacingDirection.sqrMagnitude < 0.0001f)
            {
                lastFacingDirection = desiredFacing;
            }
            else
            {
                float blendT = Mathf.Clamp01(FacingBlendSpeed * Time.fixedDeltaTime);
                lastFacingDirection = Vector3.Slerp(lastFacingDirection, desiredFacing, blendT).normalized;
            }
        }

        float targetSpeed = moveSpeed;
        Vector3 targetPlanarVelocity = desiredDirection * targetSpeed;
        Vector3 currentPlanarVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 delta = targetPlanarVelocity - currentPlanarVelocity;

        float maxDelta = acceleration * Time.fixedDeltaTime;
        if (delta.magnitude > maxDelta)
        {
            delta = delta.normalized * maxDelta;
        }

        Vector3 newPlanarVelocity = currentPlanarVelocity + delta;
        rb.linearVelocity = new Vector3(newPlanarVelocity.x, rb.linearVelocity.y, newPlanarVelocity.z);
        CurrentPlanarSpeed = newPlanarVelocity.magnitude;

        RotateTowardFacing();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyPlayerPushResistance(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        ApplyPlayerPushResistance(collision);
    }

    private void ApplyPlayerPushResistance(Collision collision)
    {
        if (IsDashing)
        {
            return;
        }

        if (moveInputMagnitude > 0.15f)
        {
            return;
        }

        if (collision.rigidbody == null || collision.rigidbody == rb)
        {
            return;
        }

        if (!collision.rigidbody.TryGetComponent<PlayerMovement>(out _))
        {
            return;
        }

        Vector3 planarVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 resistedVelocity = Vector3.Lerp(planarVelocity, Vector3.zero, playerPushResistance);
        if (maxPlayerPushSpeed > 0f)
        {
            resistedVelocity = Vector3.ClampMagnitude(resistedVelocity, maxPlayerPushSpeed);
        }

        rb.linearVelocity = new Vector3(resistedVelocity.x, rb.linearVelocity.y, resistedVelocity.z);
        CurrentPlanarSpeed = resistedVelocity.magnitude;
    }

    private void StartDash(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        }
        else
        {
            dashDirection = lastFacingDirection.sqrMagnitude > 0.0001f ? lastFacingDirection.normalized : transform.forward;
            dashDirection.y = 0f;
            if (dashDirection.sqrMagnitude < 0.0001f)
            {
                dashDirection = Vector3.forward;
            }
            else
            {
                dashDirection.Normalize();
            }
        }

        lastFacingDirection = dashDirection;
        dashTimeRemaining = dashDuration;
        dashCooldownRemaining = dashCooldown;
    }

    private void RotateTowardFacing()
    {
        if (lastFacingDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lastFacingDirection, Vector3.up);
        Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedRotation);
    }
}
