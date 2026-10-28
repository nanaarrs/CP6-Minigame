using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class CorridaPlayer : NetworkBehaviour
{
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private MeshRenderer meshRenderer;

    [Networked] public int Pontos { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            meshRenderer.material.color = Runner.IsSharedModeMasterClient ? Color.blue : Color.red;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GerenciadorPartida.Instance != null && GerenciadorPartida.Instance.JogoFinalizado)
            return;

        if (!HasStateAuthority) return;

        float moverX = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float moverZ = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 direcao = new Vector3(moverX, 0, moverZ).normalized;
        controller.Move(direcao * velocidade * Runner.DeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.CompareTag("Moeda"))
        {
            NetworkObject moedaNetObj = other.GetComponent<NetworkObject>();
            if (moedaNetObj != null)
            {
                Pontos += 10;
                Runner.Despawn(moedaNetObj);
            }
        }
    }
}
