using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public partial class VehicleController : MonoBehaviour
{
    [Header("References")]
    [FormerlySerializedAs("carRB")]
    [SerializeField] private Rigidbody m_carRB;
    [FormerlySerializedAs("rayPoints")]
    [SerializeField] private Transform[] m_rayPoints;
    [FormerlySerializedAs("drivable")]
    [SerializeField] private LayerMask m_drivable;
    [FormerlySerializedAs("accelerationPoint")]
    [SerializeField] private Transform m_accelerationPoint;
    [FormerlySerializedAs("frontTireParent")]
    [SerializeField] private GameObject[] m_frontTireParent = new GameObject[2];

    [Header("Suspension Settings")]
    [FormerlySerializedAs("springStiffness")]
    [SerializeField] private float m_springStiffness;
    [FormerlySerializedAs("damperStiffness")]
    [SerializeField] private float m_damperStiffness;
    [FormerlySerializedAs("restLength")]
    [SerializeField] private float m_restLength;
    [FormerlySerializedAs("springTravel")]
    [SerializeField] private float m_springTravel;
    [FormerlySerializedAs("wheelRadius")]
    [SerializeField] private float m_wheelRadius;
    [FormerlySerializedAs("dampingFactor")]
    [SerializeField] private float m_dampingFactor;
    [FormerlySerializedAs("skidMarks")]
    [SerializeField] private TrailRenderer[] m_skidMarks = new TrailRenderer[2];
    [FormerlySerializedAs("skidSmokes")]
    [SerializeField] private ParticleSystem[] m_skidSmokes = new ParticleSystem[2];
    [FormerlySerializedAs("engineSound")]
    [SerializeField] private AudioSource m_engineSound;
    [FormerlySerializedAs("skidSound")]
    [SerializeField] private AudioSource m_skidSound;

    private PlayerInput m_playerInput;
    private Vector2 m_moveVector;

    [HideInInspector] public bool useBotInput = false;
    [HideInInspector] public Vector2 botInput = Vector2.zero;
    [HideInInspector] public bool botDrifting = false;

    private float m_moveInput;
    private float m_steerInput;
    private bool hasPowerUp = false;
    private bool m_isDrifting = false;
    private Vector3 m_currentCarVelocity = Vector3.zero;
    private float m_carVelocityRatio = 0;
    private int[] m_wheelsIsGrounded = new int[4];
    private bool m_isGrounded = false;

    [Header("Car Settings")]
    [FormerlySerializedAs("acceleration")]
    [SerializeField] private float m_acceleration = 25f;
    [FormerlySerializedAs("maxSpeed")]
    [SerializeField] private float m_maxSpeed = 100f;
    [FormerlySerializedAs("deceleration")]
    [SerializeField] private float m_deceleration = 10f;
    [FormerlySerializedAs("turnSpeed")]
    [SerializeField] private float m_turnSpeed = 15f;
    [FormerlySerializedAs("customPower")]
    [SerializeField] private float m_customPower = 0.2f;
    [FormerlySerializedAs("turningCurve")]
    [SerializeField] private AnimationCurve m_turningCurve;
    [FormerlySerializedAs("dragCoefficient")]
    [SerializeField] private float m_dragCoefficient = 1f;

    [FormerlySerializedAs("rearGrip")]
    [SerializeField] private float m_rearGrip = 1.0f;
    [FormerlySerializedAs("rearGripWhenDrifting")]
    [SerializeField] private float m_rearGripWhenDrifting = 0.6f;

    [Header("Visuals")]
    [FormerlySerializedAs("maxSteeringAngle")]
    [SerializeField] private float m_maxSteeringAngle = 30f;
    [FormerlySerializedAs("minSideSkidVelocity")]
    [SerializeField] private float m_minSideSkidVelocity = 10f;
    [FormerlySerializedAs("skidDelay")]
    [SerializeField] private float m_skidDelay = 0.2f;

    [Header("Audio")]
    [FormerlySerializedAs("minPitch")]
    [SerializeField][Range(0, 1)] private float m_minPitch = 1f;
    [FormerlySerializedAs("maxPitch")]
    [SerializeField][Range(1, 5)] private float m_maxPitch = 5f;

    [SerializeField] private CinemachineCamera playerCamera;

    private bool m_skidActive = false;
    private float m_skidTimer = 0f;

    public bool isAbleToOneShot = false;

    public void SetPlayerCamera(CinemachineCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }

    public CinemachineCamera GetPlayerCamera() => playerCamera;

    public bool HasActivePowerUp()
    {
        return hasPowerUp;
    }

    public void SetHasActivePowerUp(bool hasPowerUp)
    {
        this.hasPowerUp = hasPowerUp;
    }

    public void SetAcceleration(float newAcceleration)
    {
        this.m_acceleration = newAcceleration;
    }

    public float GetAcceleration()
    {
        return m_acceleration;
    }

    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        m_engineSound = sources[0];
        m_skidSound = sources[1];
    }

    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        if (m_playerInput != null && m_playerInput.actions != null)
        {
            m_playerInput.actions["Drive"].performed += ctx => m_moveVector = ctx.ReadValue<Vector2>();
            m_playerInput.actions["Drive"].canceled += ctx => m_moveVector = Vector2.zero;

            m_playerInput.actions["HandBrake"].started += ctx => m_isDrifting = true;
            m_playerInput.actions["HandBrake"].canceled += ctx => m_isDrifting = false;
        }

        m_carRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        TireVisuals();
        Vfx();
        SidewaysDrag();
        YawStabilizer();
        EngineSound();
    }

    private void GetInput()
    {
        if (useBotInput)
        {
            m_moveInput = botInput.y;
            m_steerInput = botInput.x;
            m_isDrifting = botDrifting;
            return;
        }
        m_moveInput = m_moveVector.y;
        m_steerInput = m_moveVector.x;
    }
}
