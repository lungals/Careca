using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InteractWithObjects interactWithObjects;

    private PlayerId playerId;

    private void Awake()
    {
        PlayerRegister.Register(this, out PlayerId myPlayerId);
        playerId = myPlayerId;

        interactWithObjects.SetPlayer(playerId);
    }

    private void OnDestroy()
    {
        PlayerRegister.Unregister(playerId);
    }
}

