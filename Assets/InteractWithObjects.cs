using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractWithObjects : NetworkBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;

    InputSystem_Actions inputSystemActions;
    InputAction interact;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
    void Update()
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

                obj.Interact();
            }
        }
    }
}
