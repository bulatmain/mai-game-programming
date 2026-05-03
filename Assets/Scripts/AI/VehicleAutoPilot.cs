using UnityEngine;

public class VehicleAutoPilot : MonoBehaviour
{
    [Header("Driving")]
    public float arriveDistance = 8f;
    public float steerGain = 2.5f;
    public float minThrottle = 0.5f;
    public float driftAngleThreshold = 55f;

    [Header("Combat")]
    public float parryRadius = 6f;
    public float parryChance = 0.6f;
    public float powerUpCooldown = 4f;

    [Header("Recovery")]
    public float stuckSpeed = 1.5f;
    public float stuckTime = 2.5f;
    public float flipAfterSeconds = 2.0f;
    public float flipUpDotThreshold = 0.3f;
    public float flipLift = 1.5f;

    private VehicleController m_car;
    private LapTracker m_lap;
    private ParryHandler m_parry;
    private PlayerPowerUpHolder m_powerUps;
    private Rigidbody m_rb;

    private float m_nextPowerUpAt;
    private float m_stuckTimer;
    private float m_reverseUntil;
    private float m_flipTimer;

    public void Bind()
    {
        m_car = GetComponent<VehicleController>();
        m_lap = GetComponent<LapTracker>();
        m_parry = GetComponent<ParryHandler>();
        m_powerUps = GetComponent<PlayerPowerUpHolder>();
        m_rb = GetComponent<Rigidbody>();
        if (m_car != null) m_car.useBotInput = true;
    }

    private void Awake() { Bind(); }
    private void OnEnable() { if (m_car != null) m_car.useBotInput = true; }

    private void Update()
    {
        if (m_car == null) Bind();
        if (m_car == null || m_lap == null || m_rb == null) return;

        Vector3? targetOpt = m_lap.GetNextCheckpointPosition();
        Vector3 target = targetOpt ?? (transform.position + transform.forward * 20f);

        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;
        Vector3 fwd = transform.forward; fwd.y = 0f; fwd.Normalize();

        float signedAngle = Vector3.SignedAngle(fwd, toTarget.normalized, Vector3.up);
        float steer = Mathf.Clamp(signedAngle / 30f * steerGain, -1f, 1f);

        float align = Vector3.Dot(fwd, toTarget.normalized);
        float throttle = Mathf.Lerp(minThrottle, 1f, Mathf.Clamp01(align));

        float speed = m_rb.linearVelocity.magnitude;

        if (speed < stuckSpeed) m_stuckTimer += Time.deltaTime;
        else m_stuckTimer = 0f;
        if (m_stuckTimer > stuckTime)
        {
            m_reverseUntil = Time.time + 1.2f;
            m_stuckTimer = 0f;
        }
        if (Time.time < m_reverseUntil)
        {
            throttle = -1f;
            steer = -steer;
        }

        m_car.botInput = new Vector2(steer, throttle);
        m_car.botDrifting = Mathf.Abs(signedAngle) > driftAngleThreshold && speed > 8f;

        HandleAutoFlip();

        if (m_parry != null && m_parry.GetParryCooldownNormalized() <= 0f)
        {
            Collider[] near = Physics.OverlapSphere(transform.position, parryRadius);
            foreach (var c in near)
            {
                if (c.gameObject == gameObject) continue;
                if (!c.CompareTag("Player") && !c.CompareTag("Obstacle")) continue;
                if (Random.value < parryChance * Time.deltaTime * 5f)
                {
                    m_parry.TryParry();
                    break;
                }
            }
        }

        if (m_powerUps != null && m_powerUps.storedPowerUp != null && Time.time >= m_nextPowerUpAt)
        {
            m_powerUps.TryUsePowerUp();
            m_nextPowerUpAt = Time.time + powerUpCooldown;
        }
    }

    private void HandleAutoFlip()
    {
        bool upsideDown = Vector3.Dot(transform.up, Vector3.up) < flipUpDotThreshold;
        if (upsideDown) m_flipTimer += Time.deltaTime;
        else m_flipTimer = 0f;

        if (m_flipTimer > flipAfterSeconds)
        {
            AutoFlip();
            m_flipTimer = 0f;
        }
    }

    private void AutoFlip()
    {
        Vector3 facing = transform.forward;
        facing.y = 0f;
        if (facing.sqrMagnitude < 0.01f)
        {
            Vector3? cp = m_lap != null ? m_lap.GetNextCheckpointPosition() : null;
            facing = cp.HasValue ? (cp.Value - transform.position) : Vector3.forward;
            facing.y = 0f;
        }
        if (facing.sqrMagnitude < 0.01f) facing = Vector3.forward;
        facing.Normalize();

        Vector3 newPos = transform.position + Vector3.up * flipLift;
        transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(facing, Vector3.up));

        if (m_rb != null)
        {
            m_rb.linearVelocity = Vector3.zero;
            m_rb.angularVelocity = Vector3.zero;
        }
    }
}
