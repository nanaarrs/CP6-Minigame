using UnityEngine;
using Fusion;
using TMPro;

public class HUDCorrida : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private TMP_Text textoPontos;
    [SerializeField] private TMP_Text textoTempo;
    [SerializeField] private GameObject painelFimDeJogo;
    [SerializeField] private TMP_Text textoResultadoFinal;

    void Start()
    {
        painelFimDeJogo.SetActive(false);
    }

    void Update()
    {
        if (runner == null || !runner.IsRunning || GerenciadorPartida.Instance == null) return;

        if (!GerenciadorPartida.Instance.PartidaIniciada)
        {
            textoTempo.text = "Aguardando Jogador 2...";
            textoPontos.text = "Meus Pontos: 0";
            return;
        }

        float tempoRestante = GerenciadorPartida.Instance.TempoRestante.RemainingTime(runner) ?? 0f;
        textoTempo.text = $"Tempo: {Mathf.CeilToInt(tempoRestante)}s";

        if (runner.TryGetPlayerObject(runner.LocalPlayer, out NetworkObject playerObj))
        {
            CorridaPlayer playerLocal = playerObj.GetComponent<CorridaPlayer>();
            if (playerLocal != null)
            {
                textoPontos.text = $"Meus Pontos: {playerLocal.Pontos}";
            }
        }

        if (GerenciadorPartida.Instance.JogoFinalizado)
        {
            ExibirResultadoFinal();
        }
    }


    private void ExibirResultadoFinal()
    {
        painelFimDeJogo.SetActive(true);
        textoTempo.text = "Fim de Jogo!";

        CorridaPlayer jogadorLocal = null;
        CorridaPlayer jogadorAdversario = null;

        foreach (PlayerRef playerRef in runner.ActivePlayers)
        {
            if (runner.TryGetPlayerObject(playerRef, out NetworkObject pObj))
            {
                CorridaPlayer cp = pObj.GetComponent<CorridaPlayer>();
                if (playerRef == runner.LocalPlayer) jogadorLocal = cp;
                else jogadorAdversario = cp;
            }
        }

        if (jogadorLocal != null)
        {
            int meusPontos = jogadorLocal.Pontos;
            int pontosAdversario = (jogadorAdversario != null) ? jogadorAdversario.Pontos : 0;

            if (meusPontos > pontosAdversario) {
                textoResultadoFinal.text = $"VOCÊ VENCEU!\nSeus Pontos: {meusPontos}\nAdversário: {pontosAdversario}";
                textoResultadoFinal.color = Color.green;
            }
            else if (meusPontos < pontosAdversario) {
                textoResultadoFinal.text = $"VOCÊ PERDEU!\nSeus Pontos: {meusPontos}\nAdversário: {pontosAdversario}";
                textoResultadoFinal.color = Color.red;
            }
            else {
                textoResultadoFinal.text = $"EMPATE!\nAmbos fizeram {meusPontos} pontos.";
                textoResultadoFinal.color = Color.yellow;
            }
        }
    }
}

