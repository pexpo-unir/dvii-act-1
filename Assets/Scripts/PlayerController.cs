using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 _inputDirection = Vector3.zero;
    private Vector3 _movementDirection = Vector3.zero;

    [SerializeField] private PlayerCharacter playerCharacter;

    [SerializeField] private Transform cameraTransform;
    
    [SerializeField] private CinemachineCamera aimingCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private bool _isAiming = false;
    
    void Start()
    {
        cinemachineCamera.Priority = 100;
        aimingCamera.Priority = 0;
    }

    void Update()
    {
        _inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _inputDirection.Normalize();
        _movementDirection = cameraTransform.forward * _inputDirection.z + cameraTransform.right * _inputDirection.x;
        _movementDirection.y = 0;

        playerCharacter.InputDirection = _movementDirection.normalized;

        if (Input.GetMouseButtonDown(0))
        {
            playerCharacter.Shoot();
        }

        if (Input.GetMouseButtonDown(1) && !_isAiming)
        {
            _isAiming = true;
            cinemachineCamera.Priority = 0;
            aimingCamera.Priority = 100;
            
            playerCharacter.StartAim();
        }
        
        if (Input.GetMouseButtonUp(1) && _isAiming)
        {
            _isAiming = false;
            cinemachineCamera.Priority = 100;
            aimingCamera.Priority = 0;
            
            playerCharacter.EndAim();
        }
    }
}