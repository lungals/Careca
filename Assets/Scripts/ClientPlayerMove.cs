using UnityEngine;

using Unity.Netcode;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField]
    CharacterController m_CharacterController;
    [SerializeField]
    ThirdPersonController m_ThirdPersonController;
    [SerializeField]
    PlayerInput m_PlayerInput;

    [SerializeField]
    Transform m_CameraFollow;



    private void Awake()
    {
        m_PlayerInput.enabled = false;
        m_ThirdPersonController.enabled = false;
        m_CharacterController.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient; // Enable if this is a client.
        if (!IsOwner)
        {
            // Disable if this is not the owner
            enabled = false;
            m_PlayerInput.enabled = false;
            m_CharacterController.enabled = false;
            m_ThirdPersonController.enabled = false;
            return;
        }

        // Enable if this is an owner
        m_PlayerInput.enabled = true;
        m_CharacterController.enabled = true;
        m_ThirdPersonController.enabled = true;

        //aqui que arruma a camera
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Follow = transform.Find("PlayerCameraRoot");

    }
}
