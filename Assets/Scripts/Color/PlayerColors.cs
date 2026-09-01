using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColors : NetworkBehaviour
{
    [Header("Skinned Mesh Renderer")]

    // SkinnedMeshRenderer do personagem.
    // Ele contém a lista de materiais utilizada pelo modelo.
    [SerializeField]
    private SkinnedMeshRenderer playerRenderer;


    [Header("Índices dos materiais")]

    // Índice do material que receberá a cor principal.
    //
    // Exemplo:
    // Element 0 -> corpo
    // Element 1 -> braços
    // Element 2 -> pernas
    //
    // Se a cor principal deve ser aplicada no Element 0,
    // deixe este valor como 0.
    [SerializeField]
    private int mainMaterialIndex = 0;


    // Índice do material que receberá a cor secundária.
    //
    // Se a cor secundária deve ser aplicada no Element 1,
    // deixe este valor como 1.
    [SerializeField]
    private int secondaryMaterialIndex = 1;


    // -------------------------------------------------------------
    // NETWORK VARIABLE DA COR PRINCIPAL
    // -------------------------------------------------------------

    // Guarda o índice da cor principal.
    //
    // Por padrão:
    // - todos podem ler;
    // - somente o servidor pode escrever.
    //
    // Isso é exatamente o que queremos para a cor principal.
    private NetworkVariable<int> mainColorIndex =
        new NetworkVariable<int>(0);


    // -------------------------------------------------------------
    // NETWORK VARIABLE DA COR SECUNDÁRIA
    // -------------------------------------------------------------

    // Guarda o índice da cor secundária.
    //
    // Todos podem ler o valor.
    //
    // Somente o Owner do Player pode alterar.
    private NetworkVariable<int> secondaryColorIndex =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );


    // -------------------------------------------------------------
    // TABELA DE CORES
    // -------------------------------------------------------------

    // Todos os clientes possuem exatamente a mesma tabela.
    //
    // Pela rede será enviado apenas o índice da cor,
    // e não o objeto Color completo.
    private Color[] colors =
    {
        Color.red,                      // 0
        Color.blue,                     // 1
        Color.green,                    // 2
        Color.yellow,                   // 3
        Color.magenta,                  // 4
        Color.cyan,                     // 5
        new Color(1f, 0.5f, 0f),        // 6 - laranja
        new Color(0.5f, 0f, 1f),        // 7 - violeta
        new Color(0f, 0.5f, 1f),        // 8 - azul claro
        Color.white                     // 9
    };


    // Array que armazenará as instâncias dos materiais
    // utilizadas exclusivamente por este Player.
    private Material[] playerMaterials;

    InputSystem_Actions inputSystemActions;
    InputAction playercolor;

    private void Awake()
    {
        inputSystemActions=new InputSystem_Actions();
        playercolor = inputSystemActions.Player.Color;
    } 
    private void OnEnable()
    {
        playercolor.Enable();
    }
    private void OnDisable()
    {
        playercolor.Disable();
    }
    public override void OnNetworkSpawn()
    {
        // ---------------------------------------------------------
        // CRIAÇÃO DAS INSTÂNCIAS DOS MATERIAIS
        // ---------------------------------------------------------

        // playerRenderer.materials devolve instâncias dos materiais.
        //
        // Isso é importante porque não queremos alterar o material
        // original compartilhado por todos os Players.
        //
        // Cada Player passa a ter suas próprias instâncias.
        playerMaterials = playerRenderer.materials;


        // ---------------------------------------------------------
        // REGISTRO DOS EVENTOS
        // ---------------------------------------------------------

        // Quando a cor principal mudar pela rede,
        // OnMainColorChanged será executado.
        mainColorIndex.OnValueChanged += OnMainColorChanged;

        // Quando a cor secundária mudar pela rede,
        // OnSecondaryColorChanged será executado.
        secondaryColorIndex.OnValueChanged += OnSecondaryColorChanged;


        // ---------------------------------------------------------
        // APLICAÇÃO DOS VALORES ATUAIS
        // ---------------------------------------------------------

        // Aplica imediatamente os valores atuais das NetworkVariables.
        //
        // Isso também garante que um cliente que entre depois
        // visualize corretamente as cores já existentes.
        ApplyMainColor(mainColorIndex.Value);

        ApplySecondaryColor(secondaryColorIndex.Value);


        // ---------------------------------------------------------
        // ESCOLHA DA COR PRINCIPAL
        // ---------------------------------------------------------

        // Somente o servidor escolhe a cor principal.
        if (IsServer)
        {
            // Solicita uma cor ainda disponível ao ColorManager.
            int color = ColorManager.Instance.GetAvailableColor();

            // -1 significa que não existem mais cores disponíveis.
            if (color != -1)
            {
                // O servidor altera a NetworkVariable.
                //
                // O novo valor será então enviado para todos os clientes.
                mainColorIndex.Value = color;
            }
            else
            {
                Debug.LogWarning(
                    "Não existem mais cores principais disponíveis."
                );
            }
        }
    }


    public override void OnNetworkDespawn()
    {
        // Remove os eventos registrados anteriormente.
        mainColorIndex.OnValueChanged -= OnMainColorChanged;

        secondaryColorIndex.OnValueChanged -= OnSecondaryColorChanged;


        // Somente o servidor administra as cores disponíveis.
        //
        // Quando o Player sai, sua cor principal é devolvida.
        if (IsServer)
        {
            ColorManager.Instance.ReturnColor(
                mainColorIndex.Value
            );
        }
    }


    private void Update()
    {
        Debug.Log("update rolando");

        // Somente o dono deste Player deve responder
        // aos comandos locais de teclado.
        if (!IsOwner)
        {
            return;
        }


        // Ao pressionar C, troca a cor secundária.
        if(playercolor.WasPressedThisFrame())
        {
            Debug.Log("Deveria trocar a cor");
            ChangeSecondaryColor();
        }
    }


    private void ChangeSecondaryColor()
    {
        // Avança para a próxima cor.
        int nextColor =
            secondaryColorIndex.Value + 1;


        // Se chegarmos depois da última cor,
        // voltamos para o índice 0.
        if (nextColor >= colors.Length)
        {
            nextColor = 0;
        }


        // Como esta NetworkVariable possui
        // NetworkVariableWritePermission.Owner,
        // o jogador proprietário pode alterá-la diretamente.
        secondaryColorIndex.Value = nextColor;
    }


    // -------------------------------------------------------------
    // EVENTOS DAS NETWORK VARIABLES
    // -------------------------------------------------------------

    private void OnMainColorChanged(
        int previousValue,
        int newValue)
    {
        // Executado quando o índice da cor principal muda.
        ApplyMainColor(newValue);
    }


    private void OnSecondaryColorChanged(
        int previousValue,
        int newValue)
    {
        // Executado quando o índice da cor secundária muda.
        ApplySecondaryColor(newValue);
    }


    // -------------------------------------------------------------
    // APLICAÇÃO DA COR PRINCIPAL
    // -------------------------------------------------------------

    private void ApplyMainColor(int colorIndex)
    {
        // Verifica se o índice da cor é válido.
        if (colorIndex < 0 ||
            colorIndex >= colors.Length)
        {
            return;
        }


        // Verifica se os materiais foram carregados corretamente.
        if (playerMaterials == null)
        {
            return;
        }


        // Verifica se o índice do material principal existe.
        if (mainMaterialIndex < 0 ||
            mainMaterialIndex >= playerMaterials.Length)
        {
            Debug.LogWarning(
                "Índice do material principal inválido."
            );

            return;
        }


        // Obtém o material correspondente ao índice.
        Material material =
            playerMaterials[mainMaterialIndex];


        // Altera a propriedade Base Color do material.
        //
        // "_BaseColor" é utilizada normalmente pelos shaders URP.
        material.SetColor(
            "_BaseColor",
            colors[colorIndex]
        );
    }


    // -------------------------------------------------------------
    // APLICAÇÃO DA COR SECUNDÁRIA
    // -------------------------------------------------------------

    private void ApplySecondaryColor(int colorIndex)
    {
        // Verifica se o índice da cor é válido.
        if (colorIndex < 0 ||
            colorIndex >= colors.Length)
        {
            return;
        }


        // Verifica se os materiais estão disponíveis.
        if (playerMaterials == null)
        {
            return;
        }


        // Verifica se o índice do material secundário existe.
        if (secondaryMaterialIndex < 0 ||
            secondaryMaterialIndex >= playerMaterials.Length)
        {
            Debug.LogWarning(
                "Índice do material secundário inválido."
            );

            return;
        }


        // Obtém o material correspondente.
        Material material =
            playerMaterials[secondaryMaterialIndex];


        // Altera o Base Color do material secundário.
        material.SetColor(
            "_BaseColor",
            colors[colorIndex]
        );
    }
}
