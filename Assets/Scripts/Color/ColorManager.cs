using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // Singleton simples.
    // Permite que outros scripts acessem o ColorManager usando:
    // ColorManager.Instance
    public static ColorManager Instance;

    // Lista que guarda os índices das cores principais ainda disponíveis.
    //
    // No início teremos:
    // 0, 1, 2, 3, 4, 5, 6, 7, 8 e 9.
    //
    // Quando uma cor é atribuída a um jogador, seu índice é removido.
    // Quando o jogador sai, o índice é devolvido para esta lista.
    private List<int> availableColors = new List<int>();


    private void Awake()
    {
        // Guarda uma referência para este objeto.
        // Assim, os Players podem chamar métodos do ColorManager
        // sem precisar procurar o objeto na cena.
        Instance = this;

        // Preenche a lista com os 10 índices de cor.
        //
        // É importante observar que o ColorManager NÃO guarda objetos Color.
        // Ele trabalha somente com índices.
        //
        // Isso é suficiente porque todos os jogadores possuem a mesma
        // tabela de cores no script PlayerColors.
        for (int i = 0; i < 10; i++)
        {
            availableColors.Add(i);
        }
    }


    public int GetAvailableColor()
    {
        // Se não houver nenhuma cor disponível, devolvemos -1.
        //
        // Isso aconteceria, por exemplo, se já existirem 10 jogadores
        // conectados e um 11º jogador tentar entrar.
        if (availableColors.Count == 0)
        {
            return -1;
        }

        // Escolhe aleatoriamente uma POSIÇÃO dentro da lista.
        //
        // Exemplo:
        // availableColors = [0, 2, 5, 7]
        //
        // Random.Range pode escolher posição 2.
        // Nessa posição está armazenado o índice de cor 5.
        int position = Random.Range(0, availableColors.Count);

        // Guarda o índice da cor escolhida.
        int colorIndex = availableColors[position];

        // Remove a cor da lista.
        //
        // Desta forma, nenhum outro jogador poderá receber essa mesma
        // cor enquanto ela estiver sendo utilizada.
        availableColors.RemoveAt(position);

        // Devolve o índice da cor escolhida.
        return colorIndex;
    }


    public void ReturnColor(int colorIndex)
    {
        // Quando um jogador sai, sua cor deve voltar para a lista.
        //
        // Antes de adicionar, verificamos se ela já está na lista.
        // Isso evita inserir o mesmo índice duas vezes por engano.
        if (!availableColors.Contains(colorIndex))
        {
            availableColors.Add(colorIndex);
        }
    }
}
