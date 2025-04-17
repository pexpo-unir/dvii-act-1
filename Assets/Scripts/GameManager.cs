using DG.Tweening;
using UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Start Menu")] [SerializeField]
    private CinemachineCamera menuCamera;

    [SerializeField] private Button startGameButton;

    [SerializeField] private Canvas menuCanvas;

    [Header("Start Menu | Backdrop Start")] [SerializeField]
    private Image backdropStart;

    [SerializeField] private Color backdropStartColor = new(0f, 0f, 0f, 1f);

    [SerializeField] private float fadeOutDuration = 5.0f;

    [SerializeField] private GameObject menuBear;

    [Header("Gameplay")] [SerializeField] private PlayerController playerController;

    [SerializeField] private HUD hud;

    [SerializeField] private BearSpawner bearSpawner;

    [SerializeField] private PostprocessManager ppManager;


    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        startGameButton.onClick.AddListener(StartLevel);
        backdropStart.color = backdropStartColor;
        playerController.enabled = false;
        hud.gameObject.SetActive(false);

        var seq = DOTween.Sequence();
        seq.Append(backdropStart.DOFade(0, fadeOutDuration));
    }

    private void StartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        menuCanvas.gameObject.SetActive(false);
        menuBear.gameObject.SetActive(false);

        ppManager.StartGameplay();

        playerController.enabled = true;
        hud.gameObject.SetActive(true);

        menuCamera.Priority.Value = 0;

        bearSpawner.StartSpawn();
    }

    public void UpdateChromaticAberration(float intensity)
    {
        ppManager.UpdateChromaticAberration(intensity);
    }
}