using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class GerenciadorPartida : NetworkBehaviour
{
    public static GerenciadorPartida Instance { get; private set; }

    [Header("Configurações")]
    [SerializeField] private NetworkObject moedaPrefab;
    [SerializeField] private float tempoMaximoPartida = 45f;
    [SerializeField] private float intervaloSpawnMoeda = 2.5f;

    [Header("Limites da Pista para Moedas")]
    [SerializeField] private float limiteMinX = -6f;
    [SerializeField] private float limiteMaxX = 6f;
    [SerializeField] private float limiteMinZ = -20f;
    [SerializeField] private float limiteMaxZ = 20f;

    [Networked] public TickTimer TempoRestante { get; set; }
    [Networked] public NetworkBool JogoFinalizado { get; set; }
    [Networked] public NetworkBool PartidaIniciada { get; set; }

    private TickTimer timerSpawnMoeda;

    public override void Spawned()
    {
        Instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        if (JogoFinalizado) return;

        if (!PartidaIniciada)
        {
            if (HasStateAuthority)
            {
                int contagemReal = 0;
                foreach (var playerRef in Runner.ActivePlayers)
                {
                    contagemReal++;
                }

                if (contagemReal >= 2)
                {
                    PartidaIniciada = true;
                    TempoRestante = TickTimer.CreateFromSeconds(Runner, tempoMaximoPartida);
                    timerSpawnMoeda = TickTimer.CreateFromSeconds(Runner, intervaloSpawnMoeda);
                }
            }
            return;
        }



        if (TempoRestante.Expired(Runner))
        {
            if (HasStateAuthority)
            {
                JogoFinalizado = true;
                FinalizarPartida();
            }
            return;
        }

        if (HasStateAuthority)
        {
            if (timerSpawnMoeda.Expired(Runner))
            {
                SpawnMoedaAleatoria();
                timerSpawnMoeda = TickTimer.CreateFromSeconds(Runner, intervaloSpawnMoeda);
            }
        }
    }

    private void SpawnMoedaAleatoria()
    {
        float randomX = Random.Range(limiteMinX, limiteMaxX);
        float randomZ = Random.Range(limiteMinZ, limiteMaxZ);
        Vector3 posicaoSpawn = new Vector3(randomX, 1f, randomZ);

        Runner.Spawn(moedaPrefab, posicaoSpawn, Quaternion.identity);
    }

    private void FinalizarPartida()
    {
        var todosObjetos = new List<NetworkObject>(Runner.GetAllNetworkObjects());
        foreach (var obj in todosObjetos)
        {
            if (obj != null && obj.CompareTag("Moeda"))
            {
                Runner.Despawn(obj);
            }
        }
    }
}
