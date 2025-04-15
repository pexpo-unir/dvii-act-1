using DG.Tweening;
using UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera menuCamera;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Image backdropStart;
    [SerializeField] private GameObject menuBear;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private HUD hud;
    [SerializeField] private BearSpawner bearSpawner;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartLevel);
        backdropStart.color = new Color(0f, 0f, 0f, 1f);
        playerController.enabled = false;
        hud.gameObject.SetActive(false);

        var seq = DOTween.Sequence();

        seq.Append(backdropStart.DOFade(0, 5f));
    }

    private void StartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        menuCanvas.gameObject.SetActive(false);
        menuBear.gameObject.SetActive(false);

        playerController.enabled = true;
        hud.gameObject.SetActive(true);

        menuCamera.Priority.Value = 0;

        bearSpawner.StartSpawn();
    }
}