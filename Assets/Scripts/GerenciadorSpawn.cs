using UnityEngine;
using Fusion;

public class GerenciadorSpawn : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private NetworkObject gameManagerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;

        bool ehMaster = Runner.IsSharedModeMasterClient;

        if (ehMaster)
        {
            Runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        Vector3 posicaoSpawn = ehMaster ? new Vector3(-2, 2, -25) : new Vector3(2, 2, -25);

        NetworkObject objPlayer = Runner.Spawn(playerPrefab, posicaoSpawn, Quaternion.identity);
        Runner.SetPlayerObject(player, objPlayer);
    }
}


