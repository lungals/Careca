using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnCaixa : NetworkBehaviour
{
    [SerializeField]
    GameObject prefabCaixa;

    InputSystem_Actions inputSystemActions;
    InputAction gerarCaixa;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        gerarCaixa = inputSystemActions.Player.Interact;
    }

    private void OnEnable()
    {
        gerarCaixa.Enable();
    }

    private void OnDisable()
    {
        gerarCaixa.Disable();
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

        if(gerarCaixa.WasPressedThisFrame())
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
