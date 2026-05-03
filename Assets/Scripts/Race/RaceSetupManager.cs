using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RaceSetupManager : MonoBehaviour
{
    [Header("Optional Speedometer")]
    public SpeedometerUI p1Speedometer;

    [Header("Car Prefabs")]
    public GameObject[] player1CarPrefabs;

    [Header("Spawn Point")]
    public Transform player1Spawn;

    [Header("Player 1 References")]
    public RawImage p1PowerUpImage;
    public TMPro.TextMeshProUGUI p1ProgressText;
    public TMPro.TextMeshProUGUI p1LapText;
    public Image p1ParryCD;
    public Image p1HealthBar;
    public CinemachineCamera p1Camera;

    [Header("Shared")]
    public RaceCheckpoint[] sharedCheckpoints;
    public ParticleSystem explosion;

    [Header("AI Opponents")]
    public int aiCarCount = 3;
    public float aiRowSpacing = 5.5f;
    public float aiColumnSpacing = 3.0f;

    CamerasController cameraManager;
    public RaceStartCountdown gameStartCountdown;
    public PauseMenu pauseManager;

    public GameObject endGamePanel;
    public TextMeshProUGUI playerText;
    private int m_P1_LapCount;
    GameObject p1Car;
    private List<GameObject> m_aiCars = new List<GameObject>();

    public int maxLaps = 1;

    void Start()
    {

        cameraManager = GetComponent<CamerasController>();

        int p1Index = PlayerPrefs.GetInt("carIndex_P1", 0);

        Quaternion rotatedY = Quaternion.Euler(0, 90, 0);

        p1Car = Instantiate(player1CarPrefabs[p1Index], player1Spawn.position, rotatedY);

        cameraManager.Setup(1, p1Car, p1Car.GetComponent<PlayerInput>());

        p1Speedometer?.SetTarget(p1Car.GetComponent<Rigidbody>());

        VehicleSpawner p1Init = p1Car.GetComponent<VehicleSpawner>();
        if (p1Init != null)
        {
            p1Init.Initialize(
                p1PowerUpImage,
                p1Car.GetComponent<VehicleController>(),
                p1ProgressText,
                p1LapText,
                sharedCheckpoints,
                p1ParryCD,
                p1HealthBar,
                p1Camera,
                explosion
            );
        }

        SpawnAIOpponents(rotatedY, p1Index);

        if (pauseManager != null)
            pauseManager.SetPlayerInput(p1Car.GetComponent<PlayerInput>());

        var freezeList = new List<GameObject> { p1Car };
        freezeList.AddRange(m_aiCars);
        gameStartCountdown.SetPlayersToFreeze(freezeList.ToArray());
        gameStartCountdown.FreezePlayers();
    }

    private void SpawnAIOpponents(Quaternion rot, int playerCarIndex)
    {
        if (player1CarPrefabs == null || player1CarPrefabs.Length == 0) return;

        Vector3 origin = player1Spawn.position;
        Vector3 carForward = rot * Vector3.forward;
        Vector3 carRight = rot * Vector3.right;

        for (int i = 0; i < aiCarCount; i++)
        {

            int prefabIndex = (playerCarIndex + 1 + i) % player1CarPrefabs.Length;
            GameObject prefab = player1CarPrefabs[prefabIndex];
            if (prefab == null) continue;

            int row = i / 2 + 1;
            int colSign = (i % 2 == 0) ? -1 : 1;
            Vector3 offset = -carForward * (row * aiRowSpacing)
                            + carRight * (colSign * aiColumnSpacing);
            Vector3 pos = origin + offset;

            GameObject aiCar = Instantiate(prefab, pos, rot);
            aiCar.name = $"AI_Car_{i + 1}";

            DisablePlayerOnlyComponents(aiCar);

            var controller = aiCar.GetComponent<VehicleController>();
            if (controller != null) controller.useBotInput = true;

            if (aiCar.GetComponent<VehicleAutoPilot>() == null)
                aiCar.AddComponent<VehicleAutoPilot>();

            var aiInit = aiCar.GetComponent<VehicleSpawner>();
            if (aiInit != null)
            {

                aiInit.Initialize(
                    null,
                    controller,
                    null,
                    null,
                    sharedCheckpoints,
                    null,
                    null,
                    null,
                    explosion
                );
            }

            m_aiCars.Add(aiCar);
        }
    }

    private void DisablePlayerOnlyComponents(GameObject aiCar)
    {

        var pi = aiCar.GetComponent<PlayerInput>();
        if (pi != null) pi.enabled = false;

        var pjh = aiCar.GetComponent<PlayerJoinHandler>();
        if (pjh != null) pjh.enabled = false;

        var hud = aiCar.GetComponent<PlayerHudController>();
        if (hud != null) hud.enabled = false;
    }

    private bool m_gameEnded = false;

    private void Update()
    {
        if (m_gameEnded) return;
        if (p1Car == null) return;

        var playerLap = p1Car.GetComponent<LapTracker>();
        if (playerLap == null) return;

        m_P1_LapCount = playerLap.lapCount;

        if (m_P1_LapCount >= maxLaps)
        {
            EndGame("Игрок победил! Поздравляем");
            return;
        }

        for (int i = 0; i < m_aiCars.Count; i++)
        {
            var ai = m_aiCars[i];
            if (ai == null) continue;
            var aiLap = ai.GetComponent<LapTracker>();
            if (aiLap != null && aiLap.lapCount >= maxLaps)
            {
                EndGame($"ИИ-{i + 1} победил");
                return;
            }
        }
    }

    private void EndGame(string text)
    {
        m_gameEnded = true;
        if (playerText != null) playerText.text = text;
        if (endGamePanel != null) endGamePanel.SetActive(true);

        Time.timeScale = 0f;

        var pi = p1Car != null ? p1Car.GetComponent<PlayerInput>() : null;
        if (pi != null && pi.enabled) pi.DeactivateInput();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
