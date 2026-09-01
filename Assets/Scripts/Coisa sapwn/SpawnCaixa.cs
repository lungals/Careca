using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnCaixa : NetworkBehaviour
{
    [SerializeField]
    GameObject prefabCaixa;

    InputSystem_Actions inputSystemActions;
    InputAction interact;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        interact = inputSystemActions.Player.SpawnBox;
    }

    private void OnEnable()
    {
        interact.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        if(interact.WasPressedThisFrame())
        {
            InserirCaixaCenaServerRPC();
        }
    }


    [Rpc(SendTo.Server)]

    public void InserirCaixaCenaServerRPC()
    {
        GameObject caixaJenniffer = Instantiate(prefabCaixa, transform.position + transform.forward * 3, transform.rotation);

        NetworkObject instanciaJenniffer=caixaJenniffer.GetComponent<NetworkObject>();
        instanciaJenniffer.Spawn();
        Destroy(caixaJenniffer, 5f);
    }
}
