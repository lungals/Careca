using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractWithObjects : NetworkBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;

    private InputSystem_Actions inputSystemActions;
    private InputAction interact;

    private PlayerId playerId;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        interact = inputSystemActions.Player.Interact;
    }

    private void OnEnable()
    {
        interact.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    private void Update()
    {
        if (!IsOwner)
            return;


        if (interact.WasPressedThisFrame())
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);

            foreach (Collider collider in colliders)
            {
                IInteractableObject obj = collider.GetComponent<IInteractableObject>();

                if (obj == null)
                    continue;

                obj.Interact(playerId);
            }
        }
    }

    public void SetPlayer(PlayerId playerId)
    {
        this.playerId = playerId;
    }
}
